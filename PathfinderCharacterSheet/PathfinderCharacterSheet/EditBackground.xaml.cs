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
        private Page pushedPage = null;
        private List<CharacterSheet.LevelOfClass> levelOfClass = new List<CharacterSheet.LevelOfClass>();

        public class AlignmentPickerItem
        {
            public string Name { set; get; }
            public CharacterSheet.Alignments Value { set; get; }
        }

        public EditBackground()
		{
			InitializeComponent ();
            InitControls();
            UpdateView();
        }

        private void InitControls()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            levelOfClass = CharacterSheet.LevelOfClass.CreateClone(sheet.levelOfClass);
            var alignments = new List<AlignmentPickerItem>();
            var values = Enum.GetValues(typeof(CharacterSheet.Alignments));
            var selectedIndex = -1;
            var index = -1;
            foreach (var v in values)
            {
                index += 1;
                var alignment = new AlignmentPickerItem()
                {
                    Name = v.ToString(),
                    Value = (CharacterSheet.Alignments)v,
                };
                alignments.Add(alignment);
                if (sheet.Alignment == alignment.Value)
                    selectedIndex = index;
            }
            Alignment.ItemsSource = alignments;
            Alignment.ItemDisplayBinding = new Binding("Name");
            Alignment.SelectedIndex = selectedIndex;
            CharacterName.Text = sheet.name;
            Experience.Text = sheet.experience.ToString();
            NextLevel.Text = sheet.nextLevelExperience.ToString();
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

        public void UpdateView()
        {
            pushedPage = null;
            Level.Text = CharacterSheet.LevelOfClass.AsString(levelOfClass);
        }

        private bool StringToAlignment(string alignmentName, ref CharacterSheet.Alignments alignment)
        {
            var values = Enum.GetValues(typeof(CharacterSheet.Alignments));
            foreach (var v in values)
                if (v.ToString() == alignmentName)
                {
                    alignment = (CharacterSheet.Alignments)v;
                    return true;
                }
            return false;
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            c.name = CharacterName.Text;
            var alignment = Alignment.SelectedItem as AlignmentPickerItem;
            if (alignment != null)
                c.Alignment = alignment.Value;
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

        private void Level_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var loc = new EditLevelOfClass();
            loc.Init(levelOfClass);
            pushedPage = loc;
            Navigation.PushAsync(loc);
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

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var characterName = string.Empty;
            if ((sheet != null) && !string.IsNullOrWhiteSpace(sheet.name))
                characterName = " \"" + sheet.name + "\"";
            bool allow = await DisplayAlert("Remove character" + characterName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                CharacterSheetStorage.Instance.DeleteCharacter(CharacterSheetStorage.Instance.selectedCharacter);
                await Navigation.PopToRootAsync();
            }
        }
    }
}