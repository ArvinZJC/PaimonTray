using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace PaimonTray.Views
{
    /// <summary>
    /// The game data page.
    /// </summary>
    public sealed partial class GameDataPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the game data page.
        /// </summary>
        public GameDataPage()
        {
            InitializeComponent();
        } // end constructor GameDataPage

        #endregion Constructors

        #region Event Handlers

        /// <summary>
        /// Invoked when the <see cref="GameDataPage"/> is loaded and becomes the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="args">Details about the pending navigation that will load the current <see cref="GameDataPage"/>.</param>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var parameter = (KeyValuePair<string, string>)args.Parameter;

            TextBlockTest.Text = $"{parameter.Key}-{parameter.Value}";

            base.OnNavigatedTo(args);
        } // end method OnNavigatedTo

        #endregion Event Handlers
    } // end class GameDataPage
} // end namespace PaimonTray.Views