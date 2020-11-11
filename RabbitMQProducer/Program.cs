using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = "127.0.0.1",
                UserName = "wangkun",
                Password = "88888888"
            };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //错误的设置持久化队列的方法，QueueDeclare()方法的最后两个参数exclusive和autodelete必须同时设置为false
                    //channel.QueueDeclare("First Queue", true);
                    channel.QueueDeclare("First Queue", true, false, false);

                    channel.CreateBasicProperties().Persistent = true;
                    string message = "First message of first queue";
                    byte[] messageBody = Encoding.Default.GetBytes(message);
                    channel.BasicPublish("", "First Queue", null, messageBody);
                    Console.WriteLine("The message has sent!");
                    Console.Read();
                }
            }
        }
    }
}
