using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MovieBestAuthorizeBased.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromMail = "";//OutLook
            string fromPassword = "";

            MailMessage message = new ();
            message.From =new MailAddress( fromMail);
            message.Subject = subject;
            message.Body =$"<html><body>{htmlMessage} </body></html>" ;
            message.To.Add(email);
            message.IsBodyHtml = true;
            SmtpClient smtpClient = new ("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };
            smtpClient.Send(message) ;



        }
    }
}
