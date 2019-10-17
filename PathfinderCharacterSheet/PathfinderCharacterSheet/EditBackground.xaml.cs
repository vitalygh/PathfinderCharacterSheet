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
	public partial class EditBackground : ContentPage, ISheetView
	{
        private List<CharacterSheet.LevelOfClass> levelOfClass = new List<CharacterSheet.LevelOfClass>();

        public EditBackground()
		{
			InitializeComponent ();
            InitControls();
            UpdateView();
        }

        private void InitControls()
        {
            levelOfClass = CharacterSheet.LevelOfClass.Clone(CharacterSheetStorage.Instance.selectedCharacter.levelOfClass);
            var alignments = new List<CharacterSheet.Alignment>();
            var values = Enum.GetValues(typeof(CharacterSheet.Alignment));
            foreach (var v in values)
                alignments.Add((CharacterSheet.Alignment)v);
            Alignment.ItemsSource = alignments;
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) => EditLevel();
            Level.GestureRecognizers.Add(tgr);
        }

        private void EditLevel()
        {
            var loc = new EditLevelOfClass();
            loc.Init(levelOfClass);
            Navigation.PushAsync(loc);
        }

        public void UpdateView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            CharacterName.Text = c.name;
            Alignment.SelectedItem = c.alignment;
            Experience.Text = c.experience.ToString();
            NextLevel.Text = c.nextLevelExperience.ToString();
            Level.Text = CharacterSheet.LevelOfClass.AsString(levelOfClass);
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

        private bool StringToAlignment(string alignmentName, ref CharacterSheet.Alignment alignment)
        {
            var values = Enum.GetValues(typeof(CharacterSheet.Alignment));
            foreach (var v in values)
                if (v.ToString() == alignmentName)
                {
                    alignment = (CharacterSheet.Alignment)v;
                    return true;
                }
            return false;
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            c.name = CharacterName.Text;
            if (Alignment.SelectedItem != null)
                c.alignment = (CharacterSheet.Alignment)Alignment.SelectedItem;
            MainPage.StrToInt(Experience.Text, ref c.experience);
            MainPage.StrToInt(NextLevel.Text, ref c.nextLevelExperience);
            c.levelOfClass = levelOfClass;
            c.deity = Deity.Text;
            c.homeland = Homeland.Text;
            c.race = Race.Text;
            c.size = Size.Text;
            c.gender = Gender.Text;
            c.age = Age.Text;
            c.height = CharacterHeight.Text;
            c.weight = Weight.Text;
            c.hair = Hair.Text;
            c.eyes = Eyes.Text;
            c.biography = Biography.Text;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c.Name != CharacterName.Text)
                CharacterSheetStorage.Instance.DeleteCharacter(c);
            EditToView();
            CharacterSheetStorage.Instance.SaveCharacter(c);
            CharacterSheetStorage.Instance.selectedCharacter = c;
            Navigation.PopAsync();
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            CharacterSheetStorage.Instance.DeleteCharacter(CharacterSheetStorage.Instance.selectedCharacter);
            Navigation.PopToRootAsync();
        }
    }
}