using System.Net;
using System.Net.Mail;

namespace Wulikunkun.Utility
{
    public static class SendEmail
    {
        public static void Send(string address, string title, string content)
        {
            string sendAccount = "918407369@qq.com";
            string authorizeCode = "fxemczwjopiibeij";
            string receiver = address;
            MailMessage message = new MailMessage();
            MailAddress sendAddress = new MailAddress("918407369@qq.com");
            message.From = sendAddress;
            message.To.Add(receiver);
            message.Subject = title;
            message.Body = content;
            SmtpClient client = new SmtpClient("smtp.qq.com", 25);
            client.Credentials = new NetworkCredential(sendAccount, authorizeCode);
            client.EnableSsl = true;
            client.Send(message);
        }
    }
}