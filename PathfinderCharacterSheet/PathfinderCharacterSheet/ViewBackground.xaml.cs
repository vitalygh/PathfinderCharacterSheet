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
		}

        public void UpdateView()
        {
            pushedPage = null;
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            CharacterName.Text = c.Name;
            Alignment.Text = c.alignment.ToString();
            Experience.Text = c.experience.ToString();
            NextLevel.Text = c.nextLevelExperience.ToString();
            Level.Text = c.LevelAsString;
            Deity.Text = c.deity;
            Homeland.Text = c.homeland;
            Race.Text = c.Race;
            Size.Text = c.size;
            Gender.Text = c.gender;
            Age.Text = c.age;
            CharacterHeight.Text = c.height;
            Weight.Text = c.weight;
            Hair.Text = c.hair;
            Eyes.Text = c.eyes;
            Biography.Text = c.biography;
        }

        private void Background_DoubleTapped(object sender, EventArgs e)
        {
            pushedPage = new EditBackground();
            Navigation.PushAsync(pushedPage);
        }

    }
}