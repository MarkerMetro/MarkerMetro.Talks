using System.Collections.Generic;
using Windows.Storage;
using XboxMediaRemote.App.ViewModels;

namespace XboxMediaRemote.App.Services
{
    public interface IPlayHistoryService
    {
        void Add(StorageFileViewModel fileViewModel);
        List<PlayHistoryItem> GetPlayHistory();
        void SetPlayHistory(List<PlayHistoryItem> history);
    }
}