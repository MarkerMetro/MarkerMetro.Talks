using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Caliburn.Micro;
using XboxMediaRemote.App.Resources;
using XboxMediaRemote.App.Services;
using XboxMediaRemote.App.ViewModels;
using XboxMediaRemote.App.ViewModels.Settings;

namespace XboxMediaRemote.App
{
    public sealed partial class App
    {
        private WinRTContainer container;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            DisplayRootViewFor<MediaHubViewModel>();
        }

        protected override void Configure()
        {
            MarkedUp.AnalyticClient.Initialize("5809dd47-4d72-4c1e-b125-c7272bfc149d");

            container = new WinRTContainer();

            container
                .Instance(container);

            container
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IApplicationSettingsService, ApplicationSettingsService>();

            container
                .PerRequest<MediaHubViewModel>()
                .PerRequest<BrowseFolderViewModel>()
                .PerRequest<SearchResultsViewModel>()
                .PerRequest<AboutViewModel>()
                .PerRequest<PrivacyPolicyViewModel>();

            var settings = container.RegisterSettingsService();

            settings.RegisterFlyoutCommand<AboutViewModel>(Strings.SettingsAbout);
            settings.RegisterFlyoutCommand<PrivacyPolicyViewModel>(Strings.SettingsPrivacyPolicy);
        }

        protected override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);

            MarkedUp.AnalyticClient.LogLastChanceException(e);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }
    }
}
