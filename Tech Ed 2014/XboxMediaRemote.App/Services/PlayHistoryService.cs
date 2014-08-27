using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Newtonsoft.Json;
using XboxMediaRemote.App.ViewModels;

namespace XboxMediaRemote.App.Services
{
    public class PlayHistoryService : IPlayHistoryService
    {
        public void Add(StorageFileViewModel fileViewModel)
        {
            var token = StorageApplicationPermissions.FutureAccessList.Add(fileViewModel.File);

            var item = new PlayHistoryItem { Token = token, PlayedOn = DateTime.Now };

            var history = GetPlayHistory();

            history.Add(item);

            SetPlayHistory(history);
        }

        public List<PlayHistoryItem> GetPlayHistory()
        {
            var json = ApplicationData.Current.LocalSettings.Values["PlayHistory"] as string;

            if (String.IsNullOrEmpty(json))
                return new List<PlayHistoryItem>();

            return JsonConvert.DeserializeObject<List<PlayHistoryItem>>(json);
        }

        public void SetPlayHistory(List<PlayHistoryItem> history)
        {
            var json = JsonConvert.SerializeObject(history);

            ApplicationData.Current.LocalSettings.Values["PlayHistory"] = json;
        }
    }
}
