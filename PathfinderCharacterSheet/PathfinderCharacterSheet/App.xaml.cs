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
            np.Popped += (s, e) =>
            {
                var page = (MainPage as NavigationPage).CurrentPage;
                var cst = page as CharacterSheetTabs;
                if (cst != null)
                    cst.UpdateFields();
                var ehp = page as EditHP;
                if (ehp != null)
                    ehp.ModifiersToEdit();
                if (page == mp)
                    mp.UpdateListView();
            };
            MainPage = np;
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
