using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;
using Windows.Storage.BulkAccess;
using MetroTalks.Win81.PetesEats.Annotations;
using MetroTalks.Win81.PetesEats.Services;

namespace MetroTalks.Win81.PetesEats.ViewModels
{
    public class MainHubViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Eat> _mostRecentEats = new ObservableCollection<Eat>();
        private readonly ObservableCollection<Eat> _searchResults = new ObservableCollection<Eat>();

        private readonly PetesEatsService _peteEatsService;
        private Eat _pickOfTheWeek;
        private SearchSuggestionCollection _searchSuggestions;
        

        public MainHubViewModel()
        {
            _peteEatsService = new PetesEatsService();
        }

        public ObservableCollection<Eat> MostRecentEats
        {
            get { return _mostRecentEats; }
        }

        public async Task BindEats()
        {

            PickOfTheWeek = await _peteEatsService.GetPickOfTheWeek();

            var mostRecent = await _peteEatsService.GetRecentEats();
            MostRecentEats.Clear();
            foreach (var eat in mostRecent)
            {
                MostRecentEats.Add(eat);
            }

            
        }

        public Eat PickOfTheWeek
        {
            get { return _pickOfTheWeek; }
            set
            {
                if (Equals(value, _pickOfTheWeek)) return;
                _pickOfTheWeek = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Eat> SearchResults
        {
            get { return _searchResults; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private CancellationTokenSource _cts;

        public async Task<IEnumerable<Eat>> SearchForEats(string queryText)
        {

            if (_cts != null)
            {
                _cts.Cancel();
            }

            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                _cts = cts;

                var searchTask = DoSearch(queryText);

                var searchResults = await searchTask;


                if (!cts.IsCancellationRequested)
                {
                    return searchResults;
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception)
            {
                
            }
            return new Eat[0];
        }


        public async Task<bool> DoInlineSearch(string searchTerm)
        {

            if (_cts != null)
            {
                _cts.Cancel();
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            _cts = cts;
            
            try
            {

                var results = await DoSearch(searchTerm);

                if(cts.Token.IsCancellationRequested)
                {
                    return false;
                }

                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(result);
                }
                
                return true;

            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception)
            {
                SearchResults.Clear();
            }
            return false;
        }


        private async Task<IEnumerable<Eat>> DoSearch(string searchTerm)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            return await _peteEatsService.SearchEats(searchTerm);
        }

        public void ClearSearch()
        {
            SearchResults.Clear();
        }
    }
}