using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace PaimonTray.Views
{
    /// <summary>
    /// The real-time notes page.
    /// </summary>
    public sealed partial class RealTimeNotesPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the real-time notes page.
        /// </summary>
        public RealTimeNotesPage()
        {
            InitializeComponent();
        } // end constructor RealTimeNotesPage

        #endregion Constructors

        #region Event Handlers

        /// <summary>
        /// Invoked when the <see cref="RealTimeNotesPage"/> is loaded and becomes the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="args">Details about the pending navigation that will load the current <see cref="RealTimeNotesPage"/>.</param>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var parameter = (KeyValuePair<string, string>)args.Parameter;

            TextBlockTest.Text = $"{parameter.Key}-{parameter.Value}";

            base.OnNavigatedTo(args);
        } // end method OnNavigatedTo

        #endregion Event Handlers
    } // end class RealTimeNotesPage
} // end namespace PaimonTray.Views