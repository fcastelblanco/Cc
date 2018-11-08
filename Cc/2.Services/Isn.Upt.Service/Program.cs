using System.ServiceProcess;
using Isn.Ioc;

namespace Isn.Upt.Service
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            IsnContainer.Build();

            var servicesToRun = new ServiceBase[]
            {
                new MainService()
            };
            ServiceBase.Run(servicesToRun);
            //new MainService().StartService();
        }
    }
}