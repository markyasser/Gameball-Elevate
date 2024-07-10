namespace Gameball_Elevate.Services.SMTP
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
