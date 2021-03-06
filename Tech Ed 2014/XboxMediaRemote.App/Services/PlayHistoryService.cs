﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Newtonsoft.Json;
using XboxMediaRemote.App.ViewModels;
using XboxMediaRemote.Core.Extensions;

namespace XboxMediaRemote.App.Services
{
    public class PlayHistoryService : IPlayHistoryService
    {
        public void Add(StorageFileViewModel fileViewModel)
        {
            var token = StorageApplicationPermissions.FutureAccessList.Add(fileViewModel.File);

            var item = new PlayHistoryItem { Token = token, PlayedOn = DateTime.Now };

            var history = GetPlayHistory();

            if (history.ContainsKey(fileViewModel.MediaType))
            {
                history[fileViewModel.MediaType].Add(item);
            }
            else
            {
                history[fileViewModel.MediaType] = new List<PlayHistoryItem> { item };
            }

            SetPlayHistory(history);
        }

        public Dictionary<MediaType, List<PlayHistoryItem>> GetPlayHistory()
        {
            var json = ApplicationData.Current.LocalSettings.Values["PlayHistory"] as string;

            if (String.IsNullOrEmpty(json))
                return new Dictionary<MediaType, List<PlayHistoryItem>>();

            return JsonConvert.DeserializeObject<Dictionary<MediaType, List<PlayHistoryItem>>>(json);
        }

        public void SetPlayHistory(Dictionary<MediaType, List<PlayHistoryItem>> history)
        {
            var json = JsonConvert.SerializeObject(history);

            ApplicationData.Current.LocalSettings.Values["PlayHistory"] = json;
        }

        public async Task MigrateToDictionaryAsync()
        {
            var listJson = ApplicationData.Current.LocalSettings.Values["PlayHistory"] as string;

            if (String.IsNullOrEmpty(listJson))
                return;

            var historyList = JsonConvert.DeserializeObject<List<PlayHistoryItem>>(listJson);

            var files = await historyList.SelectAsync(async i => new
            {
                Item = i,
                File = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(i.Token)
            });

            var historyDictionary = files
                .GroupBy(f => StorageFileViewModel.DetermineMediaType(f.File.ContentType))
                .ToDictionary(g => g.Key, g => g.Select(i => i.Item).ToList());

            var dictionaryJson = JsonConvert.SerializeObject(historyDictionary);

            ApplicationData.Current.LocalSettings.Values["PlayHistory"] = dictionaryJson;
        }
    }
}
