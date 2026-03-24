using Domain.Interfaces.Services;
using Domain.Settings;
using Domain.SharedKernel;
using Domain.Users.Errors;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

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
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Doc2Pod Support", _emailSettings.Email));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls, ct);
                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password, ct);
                await smtp.SendAsync(message, ct);
                await smtp.DisconnectAsync(true, ct);

                _logger.LogInformation("Email sent successfully to {Email}", to);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}: {Message}", to, ex.Message);
                return Result.Failure(new Error("Email.SendFailed", ex.Message));
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

        public async Task<Result> SendActivationReminderEmailAsync(string email, string name, string confirmationLink, CancellationToken ct = default)
        {
            var body = GetActivationReminderHtml(name, confirmationLink);
            return await SendEmailAsync(email, "Activate Your Account - Doc2Pod", body, ct);
        }

        private string GetEmailLayout(string content) => $@"
        <div dir='ltr' style='background-color: #121212; color: #ffffff; font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; padding: 40px; line-height: 1.6;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: #1e1e1e; border-radius: 16px; overflow: hidden; border: 1px solid #333; box-shadow: 0 10px 30px rgba(0,0,0,0.5);'>
                <div style='background-color: #FFD700; padding: 30px; text-align: center;'>
                    <h1 style='color: #000; margin: 0; font-size: 28px; font-weight: 800; letter-spacing: 1px;'>Doc2Pod</h1>
                    <p style='color: #000; margin: 5px 0 0 0; font-size: 14px; opacity: 0.8;'>AI-Powered Educational Podcasts</p>
                </div>
        
                <div style='padding: 40px 30px;'>
                    {content}
                </div>
        
                <div style='background-color: #1a1a1a; padding: 25px; text-align: center; font-size: 12px; color: #666; border-top: 1px solid #222;'>
                    <p style='margin: 0;'>This is an automated security notification from Doc2Pod AI System.</p>
                    <p style='margin: 5px 0 0 0;'>© {DateTime.Now.Year} Doc2Pod Team. Cairo, Egypt.</p>
                </div>
            </div>
        </div>";

        private string GetConfirmationHtml(string name, string link) => GetEmailLayout($@"
        <h2 style='color: #FFD700; margin-top: 0;'>Welcome to the Future, {name}!</h2>
        <p>We're thrilled to have you join Doc2Pod. Your journey of turning PDFs into natural Egyptian Arabic podcasts starts here.</p>
        <p>Please verify your email to activate your account and access your dashboard:</p>
        <div style='text-align: center; margin: 35px 0;'>
            <a href='{link}' style='background: #FFD700; color: #000; padding: 14px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; font-size: 16px;'>Verify My Account</a>
        </div>
        <p style='font-size: 0.85em; color: #888;'>If the button doesn't work, copy this link:<br/>
        <span style='color: #FFD700; word-break: break-all;'>{link}</span></p>");

        private string GetActivationReminderHtml(string name, string link) => GetEmailLayout($@"
        <h2 style='color: #FFD700; margin-top: 0;'>Account Activation Required</h2>
        <p>Hi {name},</p>
        <p>It looks like you're trying to access your account, but your email address hasn't been verified yet.</p>
        <p>To ensure your security and enable all <b>Doc2Pod</b> features, please confirm your email below:</p>
        <div style='text-align: center; margin: 35px 0;'>
            <a href='{link}' style='background: #FFD700; color: #000; padding: 14px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; font-size: 16px; box-shadow: 0 4px 15px rgba(255, 215, 0, 0.2);'>Activate My Account</a>
        </div>
        <p style='color: #aaa; font-size: 0.9em;'>Once verified, you can proceed with your request normally.</p>");

        private string GetResetPasswordHtml(string name, string link) => GetEmailLayout($@"
        <h2 style='color: #FFD700; margin-top: 0;'>Reset Your Password</h2>
        <p>Hi {name},</p>
        <p>We received a request to reset your password. No worries, it happens to the best of us!</p>
        <p>Click the button below to set a new password:</p>
        <div style='text-align: center; margin: 35px 0;'>
            <a href='{link}' style='background: #FFD700; color: #000; padding: 14px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; font-size: 16px;'>Reset Password</a>
        </div>
        <p style='color: #f44336; font-size: 0.85em;'>If you didn't request this, please ignore this email or contact support if you have concerns.</p>");

        private string GetEmailChangeConfirmationHtml(string name, string link) => GetEmailLayout($@"
        <h2 style='color: #FFD700; margin-top: 0;'>Confirm Your New Email</h2>
        <p>Hi {name},</p>
        <p>You're almost there! Please click the button below to confirm your new email address and complete the update:</p>
        <div style='text-align: center; margin: 35px 0;'>
            <a href='{link}' style='background: #FFD700; color: #000; padding: 14px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; font-size: 16px;'>Confirm New Email</a>
        </div>");

        private string GetEmailChangeWarningHtml(string name) => GetEmailLayout($@"
        <h2 style='color: #f44336; margin-top: 0;'>Security Alert!</h2>
        <p>Hi {name},</p>
        <p>A request was recently made to change the email address associated with your <b>Doc2Pod</b> account.</p>
        <p style='background: #2a2a2a; padding: 15px; border-left: 4px solid #f44336; border-radius: 4px;'>
            <b>If this was NOT you:</b> Please secure your account by changing your password immediately and contacting our support team.
        </p>
        <p>If this was you, you can safely ignore this warning.</p>");
    }
}