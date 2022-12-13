using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using WinRT;
using AppInstance = Microsoft.Windows.AppLifecycle.AppInstance;

namespace PaimonTray
{
    internal static partial class Program
    {
        /// <summary>
        /// The main entry point for the app.
        /// </summary>
        [STAThread]
        static void
            Main() // Note: STAThread is required for the app to run in the background. The error of defining more than 1 entry point can be ignored.
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!DecideRedirection())
                Application.Start(p =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                    _ = new App();
                });
        } // end Main

        #region Methods

        /// <summary>
        /// Wait for specified objects to be signalled or for a specified timeout period to elapse.
        /// Reference: https://docs.microsoft.com/en-us/windows/win32/api/combaseapi/nf-combaseapi-cowaitformultipleobjects
        /// </summary>
        /// <param name="dwFlags">A flag controlling whether call/window message re-entrance is enabled from this wait.</param>
        /// <param name="dwTimeout">The timeout in milliseconds of the wait.</param>
        /// <param name="cHandles">The length of the <see cref="pHandles"/> array.</param>
        /// <param name="pHandles">An array of handles to await-able kernel objects.</param>
        /// <param name="lpdwindex">The index of the handle that satisfied the wait.</param>
        /// <returns>The return code.</returns>
        [LibraryImport("ole32.dll")]
        private static partial uint CoWaitForMultipleObjects(uint dwFlags, uint dwTimeout, ulong cHandles,
            nint[] pHandles, out uint lpdwindex);

        /// <summary>
        /// Create/Open a named or unnamed event object.
        /// Reference: https://docs.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-createeventa
        /// </summary>
        /// <param name="lpEventAttributes">A pointer to a SECURITY_ATTRIBUTES structure.</param>
        /// <param name="bManualReset">A flag indicating if the function creates a manual-reset event object.</param>
        /// <param name="bInitialState">A flag indicating if the initial state of the event object is signalled.</param>
        /// <param name="lpName">The name of the event object.</param>
        /// <returns>A handle to the event object, or <c>null</c> if the function fails.</returns>
        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        private static partial nint CreateEventA(nint lpEventAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool bManualReset, [MarshalAs(UnmanagedType.Bool)] bool bInitialState,
            string lpName);

        /// <summary>
        /// Decide if the app should be redirected to the new app instance.
        /// </summary>
        /// <returns>A flag indicating if the app should be redirected to the new app instance.</returns>
        private static bool DecideRedirection()
        {
            var appInstance = AppInstance.FindOrRegisterForKey(Package.Current.Id.FamilyName);

            if (appInstance.IsCurrent) return false;

            RedirectActivation(AppInstance.GetCurrent().GetActivatedEventArgs(), appInstance);
            return true;
        } // end method DecideRedirection

        /// <summary>
        /// Redirect the activation on another thread, and use a non-blocking wait method to wait for the redirection to complete.
        /// </summary>
        /// <param name="args">The app activation arguments.</param>
        /// <param name="appInstance">The app instance.</param>
        private static void RedirectActivation(AppActivationArguments args, AppInstance appInstance)
        {
            var redirectEventHandle = CreateEventA(nint.Zero, true, false, null);

            Task.Run(() =>
            {
                appInstance.RedirectActivationToAsync(args).AsTask().Wait();
                _ = SetEvent(redirectEventHandle);
            });
            _ = CoWaitForMultipleObjects(0, 0xFFFFFFFF, 1, new[] { redirectEventHandle }, out _);
        } // end method RedirectActivation

        /// <summary>
        /// Set the specified event object to the signalled state.
        /// Reference: https://docs.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-setevent
        /// </summary>
        /// <param name="hEvent">A handle to the event object.</param>
        /// <returns>A flag indicating if the function succeeds.</returns>
        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetEvent(nint hEvent);

        #endregion Methods
    } // end class Program
} // end namespace PaimonTray