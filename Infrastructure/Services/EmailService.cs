using Domain.Interfaces.Services;
using Domain.Settings;
using Domain.SharedKernel;
using Domain.Users.Errors;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailOptions, ILogger<EmailService> logger)
        {
            _emailSettings = emailOptions.Value;
            _logger = logger;
        }

        public async Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(to))
                return Result.Failure(EmailErrors.Empty);

            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email, "Doc2Pod Support"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                    EnableSsl = true,
                    Timeout = 10000
                };

                await smtpClient.SendMailAsync(mailMessage, ct);
                _logger.LogInformation("Email sent successfully to {Email}", to);
                return Result.Success();
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error occurred while sending email to {Email}", to);
                return Result.Failure(EmailErrors.SendFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {Email}", to);
                return Result.Failure(EmailErrors.RemoteServiceError);
            }
        }


        public async Task<Result> SendConfirmationEmailAsync(string email, string name, string confirmationLink, CancellationToken ct = default)
        {
            var body = GetConfirmationHtml(name, confirmationLink);
            return await SendEmailAsync(email, "Confirm Your Email - Doc2Pod", body, ct);
        }

        public async Task<Result> SendPasswordResetEmailAsync(string email, string name, string token, CancellationToken ct = default)
        {
            var body = GetResetPasswordHtml(name, token);
            return await SendEmailAsync(email, "Reset Password - Doc2Pod", body, ct);
        }

        public async Task<Result> SendEmailChangeWarningAsync(string email, string name, CancellationToken ct = default)
        {
            var body = GetEmailChangeWarningHtml(name);
            return await SendEmailAsync(email, "Security Alert: Email Change Requested - Doc2Pod", body, ct);
        }

        public async Task<Result> SendEmailChangeConfirmationAsync(string email, string name, string link, CancellationToken ct = default)
        {
            var body = GetEmailChangeConfirmationHtml(name, link);
            return await SendEmailAsync(email, "Confirm Your New Email Address - Doc2Pod", body, ct);
        }

        private string GetConfirmationHtml(string name, string link) => $@"
        <div dir='ltr' style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
            <h2 style='color: #4CAF50;'>Welcome {name} to Doc2Pod!</h2>
            <p>We are excited to have you. Please verify your email to start turning your PDFs into Podcasts.</p>
            <a href='{link}' style='background: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Verify My Account</a>
            <p style='margin-top: 20px; font-size: 0.8em; color: #888;'>If you can't click the button, use this link: {link}</p>
        </div>";

        private string GetResetPasswordHtml(string name, string token) => $@"
        <div dir='ltr' style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
            <h2>Reset Password Request</h2>
            <p>Hi {name}, use the code below to reset your password:</p>
            <h1 style='background: #f4f4f4; padding: 10px; text-align: center; letter-spacing: 5px;'>{token}</h1>
            <p>This code is valid for a limited time.</p>
        </div>";

        private string GetEmailChangeWarningHtml(string name) => $@"
        <div dir='ltr' style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #f44336;'>Security Alert</h2>
            <p>Hi {name},</p>
            <p>A request was made to change your email. If this wasn't you, please change your password immediately.</p>
        </div>";

        private string GetEmailChangeConfirmationHtml(string name, string link) => $@"
        <div dir='ltr' style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #2196F3;'>Confirm New Email</h2>
            <p>Hi {name}, click below to confirm your new email address:</p>
            <a href='{link}' style='background: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Confirm Change</a>
        </div>";
    }
}