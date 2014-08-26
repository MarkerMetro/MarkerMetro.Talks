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

            history.Add(fileViewModel.MediaType, item);

            SetPlayHistory(history);
        }

        public Dictionary<MediaType, PlayHistoryItem> GetPlayHistory()
        {
            var json = ApplicationData.Current.LocalSettings.Values["PlayHistory"] as string;

            if (String.IsNullOrEmpty(json))
                return new Dictionary<MediaType, PlayHistoryItem>();

            return JsonConvert.DeserializeObject<Dictionary<MediaType, PlayHistoryItem>>(json);
        }

        public void SetPlayHistory(Dictionary<MediaType, PlayHistoryItem> history)
        {
            var json = JsonConvert.SerializeObject(history);

            ApplicationData.Current.LocalSettings.Values["PlayHistory"] = json;
        }
    }
}
