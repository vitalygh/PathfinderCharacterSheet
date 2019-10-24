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
	public partial class EditLevel : ContentPage
	{
        private List<CharacterSheet.LevelOfClass> levelsOfClass = null;
        private CharacterSheet.LevelOfClass level = null;

        public EditLevel ()
		{
			InitializeComponent ();
		}

        public void Init(List<CharacterSheet.LevelOfClass> levelsOfClass, CharacterSheet.LevelOfClass level)
        {
            this.levelsOfClass = levelsOfClass;
            this.level = level;
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            Delete.IsEnabled = level != null;
            if (level == null)
                return;
            Level.Text = level.Level.ToString();
            ClassName.Text = level.ClassName.ToString();
        }

        private void EditToView()
        {
            if (level == null)
            {
                level = new CharacterSheet.LevelOfClass();
                levelsOfClass.Add(level);
            }
            MainPage.StrToInt(Level.Text, ref level.level);
            level.className = ClassName.Text;
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
            if ((level != null) && !string.IsNullOrWhiteSpace(level.ClassName))
                className = " of class \"" + level.ClassName + "\"";
            bool allow = await DisplayAlert("Remove level" + className, "Are you sure?", "Yes", "No");
            if (allow)
            {
                levelsOfClass.Remove(level);
                await Navigation.PopAsync();
            }
        }
    }
}