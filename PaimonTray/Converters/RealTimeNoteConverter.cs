using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using PaimonTray.Helpers;
using System;

namespace PaimonTray.Converters
{
    internal class RealTimeNoteConverter : IValueConverter
    {
        #region Constants

        /// <summary>
        /// The expedition's avatar side icon  parameter.
        /// </summary>
        private const string ParameterExpeditionAvatarSideIcon = "expeditionAvatarSideIcon";

        /// <summary>
        /// The expedition's unknown avatar side icon parameter.
        /// </summary>
        private const string ParameterExpeditionAvatarSideIconUnknown = "expeditionAvatarSideIconUnknown";

        /// <summary>
        /// The expedition's complete status parameter.
        /// </summary>
        private const string ParameterExpeditionStatusComplete = "expeditionStatusComplete";

        /// <summary>
        /// The expedition's incomplete status parameter.
        /// </summary>
        private const string ParameterExpeditionStatusIncomplete = "expeditionStatusIncomplete";

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

            return parameter switch
            {
                ParameterExpeditionAvatarSideIcon => value is null ? Visibility.Collapsed : Visibility.Visible,
                ParameterExpeditionAvatarSideIconUnknown => value is null ? Visibility.Visible : Visibility.Collapsed,
                ParameterExpeditionStatusComplete => value is AccountsHelper.ExpeditionStatusFinished
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                ParameterExpeditionStatusIncomplete => value is AccountsHelper.ExpeditionStatusOngoing
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                _ => isForVisibility ? Visibility.Collapsed : null
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
        /// <exception cref="NotImplementedException">If you do not use a converter for TwoWay bindings, it is acceptable to leave ConvertBack unimplemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        } // end method ConvertBack

        #endregion Methods
    } // end class RealTimeNoteConverter
} // end namespace PaimonTray.Converters