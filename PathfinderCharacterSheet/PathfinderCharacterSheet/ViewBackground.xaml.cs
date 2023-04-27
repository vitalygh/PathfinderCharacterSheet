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
            UIHelpers.AddTapHandler(CharacterBackground, Background_DoubleTapped, 2);
       }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            UpdateValue(CharacterName, sheet.Name);
            UpdateValue(Alignment, sheet.Alignment.ToString());
            UpdateValue(Experience, sheet.experience.GetValue(sheet).ToString());
            UpdateValue(NextLevel, sheet.nextLevelExperience.GetValue(sheet).ToString());
            UpdateValue(Level, sheet.LevelAsString);
            UpdateValue(Deity, sheet.deity);
            UpdateValue(Homeland, sheet.homeland);
            UpdateValue(Race, sheet.Race);
            UpdateValue(Size, sheet.size);
            UpdateValue(Gender, sheet.gender);
            UpdateValue(Age, sheet.age);
            UpdateValue(CharacterHeight, sheet.height);
            UpdateValue(Weight, sheet.weight);
            UpdateValue(Hair, sheet.hair);
            UpdateValue(Eyes, sheet.eyes);
            UpdateValue(Biography, sheet.biography);
        }

        private void UpdateValue(Label label, string text)
        {
            if (label == null)
                return;
            if (label.Text != text)
                label.Text = text;
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