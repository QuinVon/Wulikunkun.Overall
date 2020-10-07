using System.Net;
using System.Net.Mail;

namespace Wulikunkun.Utility
{
    public static class SendEmail
    {
        public static void Send(string address, string title, string content)
        {
            string sendAccount = "QuinVon@outlook.com";
            string authorizeCode = "wangkun8899!";
            string receiver = address;
            MailMessage message = new MailMessage();
            MailAddress sendAddress = new MailAddress("QuinVon@outlook.com");
            message.From = sendAddress;
            message.To.Add(receiver);
            message.Subject = title;
            message.Body = content;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 25);
            client.Credentials = new NetworkCredential(sendAccount, authorizeCode);
            client.EnableSsl = true;
            client.Send(message);
        }
    }
}