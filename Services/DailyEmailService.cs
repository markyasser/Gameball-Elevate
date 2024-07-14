using Gameball_Elevate.Ops;
using Gameball_Elevate.Services.SMTP;

namespace Gameball_Elevate.Services
{
    public class DailyEmailService
    {
        private readonly UserOps _userRepository;
        private readonly IEmailService _emailService;

        public DailyEmailService(UserOps userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task SendDailyEmails()
        {
            var users = await _userRepository.GetUsersWithLessThan100PointsAsync();

            foreach (var user in users)
            {
                try
                {
                    Console.WriteLine($"Sending email to user {user.UserName} --- {user.Email}...");
                    await _emailService.SendEmailAsync(user.Email, "Daily Reminder, Low Points Alert", $"Hello {user.UserName}, you have less than 100 points.\n Hurry to buy new products");
                    Console.WriteLine($"Email Sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email to {user.Email}: {ex.Message}");    
                }
            }
        }
    }

}
