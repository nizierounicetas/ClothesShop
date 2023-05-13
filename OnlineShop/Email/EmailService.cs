using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace OnlineShop.Email
{
    public class EmailService
    {
        public async Task SendEmailSync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(WC.ShopTitle, WC.SiteEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(WC.SmptServerName, 465, true);
                await client.AuthenticateAsync(WC.SiteEmail, WC.SiteEmailPassword);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
