using DataTransferLib.CommunicationsServices;
using MailKit.Net.Smtp;
using MimeKit;

namespace AuthService.Services
{
    public class EmailSenderService(ILogger<EmailSenderService> logger)
    {
        private readonly string _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? string.Empty;
        private readonly int _smtpPort = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_PORT") ?? string.Empty);
        private readonly string _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? string.Empty;
        private readonly string _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? string.Empty;

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Stardrop", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            logger.LogInformation($"Connecting to {_smtpServer}");
            await client.ConnectAsync(_smtpServer, _smtpPort, true);
            logger.LogInformation($"Logging as  {_smtpUser}");
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            logger.LogInformation($"Sending message");
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

        public string GenerateConfirmationMessage(string confiramtion)
        {
            string confirmationUrl = confiramtion;

            var message = $@"
    <html>
        <head>
            <meta charset='UTF-8'>
            <title>Подтверждение электронной почты</title>
        </head>
        <body style='font-family: Arial, sans-serif;'>
            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                <h2 style='color: #333;'>Подтверждение регистрации</h2>
                <p>Здравствуйте!</p>
                <p>Спасибо за регистрацию. Для подтверждения вашего адреса электронной почты, пожалуйста, нажмите на кнопку ниже:</p>
                <p style='text-align: center;'>
                    <a href='{confirmationUrl}' style='display: inline-block; padding: 10px 20px; font-size: 16px; 
                    color: #fff; background-color: #28a745; text-decoration: none; border-radius: 5px;'>Подтвердить Email</a>
                </p>
                <p>Если кнопка не работает, скопируйте и вставьте следующую ссылку в адресную строку вашего браузера:</p>
                <p><a href='{confirmationUrl}'>{confirmationUrl}</a></p>
                <p>Если вы не создавали учетную запись, просто проигнорируйте это письмо.</p>
                <br>
                <p>С уважением,<br>Команда Stardrop</p>
            </div>
        </body>
    </html>";

            return message;
        }

    }
}
