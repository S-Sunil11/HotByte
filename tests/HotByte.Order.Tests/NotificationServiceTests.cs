using Moq;
using NUnit.Framework;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Services;
using HotByte.Modules.Order.Domain.Entities;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Order.Tests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<INotificationRepository> _repo = null!;
        private Mock<IEmailService> _email = null!;
        private Mock<IUserPublicService> _user = null!;
        private NotificationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<INotificationRepository>();
            _email = new Mock<IEmailService>();
            _user = new Mock<IUserPublicService>();
            _service = new NotificationService(_repo.Object, _email.Object, _user.Object);
        }

        [Test]
        public async Task SendNotification_PersistsAndSendsEmail()
        {
            // Requirement 1: actual email dispatched
            _user.Setup(u => u.GetUserEmailAsync(7)).ReturnsAsync("alice@x.com");
            _user.Setup(u => u.GetUserNameAsync(7)).ReturnsAsync("Alice");

            await _service.SendNotificationAsync(7, 42, "OrderPlaced", "Order placed!");

            _repo.Verify(r => r.AddAsync(It.Is<OrderNotification>(n =>
                n.UserId == 7 && n.OrderId == 42 && n.Type == NotificationType.OrderPlaced)), Times.Once);

            _email.Verify(e => e.SendAsync(
                "alice@x.com",
                It.Is<string>(s => s.Contains("OrderPlaced")),
                It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task SendNotification_EmailFailureDoesNotBlock()
        {
            _user.Setup(u => u.GetUserEmailAsync(7)).ReturnsAsync("alice@x.com");
            _user.Setup(u => u.GetUserNameAsync(7)).ReturnsAsync("Alice");
            _email.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("SMTP down"));

            // Should NOT throw — notification persisted regardless of email failure
            Assert.DoesNotThrowAsync(async () =>
                await _service.SendNotificationAsync(7, 42, "OrderPlaced", "msg"));

            _repo.Verify(r => r.AddAsync(It.IsAny<OrderNotification>()), Times.Once);
        }

        [Test]
        public async Task MarkAsRead_OwnNotification_Succeeds()
        {
            _repo.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(new OrderNotification { Id = 5, UserId = 7, OrderId = 1 });
            _repo.Setup(r => r.MarkAsReadAsync(5)).ReturnsAsync(true);

            var result = await _service.MarkAsReadAsync(5, 7);

            Assert.That(result, Is.True);
        }

        [Test]
        public void MarkAsRead_OtherUsersNotification_Throws()
        {
            // Requirement 6: ownership enforced
            _repo.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(new OrderNotification { Id = 5, UserId = 99, OrderId = 1 });

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.MarkAsReadAsync(5, 7));
        }
    }
}
