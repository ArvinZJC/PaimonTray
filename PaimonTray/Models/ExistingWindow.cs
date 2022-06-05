using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

namespace PaimonTray.Models
{
    /// <summary>
    /// The existing window model.
    /// </summary>
    public class ExistingWindow
    {
        #region Properties

        /// <summary>
        /// The background Acrylic controller.
        /// </summary>
        public DesktopAcrylicController DesktopAcrylicC { get; set; }

        /// <summary>
        /// The Mica controller.
        /// </summary>
        public MicaController MicaC { get; set; }

        /// <summary>
        /// The system backdrop configuration.
        /// </summary>
        public SystemBackdropConfiguration SystemBackdropConfig { get; set; }

        /// <summary>
        /// The window.
        /// </summary>
        public Window Win { get; set; }

        #endregion Properties
    } // end class ExistingWindow
} // end namespace PaimonTray.Models