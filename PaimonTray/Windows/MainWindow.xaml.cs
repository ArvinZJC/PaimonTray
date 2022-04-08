using PaimonTray.Helpers;

namespace PaimonTray.Windows
{
    /// <summary>
    /// The main window to show the retrieved Genshin data.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Constructors

        /// <summary>
        /// Initialise the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Activated += (_, _) =>
            {
                MainWindowHelper mainWindowHelper = new();

                TaskbarIconApp.DoubleClickCommandParameter = mainWindowHelper;
                TaskbarIconApp.LeftClickCommandParameter = mainWindowHelper;
            };
        } // end constructor MainWindow

        #endregion Constructors
    } // end class MainWindow
} // end namespace PaimonTray.Windows