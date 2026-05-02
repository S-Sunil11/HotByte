using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using HotByte.Modules.Identity.Application.DTOs;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Application.Services;
using HotByte.Modules.Identity.Domain.Entities;
using HotByte.Modules.Identity.Infrastructure;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Identity.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private ServiceProvider _sp = null!;
        private UserManager<AppUser> _userManager = null!;
        private SignInManager<AppUser> _signInManager = null!;
        private IConfiguration _config = null!;
        private Mock<IAuditLogRepository> _auditMock = null!;
        private Mock<IIdentityEmailService> _emailMock = null!;
        private AuthService _service = null!;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<IdentityDbContext>(o =>
                o.UseInMemoryDatabase("AuthSvc_" + Guid.NewGuid()));

            services.AddIdentity<AppUser, IdentityRole<int>>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            _sp = services.BuildServiceProvider();
            _userManager = _sp.GetRequiredService<UserManager<AppUser>>();
            _signInManager = _sp.GetRequiredService<SignInManager<AppUser>>();

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "HotByte@SuperSecretKey@2026#FoodOrdering",
                    ["Jwt:Issuer"] = "HotByteApi",
                    ["Jwt:Audience"] = "HotByteClient"
                }).Build();

            _auditMock = new Mock<IAuditLogRepository>();
            _emailMock = new Mock<IIdentityEmailService>();

            _service = new AuthService(_userManager, _signInManager, _config, _auditMock.Object, _emailMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager.Dispose();
            _sp.Dispose();
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var user = new AppUser
            {
                UserName = "test@hotbyte.com",
                Email = "test@hotbyte.com",
                Name = "Test",
                Role = UserRole.Customer
            };
            var created = await _userManager.CreateAsync(user, "Test@1234");
            Assert.That(created.Succeeded, Is.True);

            var result = await _service.LoginAsync(new LoginDto("test@hotbyte.com", "Test@1234"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.AccessToken, Is.Not.Empty);
            Assert.That(result.Email, Is.EqualTo("test@hotbyte.com"));
            Assert.That(result.Role, Is.EqualTo("Customer"));
        }

        [Test]
        public async Task Login_WithUnknownEmail_ReturnsNull()
        {
            var result = await _service.LoginAsync(new LoginDto("missing@x.com", "whatever"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Login_WithWrongPassword_ReturnsNull()
        {
            var user = new AppUser
            {
                UserName = "wp@hotbyte.com",
                Email = "wp@hotbyte.com",
                Name = "WP",
                Role = UserRole.Customer
            };
            await _userManager.CreateAsync(user, "Correct@1234");

            var result = await _service.LoginAsync(new LoginDto("wp@hotbyte.com", "Wrong@1234"));
            Assert.That(result, Is.Null);
        }
    }
}
