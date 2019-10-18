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
	public partial class EditIntModifier : ContentPage
	{
        private CharacterSheet.IntML modifiersList = null;
        private CharacterSheet.Modifier<int> modifier = null;

        public EditIntModifier()
		{
			InitializeComponent ();
		}

        public void Init(CharacterSheet.IntML modifiersList, CharacterSheet.Modifier<int> modifier)
        {
            this.modifiersList = modifiersList;
            this.modifier = modifier;
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            Delete.IsEnabled = modifier != null;
            if (modifier == null)
                return;
            IsActive.IsChecked = modifier.IsActive;
            Value.Text = modifier.Value.ToString();
            Name.Text = modifier.Name;
        }

        private void EditToView()
        {
            var anyChanged = false;
            if (modifier == null)
            {
                modifier = new CharacterSheet.Modifier<int>();
                modifiersList.Add(modifier);
                anyChanged = true;
            }
            anyChanged |= modifier.IsActive != IsActive.IsChecked;
            modifier.active = IsActive.IsChecked;
            anyChanged |= MainPage.StrToInt(Value.Text, ref modifier.value);
            anyChanged |= Name.Text != modifier.name;
            modifier.name = Name.Text;
            /*
            if (anyChanged)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
            */
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

        private void Delete_Clicked(object sender, EventArgs e)
        {
            modifiersList.Remove(modifier);
            //CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
            Navigation.PopAsync();
        }
    }
}