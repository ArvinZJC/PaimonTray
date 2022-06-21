using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Initialise the main window view model.
        /// </summary>
        public MainWindowViewModel()
        {
            _navigationViewPaneDisplayMode =
                (Application.Current as App)?.SettingsH.DecideMainWindowNavigationViewPaneDisplayMode() ??
                SettingsHelper.DefaultMainWindowNavigationViewPaneDisplayMode;
        } // end constructor MainWindowViewModel

        #endregion Constructors

        #region Events

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Fields

        /// <summary>
        /// The navigation view's pane display mode.
        /// </summary>
        private NavigationViewPaneDisplayMode _navigationViewPaneDisplayMode;

        /// <summary>
        /// The property name for the navigation view's pane display mode.
        /// </summary>
        public static readonly string PropertyNameNavViewPaneDisplayMode = nameof(NavViewPaneDisplayMode);

        #endregion Fields

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
    } // end class MainWindowViewModel
} // end namespace PaimonTray.ViewModels