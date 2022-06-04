using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// The property name for the navigation view's pane display mode.
        /// </summary>
        public const string PropertyNameNavViewPaneDisplayMode = nameof(NavViewPaneDisplayMode);

        #endregion Constants

        #region Fields

        /// <summary>
        /// The navigation view's pane display mode.
        /// </summary>
        private NavigationViewPaneDisplayMode _navigationViewPaneDisplayMode;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The navigation view's pane display mode.
        /// </summary>
        public NavigationViewPaneDisplayMode NavViewPaneDisplayMode
        {
            get => _navigationViewPaneDisplayMode;
            set
            {
                if (_navigationViewPaneDisplayMode == value) return;

                _navigationViewPaneDisplayMode = value;
                OnPropertyChanged();
            } // end set
        } // end property NavViewPaneDisplayMode

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the main window view model.
        /// </summary>
        public MainWindowViewModel()
        {
            _navigationViewPaneDisplayMode =
                ((App)Application.Current).SettingsH.DecideMainWindowNavigationViewPaneDisplayMode();
        } // end constructor MainWindowViewModel

        #endregion Constructors

        #region Events

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Methods

        /// <summary>
        /// Occur when the specific property is changed.
        /// </summary>
        /// <param name="propertyName">The name of the property for the event.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // end method OnPropertyChanged

        #endregion Methods
    } // end class MainWindowViewModel
} // end namespace PaimonTray.ViewModels