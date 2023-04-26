using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSettings : ContentPage
    {
        public Page pushedPage = null;
        private Settings settings = null;

        public EditSettings()
        {
            InitializeComponent();
            Version.Text = App.PlatformProxy?.GetVersionNumber + " (" + App.PlatformProxy?.GetBuildNumber + ") " + MainPage.GetBuildDateTime()?.ToString("yyyy.MM.dd HH:mm:ss");
        }

        public void UpdateView()
        {
            pushedPage = null;
            settings = MainPage.GetSettings?.Invoke()?.Clone;
            SaveChangesImmediately.IsChecked = settings.SaveChangesImmediately;
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            settings.SaveChangesImmediately = SaveChangesImmediately.IsChecked;
            MainPage.SetSettings?.Invoke(settings);
            Navigation.PopAsync();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            Navigation.PopAsync();
        }
    }
}