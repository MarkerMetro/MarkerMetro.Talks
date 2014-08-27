using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Caliburn.Micro;
using XboxMediaRemote.App.Services;
using XboxMediaRemote.Core.Extensions;

namespace XboxMediaRemote.App.ViewModels
{
    public class PlayHistoryViewModel : StorageListPageViewModelBase
    {
        private readonly IPlayHistoryService playHistoryService;

        public PlayHistoryViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IPlayHistoryService playHistoryService) 
            : base(navigationService, eventAggregator)
        {
            this.playHistoryService = playHistoryService;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            await BindPlayHistoryAsync();
        }

        private async Task BindPlayHistoryAsync()
        {
            using (Loading())
            {
                var history = playHistoryService.GetPlayHistory();

                var groups = await history.GroupBy(i => i.PlayedOn.Date)
                    .SelectAsync(async g => 
                        new StorageItemGroupViewModel(g.Key.ToString("D"), 
                            await g.SelectAsync(async f => new StorageFileViewModel(
                                await StorageApplicationPermissions.FutureAccessList.GetFileAsync(f.Token)))));

                GroupedStorageItems.AddRange(groups);
            }

            await LoadThumbnailsAsync();
        }
    }
}
