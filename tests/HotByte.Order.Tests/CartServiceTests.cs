using Moq;
using NUnit.Framework;
using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Services;
using HotByte.Modules.Order.Domain.Entities;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Order.Tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<ICartRepository> _cartRepo = null!;
        private Mock<IMenuPublicService> _menuSvc = null!;
        private CartService _service = null!;

        [SetUp]
        public void Setup()
        {
            _cartRepo = new Mock<ICartRepository>();
            _menuSvc = new Mock<IMenuPublicService>();
            _service = new CartService(_cartRepo.Object, _menuSvc.Object);
        }

        [Test]
        public async Task AddToCart_EmptyCart_AddsItemWithDerivedRestaurantId()
        {
            _menuSvc.Setup(m => m.MenuItemExistsAsync(10)).ReturnsAsync(true);
            _menuSvc.Setup(m => m.GetMenuItemRestaurantIdAsync(10)).ReturnsAsync(5);
            _menuSvc.Setup(m => m.GetMenuItemPriceAsync(10)).ReturnsAsync(100m);
            _menuSvc.Setup(m => m.GetMenuItemNameAsync(10)).ReturnsAsync("Pizza");

            var cart = new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem>() };
            _cartRepo.Setup(r => r.GetByUserIdAsync(7)).ReturnsAsync(cart);
            _cartRepo.Setup(r => r.GetCartItemAsync(1, 10)).ReturnsAsync((CartItem?)null);
            _cartRepo.Setup(r => r.AddCartItemAsync(It.IsAny<CartItem>())).Returns(Task.CompletedTask);

            CartItem? added = null;
            _cartRepo.Setup(r => r.AddCartItemAsync(It.IsAny<CartItem>()))
                .Callback<CartItem>(c => added = c)
                .Returns(Task.CompletedTask);

            await _service.AddToCartAsync(7, new AddToCartDto(10, 2));

            Assert.That(added, Is.Not.Null);
            // Requirement 5: RestaurantId is set server-side, not from client
            Assert.That(added!.RestaurantId, Is.EqualTo(5));
            Assert.That(added.UnitPrice, Is.EqualTo(100m));
            Assert.That(added.Quantity, Is.EqualTo(2));
            Assert.That(added.TotalPrice, Is.EqualTo(200m));
        }

        [Test]
        public void AddToCart_NonexistentMenuItem_Throws()
        {
            _menuSvc.Setup(m => m.MenuItemExistsAsync(999)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddToCartAsync(7, new AddToCartDto(999, 1)));
        }

        [Test]
        public void AddToCart_DifferentRestaurant_Throws()
        {
            // Requirement 3: cart can only have items from ONE restaurant
            _menuSvc.Setup(m => m.MenuItemExistsAsync(20)).ReturnsAsync(true);
            _menuSvc.Setup(m => m.GetMenuItemRestaurantIdAsync(20)).ReturnsAsync(8);
            _menuSvc.Setup(m => m.GetMenuItemPriceAsync(20)).ReturnsAsync(50m);
            _menuSvc.Setup(m => m.GetMenuItemNameAsync(20)).ReturnsAsync("Burger");

            var cart = new Cart
            {
                Id = 1,
                UserId = 7,
                CartItems = new List<CartItem>
                {
                    new() { Id = 1, CartId = 1, MenuItemId = 10, RestaurantId = 5, Quantity = 1, UnitPrice = 100m, TotalPrice = 100m }
                }
            };
            _cartRepo.Setup(r => r.GetByUserIdAsync(7)).ReturnsAsync(cart);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddToCartAsync(7, new AddToCartDto(20, 1)));
        }

        [Test]
        public async Task AddToCart_SameRestaurantExistingItem_IncrementsQuantity()
        {
            _menuSvc.Setup(m => m.MenuItemExistsAsync(10)).ReturnsAsync(true);
            _menuSvc.Setup(m => m.GetMenuItemRestaurantIdAsync(10)).ReturnsAsync(5);
            _menuSvc.Setup(m => m.GetMenuItemPriceAsync(10)).ReturnsAsync(100m);
            _menuSvc.Setup(m => m.GetMenuItemNameAsync(10)).ReturnsAsync("Pizza");

            var existing = new CartItem
            { Id = 1, CartId = 1, MenuItemId = 10, RestaurantId = 5, Quantity = 1, UnitPrice = 100m, TotalPrice = 100m };
            var cart = new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem> { existing } };
            _cartRepo.Setup(r => r.GetByUserIdAsync(7)).ReturnsAsync(cart);
            _cartRepo.Setup(r => r.GetCartItemAsync(1, 10)).ReturnsAsync(existing);

            await _service.AddToCartAsync(7, new AddToCartDto(10, 3));

            Assert.That(existing.Quantity, Is.EqualTo(4));
            Assert.That(existing.TotalPrice, Is.EqualTo(400m));
            _cartRepo.Verify(r => r.UpdateCartItemAsync(existing), Times.Once);
        }

        [Test]
        public void RemoveCartItem_OtherUsersItem_Throws()
        {
            // Requirement 6: ownership check
            var item = new CartItem { Id = 50, CartId = 99, MenuItemId = 1 };
            _cartRepo.Setup(r => r.GetCartItemByIdAsync(50)).ReturnsAsync(item);
            _cartRepo.Setup(r => r.GetByUserIdAsync(7))
                .ReturnsAsync(new Cart { Id = 1, UserId = 7 });

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.RemoveCartItemAsync(7, 50));
        }
    }
}
