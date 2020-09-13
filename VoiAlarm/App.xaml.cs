using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace VoiAlarm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string appName = "VoiAlarm";

        private Mutex mutex = new Mutex(false, appName);

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show($"{appName} はすでに起動しています。", appName, MessageBoxButton.OK);
                mutex.Close();
                mutex = null;
                Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
                mutex = null;
            }

            DispatcherUnhandledException -= OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var message = e.Exception.Message + Environment.NewLine + e.Exception.StackTrace;
            MessageBox.Show(message, $"{appName} 未処理例外");
        }
    }
}
