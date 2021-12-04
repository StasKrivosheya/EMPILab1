using EMPILab1.ViewModels;
using EMPILab1.Pages;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;

namespace EMPILab1
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SecretConstants.SYNCFUSION_LICENSE_KEY);

            InitializeComponent();

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(Tasks12)}");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<Tasks12, Tasks12ViewModel>();
            containerRegistry.RegisterForNavigation<Tasks345, Tasks345ViewModel>();
        }
    }
}
