using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using HotByte.Modules.Identity.Application.DTOs;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Application.Services;
using HotByte.Modules.Identity.Domain.Entities;
using HotByte.Modules.Identity.Infrastructure;
using HotByte.Modules.Identity.Infrastructure.Repositories;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Identity.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private ServiceProvider _sp = null!;
        private UserManager<AppUser> _userManager = null!;
        private IdentityDbContext _dbContext = null!;
        private Mock<IAuditLogRepository> _auditMock = null!;
        private UserService _service = null!;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<IdentityDbContext>(o =>
                o.UseInMemoryDatabase("UserSvc_" + Guid.NewGuid()));

            services.AddIdentity<AppUser, IdentityRole<int>>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            _sp = services.BuildServiceProvider();
            _userManager = _sp.GetRequiredService<UserManager<AppUser>>();
            _dbContext = _sp.GetRequiredService<IdentityDbContext>();

            // Seed roles
            foreach (var role in new[] { "Customer", "RestaurantOwner", "Admin" })
            {
                _dbContext.Roles.Add(new IdentityRole<int> { Name = role, NormalizedName = role.ToUpper() });
            }
            _dbContext.SaveChanges();

            _auditMock = new Mock<IAuditLogRepository>();
            var repo = new UserRepository(_userManager);

            _service = new UserService(repo, _userManager, _auditMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager.Dispose();
            _dbContext.Dispose();
            _sp.Dispose();
        }

        [Test]
        public async Task RegisterCustomer_ValidInput_CreatesCustomerUser()
        {
            var dto = new RegisterCustomerDto(
                Name: "Alice",
                Email: "alice@x.com",
                Password: "Test@1234",
                Phone: "555-1234",
                Gender: "Female",
                Address: "1 Main St");

            var result = await _service.RegisterCustomerAsync(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo("alice@x.com"));
            Assert.That(result.Role, Is.EqualTo(UserRole.Customer));
        }

        [Test]
        public async Task RegisterCustomer_DuplicateEmail_ReturnsNull()
        {
            var dto = new RegisterCustomerDto(
                "Bob", "bob@x.com", "Test@1234", "555", null, null);

            var first = await _service.RegisterCustomerAsync(dto);
            var second = await _service.RegisterCustomerAsync(dto);

            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Null);
        }

        [Test]
        public async Task CreateRestaurantOwnerAccount_ValidInput_ReturnsUserId()
        {
            var userId = await _service.CreateRestaurantOwnerAccountAsync(
                "OwnerDave", "dave@hotbyte.com", "Owner@1234", "555-9999");

            Assert.That(userId, Is.GreaterThan(0));

            var user = await _userManager.FindByEmailAsync("dave@hotbyte.com");
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Role, Is.EqualTo(UserRole.RestaurantOwner));
        }

        [Test]
        public void CreateRestaurantOwnerAccount_InvalidPassword_Throws()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.CreateRestaurantOwnerAccountAsync(
                    "Bad", "bad@x.com", "short", "555"));
        }
    }
}
