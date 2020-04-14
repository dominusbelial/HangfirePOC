using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok( BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget Web Api!")) );
        }

        public IActionResult GetFiles()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = dirInfo.GetFiles();
            StringBuilder sb = new StringBuilder();
            foreach (FileInfo f in files)
            {
                sb.Append(f.FullName + Environment.NewLine);
            }
            return Ok(BackgroundJob.Enqueue(() => Console.WriteLine(sb.ToString())));
        }
    }
}