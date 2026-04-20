using Moq;
using NUnit.Framework;
using HotByte.Modules.Restaurant.Application.DTOs;
using HotByte.Modules.Restaurant.Application.Interfaces;
using HotByte.Modules.Restaurant.Application.Services;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Restaurant.Tests
{
    [TestFixture]
    public class RestaurantServiceTests
    {
        private Mock<IRestaurantRepository> _repoMock = null!;
        private Mock<IUserPublicService> _userMock = null!;
        private RestaurantService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IRestaurantRepository>();
            _userMock = new Mock<IUserPublicService>();
            _service = new RestaurantService(_repoMock.Object, _userMock.Object);
        }

        [Test]
        public async Task CreateRestaurantWithOwner_CreatesUserThenRestaurant()
        {
            _userMock
                .Setup(u => u.CreateRestaurantOwnerAccountAsync(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string?>()))
                .ReturnsAsync(42);

            HotByte.Modules.Restaurant.Domain.Entities.Restaurant? captured = null;
            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<HotByte.Modules.Restaurant.Domain.Entities.Restaurant>()))
                .Callback<HotByte.Modules.Restaurant.Domain.Entities.Restaurant>(e =>
                {
                    e.Id = 7;
                    captured = e;
                })
                .Returns(Task.CompletedTask);

            var dto = new CreateRestaurantWithOwnerDto(
                Name: "Pizza Palace",
                Description: "Italian",
                Location: "Chennai",
                ContactNumber: "555-1000",
                Email: "pizza@x.com",
                ImageUrl: null,
                Category: "Pizza",
                OwnerName: "Pedro",
                OwnerEmail: "pedro@x.com",
                OwnerPassword: "Owner@1234",
                OwnerPhone: "555-2000");

            var result = await _service.CreateRestaurantWithOwnerAsync(dto);

            Assert.That(result.Name, Is.EqualTo("Pizza Palace"));
            Assert.That(result.OwnerUserId, Is.EqualTo(42));
            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.OwnerUserId, Is.EqualTo(42));

            _userMock.Verify(u => u.CreateRestaurantOwnerAccountAsync(
                "Pedro", "pedro@x.com", "Owner@1234", "555-2000"), Times.Once);
        }

        [Test]
        public void Update_AsOwner_OwningDifferentRestaurant_Throws()
        {
            _repoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new HotByte.Modules.Restaurant.Domain.Entities.Restaurant
                { Id = 10, OwnerUserId = 99, Name = "Other", Location = "X" });

            var dto = new UpdateRestaurantDto("NewName", null, null, null, null, null, null);

            // userId 7 is not owner (owner is 99)
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.UpdateRestaurantAsync(10, dto, userId: 7, role: "RestaurantOwner"));
        }

        [Test]
        public async Task Update_AsAdmin_AnyRestaurant_Succeeds()
        {
            _repoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new HotByte.Modules.Restaurant.Domain.Entities.Restaurant
                { Id = 10, OwnerUserId = 99, Name = "Other", Location = "X" });

            var dto = new UpdateRestaurantDto("NewName", null, null, null, null, null, null);

            var result = await _service.UpdateRestaurantAsync(10, dto, userId: 1, role: "Admin");
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Update_AsOwner_OwningSameRestaurant_Succeeds()
        {
            _repoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new HotByte.Modules.Restaurant.Domain.Entities.Restaurant
                { Id = 10, OwnerUserId = 7, Name = "Mine", Location = "X" });

            var dto = new UpdateRestaurantDto("Renamed", null, null, null, null, null, null);

            var result = await _service.UpdateRestaurantAsync(10, dto, userId: 7, role: "RestaurantOwner");
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SearchRestaurants_FiltersByNameAndCategory()
        {
            _repoMock.Setup(r => r.SearchAsync("Spice", "Indian"))
                .ReturnsAsync(new List<HotByte.Modules.Restaurant.Domain.Entities.Restaurant>
                {
                    new() { Id = 1, Name = "Spice Garden", Location = "X", Category = "Indian" }
                });

            var results = await _service.SearchRestaurantsAsync("Spice", "Indian");

            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo("Spice Garden"));
        }
    }
}
