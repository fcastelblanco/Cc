using System.ServiceProcess;
using Cc.Ioc;

namespace Cc.Upt.Service
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new MainService()
            };
            ServiceBase.Run(servicesToRun);
            //new MainService().StartService();
        }
    }
}