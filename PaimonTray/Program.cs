using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using PaimonTray.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using AppInstance = Microsoft.Windows.AppLifecycle.AppInstance;

namespace PaimonTray
{
    internal class Program
    {
        /// <summary>
        /// The main entry point for the app.
        /// </summary>
        [STAThread]
        static void
            Main() // Note: STAThread is required for the app to run in the background. The error of defining more than 1 entry point can be ignored.
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();

            if (!DecideRedirection())
                Application.Start(p =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                    _ = new App();
                });
        } // end Main

        #region Methods

        [DllImport("ole32.dll")]
        private static extern uint CoWaitForMultipleObjects(uint dwFlags, uint dwMilliseconds, ulong nHandles,
            IntPtr[] pHandles, out uint dwIndex);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState,
            string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hEvent);

        // Decide if the app should be redirected to the new app instance.
        private static bool DecideRedirection()
        {
            var appInstance = AppInstance.FindOrRegisterForKey(Package.Current.DisplayName);

            if (appInstance.IsCurrent)
            {
                appInstance.Activated += (_, _) => WindowsHelper.ShowMainWindow().Show();
                return false;
            } // end if

            RedirectActivation(AppInstance.GetCurrent().GetActivatedEventArgs(), appInstance);
            return true;
        } // end method DecideRedirection

        // Redirect the activation on another thread, and use a non-blocking wait method to wait for the redirection to complete.
        private static void RedirectActivation(AppActivationArguments args, AppInstance appInstance)
        {
            var redirectEventHandle = CreateEvent(IntPtr.Zero, true, false, null);

            Task.Run(() =>
            {
                appInstance.RedirectActivationToAsync(args).AsTask().Wait();
                SetEvent(redirectEventHandle);
            });
            _ = CoWaitForMultipleObjects(0, 0xFFFFFFFF, 1, new[] { redirectEventHandle }, out _);
        } // end method RedirectActivation

        #endregion Methods
    } // end class Program
} // end namespace PaimonTray