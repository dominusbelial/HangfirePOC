using Hangfire;
using System;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=hangfiredb;Database=Hangfire;User=sa;Password=Pass_abcd1234;");
            Thread.Sleep(20000);
            BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world! Console App Client"));
        }
    }
}
