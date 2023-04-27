using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditLevel : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private LevelOfClassList levelsOfClass = null;
        private LevelOfClass source = null;
        private LevelOfClass level = null;

        public EditLevel ()
		{
			InitializeComponent ();
		}

        public void Init(LevelOfClassList levelsOfClass, LevelOfClass level)
        {
            this.levelsOfClass = levelsOfClass;
            source = level;
            if (source != null)
            {
                this.level = source.Clone;
                ClassName.Text = source.ClassName.ToString();
            }
            else
            {
                this.level = new LevelOfClass();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            Level.Text = level.GetValue(sheet).ToString();
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
                var sheet = UIMediator.GetSelectedCharacter?.Invoke();
                if ((sheet != null) && (source.level.GetValue(sheet) > 1))
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, level.level, "Edit Experience", "Experience", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }
    }
}