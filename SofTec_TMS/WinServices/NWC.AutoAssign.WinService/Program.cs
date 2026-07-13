using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NWC.AutoAssign.WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //For Deploy
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AutoAssign()
            };
            ServiceBase.Run(ServicesToRun);

            ////For Debug Only
            //var ser = new AutoAssign();
            //ser.Start();
        }
    }
}
