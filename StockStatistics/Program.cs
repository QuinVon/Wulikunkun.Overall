using NLog;
using System;
using Topshelf;

namespace StockStatistics
{
    public class StockStatistics
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public StockStatistics()
        {

        }

        public void Start()
        {
            logger.Info("服务启动");
        }

        public void Stop()
        {

        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<StockStatistics>(s =>
                {
                    s.ConstructUsing(name => new StockStatistics());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                /* 配置服务随系统自动启动 */
                x.StartAutomatically();
                x.SetDescription("the stock statistics service");
                x.SetDisplayName("StockStatistics");
                x.SetServiceName("StockStatistics");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;

        }
    }
}
