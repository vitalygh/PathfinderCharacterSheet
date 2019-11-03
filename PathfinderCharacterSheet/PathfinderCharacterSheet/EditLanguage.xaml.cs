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
        private int itemIndex = -1;
        private List<string> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.languages;
                return null;
            }
        }

        public EditLanguage()
        {
            InitializeComponent();
        }

        public void InitEditor(string language = null, int index = -1)
        {
            if (!string.IsNullOrWhiteSpace(language))
                Language.Text = language;
            itemIndex = index;
            Delete.IsEnabled = index >= 0;
        }

        private void EditToView()
        {
            if (items == null)
                return;
            var language = Language.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                if (language != items[itemIndex])
                {
                    items[itemIndex] = language;
                    hasChanges = true;
                }
            }
            else
            {
                items.Add(language);
                hasChanges = true;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            EditToView();
            Navigation.PopAsync();
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var language = itemIndex < 0 ? string.Empty : items[itemIndex];
            if (!string.IsNullOrWhiteSpace(language))
                language = " \"" + language + "\"";
            bool allow = await DisplayAlert("Remove language" + language, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.RemoveAt(itemIndex);
                await Navigation.PopAsync();
            }
        }
    }
}