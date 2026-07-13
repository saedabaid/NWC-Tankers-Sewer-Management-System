using System;
using Topshelf;
namespace NWC.Sewer.WinService
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var rc = HostFactory.Run(configure =>
            {
                configure.Service<SewerServices>(service =>
                {
                    service.ConstructUsing(s => new SewerServices());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.
                configure.RunAsLocalSystem();
                configure.SetServiceName("Sewer_Services");
                configure.SetDisplayName("Sewer_Services");
                configure.SetDescription("This .Net windows service using Topshelf");
            });
        }
    }
}
