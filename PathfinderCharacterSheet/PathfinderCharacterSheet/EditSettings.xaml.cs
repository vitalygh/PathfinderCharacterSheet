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
            Version.Text = UIHelpers.GetBuildVersion();
        }

        public void UpdateView()
        {
            pushedPage = null;
            settings = UIMediator.GetSettings?.Invoke()?.Clone;
            SaveChangesImmediately.IsChecked = settings.SaveChangesImmediately;
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            settings.SaveChangesImmediately = SaveChangesImmediately.IsChecked;
            UIMediator.SetSettings?.Invoke(settings);
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