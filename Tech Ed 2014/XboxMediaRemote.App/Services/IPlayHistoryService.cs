using System.Collections.Generic;
using XboxMediaRemote.App.ViewModels;

namespace XboxMediaRemote.App.Services
{
    public interface IPlayHistoryService
    {
        void Add(StorageFileViewModel fileViewModel);
        Dictionary<MediaType, List<PlayHistoryItem>> GetPlayHistory();
        void SetPlayHistory(Dictionary<MediaType, List<PlayHistoryItem>> history);
    }
}