using System.Collections.Generic;
using Windows.Storage;
using XboxMediaRemote.App.ViewModels;

namespace XboxMediaRemote.App.Services
{
    public interface IPlayHistoryService
    {
        void Add(StorageFileViewModel fileViewModel);
        Dictionary<MediaType, PlayHistoryItem> GetPlayHistory();
        void SetPlayHistory(Dictionary<MediaType, PlayHistoryItem> history);
    }
}