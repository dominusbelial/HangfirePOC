using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.Jobs
{
    public interface IMyJob
    {
        Task GetFiles();
    }

    public class MyJob : IMyJob
    {
        public MyJob()
        {

        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await GetFiles();
        }

        public async Task GetFiles()
        {
            Console.WriteLine("The current directory is {0}", Directory.GetCurrentDirectory());
            DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo f in files)
            {
                Console.WriteLine(f.FullName);
            }

            //Make your background methods reentrant
            //Reentrancy means that a method can be interrupted in the middle of its execution 
            //and then safely called again. The interruption can be caused by many different things (i.e. exceptions, server shut-down), 
            //and Hangfire will attempt to retry processing many times.

            //needs to be handled properly!
            await Task.Delay(0);
        }
    }
}
