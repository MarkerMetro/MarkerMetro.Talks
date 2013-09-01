using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using MetroTalks.Win81.PetesEats.Common;
using MetroTalks.Win81.PetesEats.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=??????
using MetroTalks.Win81.PetesEats.Services;
using MetroTalks.Win81.PetesEats.ViewModels;

namespace MetroTalks.Win81.PetesEats
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly MainHubViewModel _defaultViewModel = new MainHubViewModel();
        private readonly ObservableCollection<Eat> _mostRecentEats = new ObservableCollection<Eat>();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public MainHubViewModel DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);
            this._navigationHelper.LoadState += navigationHelper_LoadState;
            Window.Current.SizeChanged += (s, e) => UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            VisualStateManager.GoToState(this, ApplicationView.GetForCurrentView().Orientation.ToString(), false);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            await DefaultViewModel.BindEats();
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Eat)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void OnSearchBoxQueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs args)
        {
            
            if (string.IsNullOrEmpty(args.QueryText))
            {
                SearchResultsSection.Visibility = Visibility.Collapsed;
                DefaultViewModel.ClearSearch();
                HeroSection.Visibility = Visibility.Visible;
                PickOfTheWeekSection.Visibility = Visibility.Visible;
                MostRecentEatsSection.Visibility = Visibility.Visible;
                return;
            }
            
            if(await DefaultViewModel.DoInlineSearch(args.QueryText));
            {
                HeroSection.Visibility = Visibility.Collapsed;
                PickOfTheWeekSection.Visibility = Visibility.Collapsed;
                MostRecentEatsSection.Visibility = Visibility.Collapsed;
                SearchResultsSection.Visibility = Visibility.Visible;
            }
        }

        private void OnSearchBoxQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            //TODO: Search Query Submitted
        }

        private async void OnSearchBoxSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            return;
            if (string.IsNullOrEmpty(e.QueryText))
            {
                return;
            }
            var request = e.Request;
            var deferral = request.GetDeferral();
            try
            {
                var searchTask = DefaultViewModel.SearchForEats(e.QueryText);
                var results = await searchTask;

                if (!request.IsCanceled && searchTask.Status == TaskStatus.RanToCompletion)
                {
                    foreach (var result in results)
                    {
                        e.Request.SearchSuggestionCollection.AppendResultSuggestion(
                            result.Name, 
                            result.Description, 
                            result.Id.ToString(),
                            Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri(result.ThumbnailImage.Url, UriKind.Absolute)),
                            result.Name);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                deferral.Complete();
            }
        }

        private void OnSearchBoxResultSuggestionChosen(SearchBox sender, SearchBoxResultSuggestionChosenEventArgs args)
        {
            return;
            this.Frame.Navigate(typeof(ItemPage), int.Parse(args.Tag));
        }
    }
}
