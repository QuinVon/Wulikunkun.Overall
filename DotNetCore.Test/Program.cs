using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCore.Test
{
    class PerformanceMetric
    {
        static readonly Random _random = new Random();
        public int Processor { get; set; }
        public long Memory { get; set; }
        public long Network { get; set; }
        public override string ToString() => $"Processor:{Processor},Memory:{Memory},Network:{Network}";
        public static PerformanceMetric Create() => new PerformanceMetric { Processor = _random.Next(1, 10), Memory = _random.Next(1, 10) * 1028, Network = _random.Next(1, 10) * 1024 };
    }

    //IHostedService表示的是实线宿主服务的接口，对于实现宿主服务接口的服务，在进行依赖注入时，可以使用两种方法进行注册
    class PerformanceMetricCollector : IHostedService
    {
        IDisposable _scheduler;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            void ConsoleInfo(object obj) => Console.WriteLine(PerformanceMetric.Create());

            _scheduler = new Timer(ConsoleInfo, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler?.Dispose();
            return Task.CompletedTask;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
