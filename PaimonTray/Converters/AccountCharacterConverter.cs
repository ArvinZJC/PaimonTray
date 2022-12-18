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
        #region Constants

        /// <summary>
        /// The parameter for the flag indicating if the account has any character.
        /// </summary>
        private const string ParameterHasCharacter = "hasCharacter";

        /// <summary>
        /// The character's nickname parameter.
        /// </summary>
        public const string ParameterNicknameCharacter = "nicknameCharacter";

        /// <summary>
        /// The character's other info parameter.
        /// </summary>
        public const string ParameterOtherInfoCharacter = "otherInfoCharacter";

        /// <summary>
        /// The character's other info visibility parameter.
        /// </summary>
        private const string ParameterOtherInfoVisibilityCharacter = "otherInfoVisibilityCharacter";

        #endregion Constants

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
            var isForVisibility = targetType == typeof(Visibility);

            if (parameter is null) return isForVisibility ? Visibility.Collapsed : null;

            if (value is not AccountCharacter accountCharacter) return isForVisibility ? Visibility.Collapsed : null;

            var parameterValue = parameter as string;

            return parameterValue switch
            {
                ParameterNicknameCharacter => accountCharacter.UidCharacter is null
                    ? (Application.Current as App)?.SettingsH.ResLoader.GetString("AccountNoCharacter")
                    : accountCharacter.NicknameCharacter,
                ParameterOtherInfoCharacter => accountCharacter.UidCharacter is null
                    ? null
                    : $"{accountCharacter.UidCharacter} | {accountCharacter.Region} | {accountCharacter.Level}",
                _ => parameterValue switch
                {
                    ParameterHasCharacter => accountCharacter.UidCharacter is not null,
                    ParameterOtherInfoVisibilityCharacter => accountCharacter.UidCharacter is null
                        ? Visibility.Collapsed
                        : Visibility.Visible,
                    _ => isForVisibility ? Visibility.Collapsed : null
                }
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