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
            UpdateView();
        }

        public void UpdateView()
        {
            CharacterName.Text = string.Empty;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            var character = new CharacterSheet();
            character.Init();
            character.name = CharacterName.Text;
            CharacterSheetStorage.Instance.SaveCharacter(character);
            Navigation.PopAsync();
        }
    }
}