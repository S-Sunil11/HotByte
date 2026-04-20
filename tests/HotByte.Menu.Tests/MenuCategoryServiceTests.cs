using Moq;
using NUnit.Framework;
using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Application.Services;
using HotByte.Modules.Menu.Domain.Entities;

namespace HotByte.Menu.Tests
{
    [TestFixture]
    public class MenuCategoryServiceTests
    {
        private Mock<IMenuCategoryRepository> _repoMock = null!;
        private MenuCategoryService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IMenuCategoryRepository>();
            _service = new MenuCategoryService(_repoMock.Object);
        }

        [Test]
        public async Task CreateCategory_ValidInput_ReturnsDto()
        {
            _repoMock.Setup(r => r.AddAsync(It.IsAny<MenuCategory>()))
                .Callback<MenuCategory>(c => c.Id = 1)
                .Returns(Task.CompletedTask);

            var dto = new CreateMenuCategoryDto("Pizza", "Italian flatbread", null);
            var result = await _service.CreateCategoryAsync(dto);

            Assert.That(result.Name, Is.EqualTo("Pizza"));
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllCategories_ReturnsAllDtos()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<MenuCategory>
            {
                new() { Id = 1, Name = "Pizza" },
                new() { Id = 2, Name = "Burger" }
            });

            var result = await _service.GetAllCategoriesAsync();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Pizza"));
        }

        [Test]
        public async Task UpdateCategory_NotFound_ReturnsFalse()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((MenuCategory?)null);

            var result = await _service.UpdateCategoryAsync(99,
                new UpdateMenuCategoryDto("X", null, null, null));

            Assert.That(result, Is.False);
        }
    }
}
