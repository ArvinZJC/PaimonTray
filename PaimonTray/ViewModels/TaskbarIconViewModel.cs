using System.Windows.Input;
using Microsoft.UI.Xaml.Input;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The taskbar icon view model.
    /// </summary>
    internal class TaskbarIconViewModel
    {
        /// <summary>
        /// The command to exit the app.
        /// </summary>
        public ICommand ExitAppCommand
        {
            get
            {
                XamlUICommand command = new();
                return command;
            } // end get
        } // end property ExitAppCommand

        /// <summary>
        /// The command to show or hide the main window.
        /// </summary>
        public ICommand ToggleMainWindowVisibilityCommand
        {
            get
            {
                XamlUICommand command = new();

                command.ExecuteRequested += (sender, e) =>
                {
                    var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                    Microsoft.UI.Windowing.AppWindow appWindow =
                        Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                    if (appWindow != null)
                    {
                        appWindow.Title = "Test";
                    }
                };
                return command;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand
    } // end class TaskbarIconViewModel
} // end namespace PaimonTray.ViewModels
