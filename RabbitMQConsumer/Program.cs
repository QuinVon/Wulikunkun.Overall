using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "127.0.0.1";
            connectionFactory.UserName = "wangkun";
            connectionFactory.Password = "88888888";
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //错误的设置持久化队列的方法，QueueDeclare()方法的最后两个参数exclusive和autodelete必须同时设置为false
                    //channel.QueueDeclare("First Queue", true);
                    channel.QueueDeclare("First Queue", true, false, false);

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("First Queue", true, consumer);
                    consumer.Received += (model, message) =>
                    {
                        var messageBodyBytes = message.Body.ToArray();
                        string messageBody = Encoding.Default.GetString(messageBodyBytes);
                        Console.WriteLine($"已经接收到消息主体，主体内容为 ：{messageBody}");
                    };
                    Console.ReadLine();
                }
            }
        }
    }
}
