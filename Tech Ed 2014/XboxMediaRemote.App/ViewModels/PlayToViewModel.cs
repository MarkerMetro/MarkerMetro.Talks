using System;
using System.Threading.Tasks;
using Windows.Media.PlayTo;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Caliburn.Micro;
using PropertyChanged;
using XboxMediaRemote.App.Events;
using XboxMediaRemote.App.Resources;

#if DEBUG
using CurrentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
#endif

namespace XboxMediaRemote.App.ViewModels
{
    [ImplementPropertyChanged]
    public class PlayToViewModel : ViewModelBase, IHandle<MediaSelectedEventArgs>
    {
        private readonly IEventAggregator eventAggregator;
        private PlayToManager playToManager;
        private PlayToConnection currentConnection;

        public PlayToViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

#if DEBUG
            var settings = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///resources/store/simulator.xml"));

            await CurrentApp.ReloadSimulatorAsync(settings);
#endif

            playToManager = PlayToManager.GetForCurrentView();

            playToManager.SourceRequested += OnSourceRequsted;
            playToManager.SourceSelected += OnSourceSelected;

            eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            playToManager.SourceRequested -= OnSourceRequsted;
            playToManager.SourceSelected -= OnSourceSelected;

            eventAggregator.Unsubscribe(this);
        }

        private void OnSourceRequsted(PlayToManager sender, PlayToSourceRequestedEventArgs args)
        {
            var deferral = args.SourceRequest.GetDeferral();

            Execute.OnUIThread(async () =>
                {
                    if (CurrentFile != null)
                    {
                        var source = await CreateSourceAsync();

                        if (source == null)
                            args.SourceRequest.DisplayErrorString(Strings.PlayToInvalidMediaType);
                        else
                        {
                            args.SourceRequest.SetSource(source);

                            CurrentConnection = source.Connection;
                        }
                    }
                    else
                    {
                        args.SourceRequest.DisplayErrorString(Strings.PlayToNoFile);
                    }

                    deferral.Complete();
                });
        }

        private void OnSourceSelected(PlayToManager sender, PlayToSourceSelectedEventArgs args)
        {
            Execute.OnUIThread(() =>
            {
                SourceName = args.FriendlyName;

                SourceIcon = new BitmapImage();
                SourceIcon.SetSource(args.Icon);    
            });
        }

        private async Task<PlayToSource> CreateSourceAsync()
        {
            switch (CurrentFile.MediaType)
            {
                case MediaType.Picture:

                    var imageStream = await CurrentFile.File.OpenReadAsync();

                    var imageSource = new BitmapImage();

                    await imageSource.SetSourceAsync(imageStream);

                    var image = new Image
                                {
                                    Source = imageSource
                                };

                    return image.PlayToSource;
                case MediaType.Video:

                    var videoStream = await CurrentFile.File.OpenReadAsync();

                    var mediaElement = new MediaElement
                    {
                        AutoPlay = false
                    };

                    mediaElement.SetSource(videoStream, CurrentFile.File.ContentType);

                    return mediaElement.PlayToSource;

                default:
                    return null;
            }
        }

        public async void Handle(MediaSelectedEventArgs message)
        {
            var duration = await GetDurationAsync(message, message.StorageFile.File);
            var canPlayFullDuration = await CanPlayFullDurationAsyc();

            if (!canPlayFullDuration && duration > TimeSpan.FromSeconds(30))
            {
                var dialog = new MessageDialog("Cannot play media longer than thirty seconds in duration in trial mode.", "Trial Exceeded");

                await dialog.ShowAsync();

                await CurrentApp.RequestAppPurchaseAsync(includeReceipt: false);

                return;
            }

            CurrentFile = message.StorageFile;

            ShowPlayToUI();
        }

        private static async Task<TimeSpan> GetDurationAsync(MediaSelectedEventArgs message, StorageFile file)
        {
            var duration = TimeSpan.Zero;

            switch (message.StorageFile.MediaType)
            {
                case MediaType.Video:

                    var videoProperties = await file.Properties.GetVideoPropertiesAsync();

                    duration = videoProperties.Duration;

                    break;
                case MediaType.Music:

                    var musicProperties = await file.Properties.GetMusicPropertiesAsync();

                    duration = musicProperties.Duration;

                    break;
            }

            return duration;
        }

        private Task<bool> CanPlayFullDurationAsyc()
        {
            return Task.FromResult(!CurrentApp.LicenseInformation.IsTrial);
        }

        public void ShowPlayToUI()
        {
            PlayToManager.ShowPlayToUI();
        }

        public StorageFileViewModel CurrentFile
        {
            get;
            set;
        }

        public PlayToConnection CurrentConnection
        {
            get
            {
                return currentConnection;
            }
            set
            {
                if (currentConnection == value)
                    return;

                if (currentConnection != null)
                {
                    currentConnection.StateChanged -= OnConnectionStateChanged;
                }

                currentConnection = value;

                if (currentConnection != null)
                {
                    currentConnection.StateChanged += OnConnectionStateChanged;
                }

                NotifyOfPropertyChange();
            }
        }

        private void OnConnectionStateChanged(PlayToConnection sender, PlayToConnectionStateChangedEventArgs args)
        {
            NotifyOfPropertyChange(() => CurrentConnection);
        }

        public string SourceName
        {
            get;
            set;
        }

        public BitmapImage SourceIcon
        {
            get;
            set;
        }
    }
}
