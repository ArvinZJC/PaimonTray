using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using PaimonTray.Helpers;
using System;
using System.Collections.Generic;

namespace PaimonTray.Converters
{
    /// <summary>
    /// The group info list key converter.
    /// </summary>
    internal class GroupInfoListKeyConverter : IValueConverter
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
            var groupInfoListKey = (Dictionary<string, object>)value;

            if (parameter == null) return null;

            return parameter switch
            {
                "aNickname" => groupInfoListKey[AccountsHelper.KeyNickname],
                "avatar" => (Application.Current as App)?.AccountsH.GetAvatarUri(
                    groupInfoListKey[AccountsHelper.KeyAccount] as string),
                "aOtherInfo" =>
                    $"{groupInfoListKey[AccountsHelper.KeyServer]} | {groupInfoListKey[AccountsHelper.KeyUid]}",
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
    } // end class GroupInfoListKeyConverter
} // end namespace PaimonTray.Converters