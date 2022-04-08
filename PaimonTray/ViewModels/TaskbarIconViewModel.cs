using Microsoft.UI.Xaml.Input;
using PaimonTray.Helpers;
using System.Windows.Input;

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
#pragma warning disable CA1822 // Mark members as static
        public ICommand ExitAppCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand command = new();
                return command;
            } // end get
        } // end property ExitAppCommand

        /// <summary>
        /// The command to show the main window.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static
        public ICommand ShowMainWindowCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand command = new();

                command.ExecuteRequested += (_, e) =>
                {
                    if (e.Parameter is MainWindowHelper mainWindowHelper)
                        mainWindowHelper.Show();
                };
                return command;
            } // end get
        } // end property ShowMainWindowCommand
    } // end class TaskbarIconViewModel
} // end namespace PaimonTray.ViewModels