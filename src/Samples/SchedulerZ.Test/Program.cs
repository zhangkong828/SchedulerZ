using SchedulerZ.Component;
using SchedulerZ.Logging;
using System;

namespace SchedulerZ.Test
{
    class Program
    {
        static Configuration _configuration;
        static ILogger _logger;
        static void Main(string[] args)
        {
            _configuration = Configuration.Create().UseAutofac().BuildContainer();

            _logger = Configuration.LoggerProvider.CreateLogger(typeof(Program).Name);


            _logger.Trace("trace1111");
            _logger.Debug("debug22222222222");
            _logger.Info("info333333333");
            _logger.Warning("warning444444");
            _logger.Error("error55555",new Exception("aaa"));
            _logger.Fatal("fatal6666");

            //Console.WriteLine("over!");
            Console.ReadKey();
        }
    }
}
