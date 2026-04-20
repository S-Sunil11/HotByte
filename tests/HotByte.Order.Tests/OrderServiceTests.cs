using Moq;
using NUnit.Framework;
using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Services;
using HotByte.Modules.Order.Domain.Entities;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Order.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepo = null!;
        private Mock<ICartRepository> _cartRepo = null!;
        private Mock<INotificationService> _notif = null!;
        private Mock<IRestaurantPublicService> _restaurantSvc = null!;
        private OrderService _service = null!;

        [SetUp]
        public void Setup()
        {
            _orderRepo = new Mock<IOrderRepository>();
            _cartRepo = new Mock<ICartRepository>();
            _notif = new Mock<INotificationService>();
            _restaurantSvc = new Mock<IRestaurantPublicService>();
            _service = new OrderService(_orderRepo.Object, _cartRepo.Object, _notif.Object, _restaurantSvc.Object);
        }

        [Test]
        public async Task PlaceOrder_CreatesOrderAndSendsNotification()
        {
            var cart = new Cart
            {
                Id = 1,
                UserId = 7,
                CartItems = new List<CartItem>
                {
                    new() { Id = 1, CartId = 1, MenuItemId = 10, MenuItemName = "Pizza", UnitPrice = 100m, Quantity = 2, TotalPrice = 200m, RestaurantId = 5 }
                }
            };
            _cartRepo.Setup(r => r.GetByUserIdAsync(7)).ReturnsAsync(cart);
            _restaurantSvc.Setup(r => r.GetRestaurantNameAsync(5)).ReturnsAsync("Pizza Palace");
            _orderRepo.Setup(r => r.AddAsync(It.IsAny<HotByte.Modules.Order.Domain.Entities.Order>()))
                .Callback<HotByte.Modules.Order.Domain.Entities.Order>(o => o.Id = 42)
                .Returns(Task.CompletedTask);

            var dto = new CreateOrderDto("Addr", "555", PaymentMethod.UPI, null);
            var result = await _service.PlaceOrderAsync(7, dto);

            Assert.That(result.RestaurantId, Is.EqualTo(5));
            Assert.That(result.RestaurantName, Is.EqualTo("Pizza Palace"));
            Assert.That(result.TotalAmount, Is.EqualTo(200m));

            // Requirement 1: notification sent on placement
            _notif.Verify(n => n.SendNotificationAsync(7, 42, "OrderPlaced", It.IsAny<string>()), Times.Once);
            _cartRepo.Verify(r => r.ClearCartAsync(1), Times.Once);
        }

        [Test]
        public void PlaceOrder_EmptyCart_Throws()
        {
            _cartRepo.Setup(r => r.GetByUserIdAsync(7))
                .ReturnsAsync(new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem>() });

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.PlaceOrderAsync(7, new CreateOrderDto(null, null)));
        }

        [Test]
        public async Task GetOrderById_AsCustomer_OwnOrder_Returns()
        {
            _orderRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new HotByte.Modules.Order.Domain.Entities.Order
                { Id = 1, UserId = 7, RestaurantId = 5, OrderItems = new List<OrderItem>() });

            var result = await _service.GetOrderByIdAsync(1, 7, "Customer");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetOrderById_AsCustomer_OtherUsersOrder_Throws()
        {
            _orderRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new HotByte.Modules.Order.Domain.Entities.Order
                { Id = 1, UserId = 99, RestaurantId = 5, OrderItems = new List<OrderItem>() });

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.GetOrderByIdAsync(1, 7, "Customer"));
        }

        [Test]
        public async Task GetOrderById_AsOwner_OfRestaurant_Returns()
        {
            _orderRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new HotByte.Modules.Order.Domain.Entities.Order
                { Id = 1, UserId = 99, RestaurantId = 5, OrderItems = new List<OrderItem>() });
            _restaurantSvc.Setup(r => r.IsOwnerOfRestaurantAsync(7, 5)).ReturnsAsync(true);

            var result = await _service.GetOrderByIdAsync(1, 7, "RestaurantOwner");
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetOrdersByRestaurant_AsOwner_NotOwning_Throws()
        {
            // Requirement 7 + 6
            _restaurantSvc.Setup(r => r.IsOwnerOfRestaurantAsync(7, 5)).ReturnsAsync(false);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.GetOrdersByRestaurantAsync(5, 7, "RestaurantOwner"));
        }

        [Test]
        public async Task GetMyRestaurantsOrders_ReturnsOrdersForAllOwnedRestaurants()
        {
            // Requirement 7: auto-detect from token
            _restaurantSvc.Setup(r => r.GetOwnedRestaurantIdsAsync(7))
                .ReturnsAsync(new List<int> { 3, 4 });

            _orderRepo.Setup(r => r.GetByRestaurantIdsAsync(It.Is<List<int>>(l => l.SequenceEqual(new[] { 3, 4 }))))
                .ReturnsAsync(new List<HotByte.Modules.Order.Domain.Entities.Order>
                {
                    new() { Id = 1, UserId = 10, RestaurantId = 3, OrderItems = new List<OrderItem>() },
                    new() { Id = 2, UserId = 11, RestaurantId = 4, OrderItems = new List<OrderItem>() }
                });

            var result = await _service.GetMyRestaurantsOrdersAsync(7);

            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetMyRestaurantsOrders_NoOwnedRestaurants_ReturnsEmpty()
        {
            _restaurantSvc.Setup(r => r.GetOwnedRestaurantIdsAsync(7))
                .ReturnsAsync(new List<int>());

            var result = await _service.GetMyRestaurantsOrdersAsync(7);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task UpdateOrderStatus_AsAdmin_SendsStatusEmail()
        {
            _orderRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new HotByte.Modules.Order.Domain.Entities.Order
                { Id = 1, UserId = 7, RestaurantId = 5, Status = OrderStatus.Placed });

            var result = await _service.UpdateOrderStatusAsync(1,
                new UpdateOrderStatusDto(OrderStatus.Confirmed), 99, "Admin");

            Assert.That(result, Is.True);
            // Requirement 1: email sent on status change
            _notif.Verify(n => n.SendNotificationAsync(7, 1, "OrderConfirmed", It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UpdateOrderStatus_AsOwner_NotOwningRestaurant_Throws()
        {
            // Requirement 6
            _orderRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new HotByte.Modules.Order.Domain.Entities.Order
                { Id = 1, UserId = 7, RestaurantId = 5 });
            _restaurantSvc.Setup(r => r.IsOwnerOfRestaurantAsync(99, 5)).ReturnsAsync(false);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.UpdateOrderStatusAsync(1,
                    new UpdateOrderStatusDto(OrderStatus.Confirmed), 99, "RestaurantOwner"));
        }
    }
}
