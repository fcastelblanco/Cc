﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Isn.Common.ExtensionMethods;
using Isn.Common.LogHelper;
using Isn.Ioc;
using Isn.Upt.Business.Implementations;

namespace Isn.Upt.Presentation.Configurator.Desktop
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public Mutex TheMutex { get; private set; }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            TheMutex = new Mutex(true, Assembly.GetEntryAssembly().GetName().Name, out var isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show("Ya se encuentra un " + Assembly.GetEntryAssembly().GetName().Name + " en marcha");
                Current.Shutdown();
                return;
            }

            if (!WindowsExtension.IsServiceInstalled(UpdaterService.UpdaterServiceName))
                WindowsExtension.InstallService(AppDomain.CurrentDomain.BaseDirectory +
                                                @"Ipm service\Isn.Upt.Service.exe");

            if (WindowsExtension.IsServiceInstalled(UpdaterService.UpdaterServiceName))
                WindowsExtension.StopService(UpdaterService.UpdaterServiceName, out var exception);

            Process[] proceses = null;
            proceses = Process.GetProcessesByName("Isn.Upt.Presentation.Notificator");
            foreach (var proces in proceses) proces.Kill();

            Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
            Log.Instance.Info("Ipm iniciado");
            IsnContainer.Build();
        }

        private static void CurrentOnDispatcherUnhandledException(object sender,
            DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            var currentException = dispatcherUnhandledExceptionEventArgs.Exception;
            try
            {
                if (!EventLog.SourceExists(Assembly.GetEntryAssembly().GetName().Name))
                    EventLog.CreateEventSource(Assembly.GetEntryAssembly().GetName().Name,
                        Assembly.GetEntryAssembly().GetName().Name);

                var eventLog = new EventLog
                {
                    Source = Assembly.GetEntryAssembly().GetName().Name
                };

                eventLog.WriteEntry(currentException.Message + Environment.NewLine + currentException.StackTrace,
                    EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }
    }
}