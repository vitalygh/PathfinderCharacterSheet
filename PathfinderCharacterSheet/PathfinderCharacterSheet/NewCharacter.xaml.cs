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
        public Page pushedPage = null;
		public NewCharacter()
		{
			InitializeComponent ();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            CharacterName.Text = string.Empty;
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
            var character = new CharacterSheet();
            character.Init();
            character.name = CharacterName.Text;
            CharacterSheetStorage.Instance.SaveCharacter(character);
            Navigation.PopAsync();
        }
    }
}