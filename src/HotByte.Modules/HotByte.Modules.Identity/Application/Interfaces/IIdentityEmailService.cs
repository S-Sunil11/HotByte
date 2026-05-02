namespace HotByte.Modules.Identity.Application.Interfaces
{
    public interface IIdentityEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
