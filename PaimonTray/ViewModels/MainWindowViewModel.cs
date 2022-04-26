﻿using Microsoft.UI.Xaml.Controls;
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
        #region Fields

        /// <summary>
        /// The navigation view's pane display mode.
        /// </summary>
        private NavigationViewPaneDisplayMode _navigationViewPaneDisplayMode =
            SettingsHelper.DecideMainWindowNavigationViewPaneDisplayMode();

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
                NotifyPropertyChanged();
            } // end set
        } // end property NavViewPaneDisplayMode

        #endregion Properties

        #region Methods

        /// <summary>
        /// Notify the property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property for the event.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // end method NotifyPropertyChanged

        #endregion Methods
    } // end class MainWindowViewModel
} // end namespace PaimonTray.ViewModels