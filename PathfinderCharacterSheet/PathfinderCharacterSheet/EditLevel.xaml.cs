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
	public partial class EditLevel : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private List<CharacterSheet.LevelOfClass> levelsOfClass = null;
        private CharacterSheet.LevelOfClass source = null;
        private CharacterSheet.LevelOfClass level = null;

        public EditLevel ()
		{
			InitializeComponent ();
		}

        public void Init(List<CharacterSheet.LevelOfClass> levelsOfClass, CharacterSheet.LevelOfClass level)
        {
            this.levelsOfClass = levelsOfClass;
            source = level;
            if (source != null)
            {
                this.level = source.Clone as CharacterSheet.LevelOfClass;
                ClassName.Text = source.ClassName.ToString();
            }
            else
            {
                this.level = new CharacterSheet.LevelOfClass();
                ClassName.Text = string.Empty;
            }
            Delete.IsEnabled = source != null;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (level == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            Level.Text = level.GetLevel(sheet).ToString();
        }

        private void EditToView()
        {
            level.className = ClassName.Text;
            if (source == null)
            {
                levelsOfClass.Add(level);
                return;
            }
            source.Fill(level);
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
            var className = string.Empty;
            if (source != null)
            {
                if (!string.IsNullOrWhiteSpace(source.ClassName))
                    className = " of class \"" + level.ClassName + "\"";
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if ((sheet != null) && (source.level.GetTotal(sheet) > 1))
                    className = "s" + className;
            }
            bool allow = await DisplayAlert("Remove level" + className, "Are you sure?", "Yes", "No");
            if (allow)
            {
                levelsOfClass.Remove(source);
                await Navigation.PopAsync();
            }
        }

        private void Level_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, level.level, "Edit Experience", "Experience: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }
    }
}