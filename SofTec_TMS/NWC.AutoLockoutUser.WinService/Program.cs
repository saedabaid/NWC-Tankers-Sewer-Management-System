using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace NWC.AutoLockoutUser.WinService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var rc = HostFactory.Run(configure =>
            {
                configure.Service<LockoutUser>(service =>
                {
                    service.ConstructUsing(s => new LockoutUser());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.
                configure.RunAsLocalSystem();
                configure.SetServiceName("NWC.LockoutUser.Service");
                configure.SetDisplayName("NWC.LockoutUser.Service");
                configure.SetDescription("This .Net windows service using Topshelf");
            });
        }
    }
}
