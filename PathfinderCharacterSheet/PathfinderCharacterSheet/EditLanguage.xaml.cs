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
	public partial class EditLanguage : ContentPage
    {
        private Page pushedPage = null;
        private int itemIndex = -1;
        private List<string> languages = null;

        public EditLanguage()
        {
            InitializeComponent();
        }

        public void InitEditor(List<string> languages, string language = null, int index = -1)
        {
            pushedPage = null;
            Language.Text = language;
            this.languages = languages;
            itemIndex = index;
            Delete.IsEnabled = index >= 0;
        }

        private void EditToView()
        {
            if (languages == null)
                return;
            var language = Language.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                if (language != languages[itemIndex])
                {
                    languages[itemIndex] = language;
                    hasChanges = true;
                }
            }
            else
            {
                languages.Add(language);
                hasChanges = true;
            }
            if (hasChanges)
                MainPage.OnCharacterSheetChanged?.Invoke();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            EditToView();
            Navigation.PopAsync();
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var language = itemIndex < 0 ? string.Empty : languages[itemIndex];
            if (!string.IsNullOrWhiteSpace(language))
                language = " \"" + language + "\"";
            bool allow = await DisplayAlert("Remove language" + language, "Are you sure?", "Yes", "No");
            if (allow)
            {
                languages.RemoveAt(itemIndex);
                await Navigation.PopAsync();
            }
        }
    }
}