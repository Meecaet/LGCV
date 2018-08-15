using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebCV_Fiches.Services;

namespace WebCV_Fiches.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirmer votre adresse",
                $"Veuillez cliquer sur ce lien pour confirmer votre adresse email et activer votre compte: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
