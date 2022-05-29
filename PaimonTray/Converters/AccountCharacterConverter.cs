using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using PaimonTray.Models;
using System;

namespace PaimonTray.Converters
{
    /// <summary>
    /// The account's character converter.
    /// </summary>
    internal class AccountCharacterConverter : IValueConverter
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

            var accountCharacter = (AccountCharacter)value;

            if (accountCharacter == null) return null;

            var hasNoCharacterLinked = accountCharacter.CNickname is null && accountCharacter.CUid is null &&
                                       accountCharacter.Level is null && accountCharacter.Region is null;

            return parameter switch
            {
                "cNickname" => hasNoCharacterLinked
                    ? (Application.Current as App)?.SettingsH.ResLoader.GetString("AccountNoCharacter")
                    : accountCharacter.CNickname,
                "cOtherInfo" => hasNoCharacterLinked
                    ? null
                    : $"{accountCharacter.Region} | {accountCharacter.Level} | {accountCharacter.CUid}",
                "cOtherInfoVisibility" => hasNoCharacterLinked ? Visibility.Collapsed : Visibility.Visible,
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
    } // end class AccountCharacterConverter
} // end namespace PaimonTray.Converters