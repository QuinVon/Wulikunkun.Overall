using System;
using System.Net;
using System.Net.Mail;

namespace Wulikunkun.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                int randomNum = GenerageRandomCode();
                System.Console.WriteLine(randomNum);
            }

            System.Console.ReadLine();
        }

        /// <summary>
        /// 生成一个6位随机数
        /// </summary>
        /// <returns></returns>
        public static int GenerageRandomCode()
        {
            Random random = new Random();
            int randomNum = random.Next(100000, 999999);
            return randomNum;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="address"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
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