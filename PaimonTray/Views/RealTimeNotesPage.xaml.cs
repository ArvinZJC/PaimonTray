using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;

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
            UpdateUiText();

            CollectionViewSourceCharacters.Source =
                (Application.Current as App)?.AccHelper.GetGroupedCharactersFromLocal();
        } // end constructor RealTimeNotesPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Invoked when the <see cref="RealTimeNotesPage"/> is loaded and becomes the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="args">Details about the pending navigation that will load the current <see cref="RealTimeNotesPage"/>.</param>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
        } // end method OnNavigatedTo

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            TextBlockTitle.Text = resourceLoader.GetString("RealTimeNotes");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the body stack panel's size changed event.
        private void StackPanelBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = StackPanelBody.ActualHeight;
            Width = StackPanelBody.ActualWidth;
        } // end method StackPanelBody_OnSizeChanged

        #endregion Event Handlers
    } // end class RealTimeNotesPage
} // end namespace PaimonTray.Views