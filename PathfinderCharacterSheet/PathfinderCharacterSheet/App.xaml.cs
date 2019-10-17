using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PathfinderCharacterSheet
{
    public partial class App : Application
    {
        public static IPlatformProxy PlatformProxy { get; set; }

        public App()
        {
            InitializeComponent();
            var mp = new MainPage();
            var np = new NavigationPage(mp);
            np.Popped += OnNavigationPopped;
            np.PoppedToRoot += OnNavigationPopped;
            MainPage = np;
        }

        void OnNavigationPopped(object s, NavigationEventArgs e)
        {
            var uv = (MainPage as NavigationPage).CurrentPage as ISheetView;
            if (uv != null)
                uv.UpdateView();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
