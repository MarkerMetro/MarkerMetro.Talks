using System;

namespace XboxMediaRemote.App.Services
{
    public class PlayHistoryItem
    {
        public string Token
        {
            get; set;
        }

        public DateTime PlayedOn
        {
            get; set;
        }
    }
}
