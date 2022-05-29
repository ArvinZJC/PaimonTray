using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PaimonTray.Controls
{
    public sealed partial class AccountGroupHeader
    {
        #region Fields

        /// <summary>
        /// The key property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(nameof(Key), typeof(string),
            typeof(AccountGroupHeader), new PropertyMetadata(null));

        #endregion Fields

        #region Properties

        /// <summary>
        /// The key.
        /// </summary>
        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        } // end property Key

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the account group header.
        /// </summary>
        public AccountGroupHeader()
        {
            InitializeComponent();
            UpdateUiText();
        } // end constructor AccountGroupHeader

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = (Application.Current as App)?.SettingsH.ResLoader;

            ToolTipService.SetToolTip(FontIconError, resourceLoader?.GetString("StatusAccountFailExplanation"));
            ToolTipService.SetToolTip(FontIconInformational, resourceLoader?.GetString("StatusAccountAddingUpdatingExplanation"));
            ToolTipService.SetToolTip(FontIconSuccess, resourceLoader?.GetString("StatusAccountReadyExplanation"));
            ToolTipService.SetToolTip(FontIconWarning, resourceLoader?.GetString("StatusAccountExpiredExplanation"));
        } // end method UpdateUiText

        #endregion Methods
    } // end class AccountGroupHeader
} // end namespace PaimonTray.Controls