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
	public partial class NewCharacter : ContentPage
	{
		public NewCharacter()
		{
			InitializeComponent ();

            if (CharacterSheetStorage.Instance.selectedCharacter != null)
                CharacterName.Text = CharacterSheetStorage.Instance.selectedCharacter.Name;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            var character = CharacterSheetStorage.Instance.selectedCharacter;
            var selected = character != null;
            if (character == null)
                character = new CharacterSheet();
            if (character.Name != CharacterName.Text)
                CharacterSheetStorage.Instance.DeleteCharacter(character);
            character.name = CharacterName.Text;
            CharacterSheetStorage.Instance.SaveCharacter(character);
            CharacterSheetStorage.Instance.selectedCharacter = character;
            Navigation.PopAsync();
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            CharacterSheetStorage.Instance.DeleteCharacter(CharacterSheetStorage.Instance.selectedCharacter);
            Navigation.PopAsync();
        }
    }
}