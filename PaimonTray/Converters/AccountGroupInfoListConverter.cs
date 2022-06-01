using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using PaimonTray.Helpers;
using PaimonTray.Models;
using System;

namespace PaimonTray.Converters
{
    /// <summary>
    /// The account group info list converter.
    /// </summary>
    internal class AccountGroupInfoListConverter : IValueConverter
    {
        #region Constants

        #endregion Constants

        /// <summary>
        /// The avatar parameter.
        /// </summary>
        private const string ParameterAvatar = "avatar";

        /// <summary>
        /// The account's nickname parameter.
        /// </summary>
        private const string ParameterNicknameAccount = "nicknameAccount";

        /// <summary>
        /// The account's other info parameter.
        /// </summary>
        private const string ParameterOtherInfoAccount = "otherInfoAccount";

        /// <summary>
        /// The adding/updating status parameter.
        /// </summary>
        private const string ParameterStatusAddingUpdating = "statusAddingUpdating";

        /// <summary>
        /// The adding/updating status's explanation parameter.
        /// </summary>
        private const string ParameterStatusAddingUpdatingExplanation = "statusAddingUpdatingExplanation";

        /// <summary>
        /// The expired status parameter.
        /// </summary>
        private const string ParameterStatusExpired = "statusExpired";

        /// <summary>
        /// The expired status's explanation parameter.
        /// </summary>
        private const string ParameterStatusExpiredExplanation = "statusExpiredExplanation";

        /// <summary>
        /// The fail status parameter.
        /// </summary>
        private const string ParameterStatusFail = "statusFail";

        /// <summary>
        /// The fail status's explanation parameter.
        /// </summary>
        private const string ParameterStatusFailExplanation = "statusFailExplanation";

        /// <summary>
        /// The ready status parameter.
        /// </summary>
        private const string ParameterStatusReady = "statusReady";

        /// <summary>
        /// The ready status's explanation parameter.
        /// </summary>
        private const string ParameterStatusReadyExplanation = "statusReadyExplanation";

        #region Methods

        /// <summary>
        /// Modify the source data before passing it to the target for display in the UI.
        /// Reference: https://docs.microsoft.com/en-us/windows/winui/api/microsoft.ui.xaml.data.ivalueconverter.convert?view=winui-3.0
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null) return null;

            if (value is not GroupInfoList { Count: > 0 } groupInfoList) return null;

            if (groupInfoList[0] is not AccountCharacter accountCharacter) return null;

            var app = Application.Current as App;
            var resourceLoader = app?.SettingsH.ResLoader;

            return parameter switch
            {
                ParameterAvatar => app?.AccountsH.GetAvatarUri(accountCharacter.Key),
                ParameterNicknameAccount => accountCharacter.NicknameAccount,
                ParameterOtherInfoAccount => $"{accountCharacter.Server} | {accountCharacter.UidAccount}",
                ParameterStatusAddingUpdating => accountCharacter.Status is AccountsHelper.TagStatusAdding
                    or AccountsHelper.TagStatusUpdating
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                ParameterStatusAddingUpdatingExplanation => resourceLoader?.GetString(
                    "StatusAccountAddingUpdatingExplanation"),
                ParameterStatusExpired => accountCharacter.Status is AccountsHelper.TagStatusExpired
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                ParameterStatusExpiredExplanation => resourceLoader?.GetString("StatusAccountExpiredExplanation"),
                ParameterStatusFail => accountCharacter.Status is AccountsHelper.TagStatusFail
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                ParameterStatusFailExplanation => resourceLoader?.GetString("StatusAccountFailExplanation"),
                ParameterStatusReady => accountCharacter.Status is AccountsHelper.TagStatusReady
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                ParameterStatusReadyExplanation => resourceLoader?.GetString("StatusAccountReadyExplanation"),
                _ => null
            };
        } // end method Convert

        /// <summary>
        /// Modify the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// Reference: https://docs.microsoft.com/en-us/windows/winui/api/microsoft.ui.xaml.data.ivalueconverter.convertback?view=winui-3.0
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        /// <exception cref="NotImplementedException">If you don't use a converter for TwoWay bindings, it's acceptable to leave ConvertBack unimplemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        } // end method ConvertBack

        #endregion Methods
    } // end class AccountGroupInfoListConverter
} // end namespace PaimonTray.Converters