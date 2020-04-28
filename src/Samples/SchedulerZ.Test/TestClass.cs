using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Test
{
    public abstract class TestClassBase
    {
        public abstract void Run();

        public void Execute()
        {
            Run();
        }
    }

   public class TestClass: TestClassBase
    {
        public override void Run()
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Hello World");
        }
    }

    public class HelloWorld : JobBase
    {
        public HelloWorld()
        {
            var a = 1;
        }

        public override void Run(JobContext context)
        {
            //Logger.Info("Hello World");
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Hello World");
        }


    }
}
