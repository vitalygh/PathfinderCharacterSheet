using System;

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
            UIHelpers.UpdateValue(CharacterName, sheet.Name);
            UIHelpers.UpdateValue(Alignment, sheet.Alignment.ToString());
            UIHelpers.UpdateValue(Experience, sheet.experience.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(NextLevel, sheet.nextLevelExperience.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(Level, sheet.LevelAsString);
            UIHelpers.UpdateValue(Deity, sheet.deity);
            UIHelpers.UpdateValue(Homeland, sheet.homeland);
            UIHelpers.UpdateValue(Race, sheet.Race);
            UIHelpers.UpdateValue(Size, sheet.size);
            UIHelpers.UpdateValue(Gender, sheet.gender);
            UIHelpers.UpdateValue(Age, sheet.age);
            UIHelpers.UpdateValue(CharacterHeight, sheet.height);
            UIHelpers.UpdateValue(Weight, sheet.weight);
            UIHelpers.UpdateValue(Hair, sheet.hair);
            UIHelpers.UpdateValue(Eyes, sheet.eyes);
            UIHelpers.UpdateValue(Biography, sheet.biography);
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