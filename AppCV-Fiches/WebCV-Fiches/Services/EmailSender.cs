using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCV_Fiches.Controllers;
using WebCV_Fiches.Helpers;

namespace WebCV_Fiches.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly ILogger logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<AccountApiController> logger)
        {
            Options = optionsAccessor.Value;
            this.logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public async Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("info@lgs.com", "LGS"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking { Enable = false }
            };

            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                logger.LogError("erreur d'envoi de mail");
                logger.LogError("email:" + email);
                logger.LogError("subject:" + subject);
                logger.LogError("message:" + message);
                logger.LogError("http response status code:" + response.StatusCode);
                logger.LogError("http response sbody" + response.Body.ReadAsStringAsync().Result);
            }
        }
    }
}
