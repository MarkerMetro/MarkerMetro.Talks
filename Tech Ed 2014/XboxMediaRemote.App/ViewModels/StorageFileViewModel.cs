using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using PropertyChanged;
using XboxMediaRemote.App.Resources;

namespace XboxMediaRemote.App.ViewModels
{
    [ImplementPropertyChanged]
    public class StorageFileViewModel : StorageItemViewModel
    {
        private readonly StorageFile file;

        public StorageFileViewModel(StorageFile file)
            : base(file)
        {
            this.file = file;

            MediaType = DetermineMediaType(file.ContentType);
        }

        public StorageFile File
        {
            get
            {
                return file;
            }
        }

        public MediaType MediaType
        {
            get; set;
        }

        public string Description
        {
            get
            {
                switch (MediaType)
                {
                    case MediaType.Picture:
                        return Strings.MediaTypeImage;
                    case MediaType.Video:
                        return Strings.MediaTypeVideo;
                    default:
                        return String.Empty;
                }
            }
        }

        public override async Task LoadThumbnailAsync()
        {
            try
            {
                using (var thumbnail = await File.GetThumbnailAsync(ThumbnailMode.ListView, 150, ThumbnailOptions.UseCurrentScale))
                {
                    if (thumbnail == null)
                        return;

                    ThumbnailImage = new BitmapImage();
                    ThumbnailImage.SetSource(thumbnail);
                }
            }
            catch
            {
                Debug.WriteLine("Unable to load thumbnail for {0}", File.Name);
            }
            
        }

        public static MediaType DetermineMediaType(string contentType)
        {
            if (contentType.StartsWith("image/"))
                return MediaType.Picture;
            
            if (contentType.StartsWith("video/"))
                return MediaType.Video;
            
            if (contentType.StartsWith("audio/"))
                return MediaType.Music;

            return MediaType.Unknown;
        }
    }
}
