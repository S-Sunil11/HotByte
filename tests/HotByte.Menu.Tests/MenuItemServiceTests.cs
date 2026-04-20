using Moq;
using NUnit.Framework;
using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Application.Services;
using HotByte.Modules.Menu.Domain.Entities;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Menu.Tests
{
    [TestFixture]
    public class MenuItemServiceTests
    {
        private Mock<IMenuItemRepository> _repoMock = null!;
        private Mock<IRestaurantPublicService> _restaurantMock = null!;
        private MenuItemService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IMenuItemRepository>();
            _restaurantMock = new Mock<IRestaurantPublicService>();
            _service = new MenuItemService(_repoMock.Object, _restaurantMock.Object);
        }

        private CreateMenuItemDto NewCreate(int restaurantId = 1) => new(
            Name: "Pizza",
            Description: "Cheesy",
            Ingredients: "Dough",
            Price: 199.99m,
            DiscountPrice: null,
            ImageUrl: null,
            CategoryId: 1,
            RestaurantId: restaurantId);

        [Test]
        public async Task Create_AsAdmin_AllowedForAnyRestaurant()
        {
            _repoMock.Setup(r => r.AddAsync(It.IsAny<MenuItem>()))
                .Callback<MenuItem>(m => m.Id = 1)
                .Returns(Task.CompletedTask);

            var result = await _service.CreateMenuItemAsync(NewCreate(5), userId: 99, role: "Admin");

            Assert.That(result.Name, Is.EqualTo("Pizza"));
            _restaurantMock.Verify(
                r => r.IsOwnerOfRestaurantAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test]
        public void Create_AsOwner_OfDifferentRestaurant_Throws()
        {
            _restaurantMock.Setup(r => r.IsOwnerOfRestaurantAsync(7, 5)).ReturnsAsync(false);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.CreateMenuItemAsync(NewCreate(5), userId: 7, role: "RestaurantOwner"));
        }

        [Test]
        public async Task Create_AsOwner_OfSameRestaurant_Succeeds()
        {
            _restaurantMock.Setup(r => r.IsOwnerOfRestaurantAsync(7, 5)).ReturnsAsync(true);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<MenuItem>()))
                .Callback<MenuItem>(m => m.Id = 1)
                .Returns(Task.CompletedTask);

            var result = await _service.CreateMenuItemAsync(NewCreate(5), userId: 7, role: "RestaurantOwner");

            Assert.That(result.RestaurantId, Is.EqualTo(5));
        }

        [Test]
        public void Update_AsOwner_OfDifferentRestaurant_Throws()
        {
            _repoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new MenuItem { Id = 10, RestaurantId = 5, Name = "X" });
            _restaurantMock.Setup(r => r.IsOwnerOfRestaurantAsync(7, 5)).ReturnsAsync(false);

            var dto = new UpdateMenuItemDto(
                "New", null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.UpdateMenuItemAsync(10, dto, userId: 7, role: "RestaurantOwner"));
        }

        [Test]
        public void Delete_AsOwner_OfDifferentRestaurant_Throws()
        {
            _repoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new MenuItem { Id = 10, RestaurantId = 5, Name = "X" });
            _restaurantMock.Setup(r => r.IsOwnerOfRestaurantAsync(7, 5)).ReturnsAsync(false);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.DeleteMenuItemAsync(10, userId: 7, role: "RestaurantOwner"));
        }

        [Test]
        public async Task Filter_ByRestaurantAndDietaryType_PassesParamsToRepository()
        {
            _repoMock.Setup(r => r.FilterAsync(1, 2, DietaryType.Veg, 10m, 500m))
                .ReturnsAsync(new List<MenuItem>
                {
                    new() { Id = 1, Name = "Veg", RestaurantId = 1, CategoryId = 2, Price = 200, DietaryType = DietaryType.Veg }
                });

            var result = await _service.FilterMenuItemsAsync(1, 2, DietaryType.Veg, 10m, 500m);

            Assert.That(result, Has.Count.EqualTo(1));
            _repoMock.Verify(r => r.FilterAsync(1, 2, DietaryType.Veg, 10m, 500m), Times.Once);
        }
    }
}
