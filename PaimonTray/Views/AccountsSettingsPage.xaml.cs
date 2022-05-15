using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using PaimonTray.Helpers;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The accounts settings page.
    /// </summary>
    public sealed partial class AccountsSettingsPage
    {
        #region Fields

        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the accounts settings page.
        /// </summary>
        public AccountsSettingsPage()
        {
            _propertySetSettings = ApplicationData.Current.LocalSettings.Containers[SettingsHelper.ContainerKeySettings]
                .Values;
            InitializeComponent();
            ShowLoginAlternativeAlwaysSelection();
            ShowServerDefaultSelection();
            UpdateUiText();
        } // end constructor AccountsSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Reload the page for adding an account if necessary.
        /// </summary>
        private static void ReloadAddAccountPage()
        {
            var mainWindow = WindowsHelper.ShowMainWindow();
            var navigationViewBody = mainWindow.NavigationViewBody;

            if (navigationViewBody.SelectedItem == navigationViewBody.MenuItems.Last())
                mainWindow.FrameBody.Navigate(typeof(AddAccountPage), null, new EntranceNavigationTransitionInfo());
        } // end method ReloadAddAccountPage

        /// <summary>
        /// Show the selection for always using the alternative login method.
        /// </summary>
        private void ShowLoginAlternativeAlwaysSelection()
        {
            ToggleSwitchLoginAlternativeAlways.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways];
        } // end method ShowLoginAlternativeAlwaysSelection

        /// <summary>
        /// Show the selection for the default server.
        /// </summary>
        private void ShowServerDefaultSelection()
        {
            RadioButtonsServerDefault.SelectedItem =
                _propertySetSettings[SettingsHelper.KeyServerDefault] as string == AccountsHelper.TagServerCn
                    ? RadioButtonServerCn
                    : RadioButtonServerGlobal;
        } // end method ShowServerDefaultExplanation

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            RadioButtonServerCn.Content = resourceLoader.GetString("ServerCn");
            RadioButtonServerGlobal.Content = resourceLoader.GetString("ServerGlobal");
            TextBlockAccountsManagement.Text = resourceLoader.GetString("AccountsManagement");
            TextBlockLoginAlternativeAlways.Text = resourceLoader.GetString("LoginAlternativeAlways");
            TextBlockServerDefault.Text = resourceLoader.GetString("ServerDefault");
            TextBlockServerDefaultExplanation.Text = resourceLoader.GetString("ServerDefaultExplanation");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the selection changed event of the radio buttons for the default server.
        private void RadioButtonsServerDefault_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var radioButtonsServerDefaultSelectedItem = RadioButtonsServerDefault.SelectedItem as RadioButton;

            if (radioButtonsServerDefaultSelectedItem == null) return;

            var radioButtonsServerDefaultSelectedItemTag = radioButtonsServerDefaultSelectedItem.Tag as string;

            _propertySetSettings[SettingsHelper.KeyServerDefault] = radioButtonsServerDefaultSelectedItemTag;

            TextBlockServerDefaultSelection.Text = radioButtonsServerDefaultSelectedItem.Content as string;
        } // end method RadioButtonsServerDefault_OnSelectionChanged

        // Handle the toggled event of the toggle switch of the setting for always using the alternative login method.
        private void ToggleSwitchLoginAlternativeAlways_OnToggled(object sender, RoutedEventArgs e)
        {
            // It is required to reload the page for adding an account if necessary.
            if ((bool)_propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] ==
                ToggleSwitchLoginAlternativeAlways.IsOn) return;

            _propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] = ToggleSwitchLoginAlternativeAlways.IsOn;
            ReloadAddAccountPage();
        } // end method ToggleSwitchLoginAlternativeAlways_OnToggled

        #endregion Event Handlers
    } // end class AccountsSettingsPage
} // end namespace PaimonTray.Views