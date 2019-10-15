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
	public partial class EditHP : ContentPage
	{
        private List<CharacterSheet.IntModifier> tempHPMods = null;

		public EditHP ()
		{
			InitializeComponent ();
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            tempHPMods = new List<CharacterSheet.IntModifier>();
            foreach (var mod in c.hp.tempHP)
                tempHPMods.Add(new CharacterSheet.IntModifier()
                {
                    active = mod.IsActive,
                    value = mod.Value,
                    name = mod.Name,
                });
            ViewToEdit();
        }

        public void ViewToEdit()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            MaxHP.Text = c.hp.maxHP.ToString();
            HP.Text = c.hp.hp.ToString();
            TempHPModifiers.ItemsSource = null;
            TempHPModifiers.ItemsSource = tempHPMods;
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var anyChanged = false;
            anyChanged |= MainPage.StrToInt(MaxHP.Text, ref c.hp.maxHP);
            anyChanged |= MainPage.StrToInt(HP.Text, ref c.hp.hp);
            if (!CharacterSheet.Compare(c.hp.tempHP, tempHPMods))
            {
                anyChanged = true;
                c.hp.tempHP = tempHPMods;
            }
            if (anyChanged)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private void TempHPModifierAddButton_Clicked(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var page = new EditIntModifier();
            page.Init(tempHPMods, null);
            Navigation.PushAsync(page);
        }

        private void TempHPModifiers_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var page = new EditIntModifier();
            page.Init(tempHPMods, e.Item as CharacterSheet.IntModifier);
            Navigation.PushAsync(page);
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
    }
}