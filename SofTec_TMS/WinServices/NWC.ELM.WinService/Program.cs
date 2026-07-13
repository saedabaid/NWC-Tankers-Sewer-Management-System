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
                configure.Service<ELMServices>(service =>
                {
                    service.ConstructUsing(s => new ELMServices());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.
                configure.RunAsLocalSystem();
                configure.SetServiceName("NWC.ELM.Services");
                configure.SetDisplayName("ELM Services Integration");
                configure.SetDescription("This ELM Services windows service using Topshelf");
            });
        }
    }
}
