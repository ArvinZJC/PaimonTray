using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Models;

namespace PaimonTray.Controls
{
    public sealed partial class AccountGroupHeader
    {
        #region Fields

        /// <summary>
        /// The group info property.
        /// </summary>
        public static readonly DependencyProperty GroupInfoProperty = DependencyProperty.Register(nameof(GroupInfo),
            typeof(GroupInfoList), typeof(AccountGroupHeader), new PropertyMetadata(null));

        #endregion Fields

        #region Properties

        /// <summary>
        /// The group info.
        /// </summary>
        public GroupInfoList GroupInfo
        {
            get => GetValue(GroupInfoProperty) as GroupInfoList;
            set => SetValue(GroupInfoProperty, value);
        } // end property GroupInfo

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
            ToolTipService.SetToolTip(FontIconInformational,
                resourceLoader?.GetString("StatusAccountAddingUpdatingExplanation"));
            ToolTipService.SetToolTip(FontIconSuccess, resourceLoader?.GetString("StatusAccountReadyExplanation"));
            ToolTipService.SetToolTip(FontIconWarning, resourceLoader?.GetString("StatusAccountExpiredExplanation"));
        } // end method UpdateUiText

        #endregion Methods
    } // end class AccountGroupHeader
} // end namespace PaimonTray.Controls