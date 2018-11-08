using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Isn.Common.LogHelper;
using Microsoft.Win32;

namespace Isn.Upt.Presentation.Notificator
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key?.DeleteValue(Application.ProductName, false);

                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key?.SetValue(Application.ProductName, Assembly.GetEntryAssembly().Location);
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            Application.Run(new MainForm());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var currentException = e.Exception;
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

                Log.Instance.Error(currentException);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var currentException = e.ExceptionObject as Exception;

                if (currentException == null)
                    return;

                if (!EventLog.SourceExists(Assembly.GetEntryAssembly().GetName().Name))
                    EventLog.CreateEventSource(Assembly.GetEntryAssembly().GetName().Name,
                        Assembly.GetEntryAssembly().GetName().Name);

                var eventLog = new EventLog
                {
                    Source = Assembly.GetEntryAssembly().GetName().Name
                };

                eventLog.WriteEntry(currentException.Message + Environment.NewLine + currentException.StackTrace,
                    EventLogEntryType.Error);

                Log.Instance.Error(currentException);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }
    }
}