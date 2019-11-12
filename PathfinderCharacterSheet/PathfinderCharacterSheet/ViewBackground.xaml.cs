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
	public partial class ViewBackground : ContentPage, ISheetView
	{
        private Page pushedPage = null;

		public ViewBackground ()
		{
			InitializeComponent ();
            MainPage.AddTapHandler(Background, Background_DoubleTapped, 2);
       }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            CharacterName.Text = sheet.Name;
            Alignment.Text = sheet.Alignment.ToString();
            Experience.Text = sheet.experience.GetTotal(sheet).ToString();
            NextLevel.Text = sheet.nextLevelExperience.GetTotal(sheet).ToString();
            Level.Text = sheet.LevelAsString;
            Deity.Text = sheet.deity;
            Homeland.Text = sheet.homeland;
            Race.Text = sheet.Race;
            Size.Text = sheet.size;
            Gender.Text = sheet.gender;
            Age.Text = sheet.age;
            CharacterHeight.Text = sheet.height;
            Weight.Text = sheet.weight;
            Hair.Text = sheet.hair;
            Eyes.Text = sheet.eyes;
            Biography.Text = sheet.biography;
        }

        private void Background_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = new EditBackground();
            Navigation.PushAsync(pushedPage);
        }

    }
}