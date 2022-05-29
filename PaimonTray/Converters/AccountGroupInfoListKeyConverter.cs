using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using PaimonTray.Helpers;
using PaimonTray.Models;
using System;
using System.Text.Json;

namespace PaimonTray.Converters
{
    /// <summary>
    /// The account group info list key converter.
    /// </summary>
    internal class AccountGroupInfoListKeyConverter : IValueConverter
    {
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

            var groupInfoListKey = (string)value;

            if (groupInfoListKey == null) return null;

            var accountGroup = JsonSerializer.Deserialize<AccountCharacter>(groupInfoListKey);

            if (accountGroup == null) return null;

            return parameter switch
            {
                "aNickname" => accountGroup.ANickname,
                "avatar" => (Application.Current as App)?.AccountsH.GetAvatarUri(accountGroup.Key),
                "aOtherInfo" => $"{accountGroup.Server} | {accountGroup.AUid}",
                "statusAddingUpdating" => accountGroup.Status is AccountsHelper.TagStatusAdding
                    or AccountsHelper.TagStatusUpdating
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                "statusExpired" => accountGroup.Status is AccountsHelper.TagStatusExpired
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                "statusFail" => accountGroup.Status is AccountsHelper.TagStatusFail
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                "statusReady" => accountGroup.Status is AccountsHelper.TagStatusReady
                    ? Visibility.Visible
                    : Visibility.Collapsed,
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
    } // end class AccountGroupInfoListKeyConverter
} // end namespace PaimonTray.Converters