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
	public partial class EditBackground : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private LevelOfClassList levelOfClass = new LevelOfClassList();
        private ValueWithIntModifiers experience = null;
        private ValueWithIntModifiers nextLevelExperience = null;

        public class AlignmentPickerItem
        {
            public string Name { set; get; }
            public Alignment Value { set; get; }
        }

        public EditBackground()
		{
			InitializeComponent ();
            InitControls();
            UpdateView();
        }

        private void InitControls()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            levelOfClass = sheet.levelOfClass?.Clone;
            var alignments = new List<AlignmentPickerItem>();
            var values = Enum.GetValues(typeof(Alignment));
            var selectedIndex = -1;
            var index = -1;
            foreach (var v in values)
            {
                index += 1;
                var alignment = new AlignmentPickerItem()
                {
                    Name = v.ToString(),
                    Value = (Alignment)v,
                };
                alignments.Add(alignment);
                if (sheet.Alignment == alignment.Value)
                    selectedIndex = index;
            }
            Alignment.ItemsSource = alignments;
            Alignment.ItemDisplayBinding = new Binding("Name");
            Alignment.SelectedIndex = selectedIndex;
            CharacterName.Text = sheet.name;
            experience = sheet.experience.Clone;
            nextLevelExperience = sheet.nextLevelExperience.Clone;
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            Level.Text = levelOfClass?.AsString(sheet);
            Experience.Text = experience.GetValue(sheet).ToString();
            NextLevel.Text = nextLevelExperience.GetValue(sheet).ToString();
        }

        private bool EditToView()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return false;
            var hasChanges = false;
            hasChanges |= CopyCheckEqual(CharacterName.Text, ref sheet.name);
            if ((Alignment.SelectedItem is AlignmentPickerItem alignment) && (sheet.Alignment != alignment.Value))
            {
                hasChanges |= true;
                sheet.Alignment = alignment.Value;
            }
            if (!sheet.experience.Equals(experience))
            {
                hasChanges |= true;
                sheet.experience = experience;
            }
            if (!sheet.nextLevelExperience.Equals(nextLevelExperience))
            {
                hasChanges |= true;
                sheet.nextLevelExperience = nextLevelExperience;
            }
            if (sheet.levelOfClass != levelOfClass)
            {
                hasChanges = true;
                sheet.levelOfClass = levelOfClass;
            }
            hasChanges |= CopyCheckEqual(Deity.Text, ref sheet.deity);
            hasChanges |= CopyCheckEqual(Homeland.Text, ref sheet.homeland);
            hasChanges |= CopyCheckEqual(Race.Text, ref sheet.race);
            hasChanges |= CopyCheckEqual(Size.Text, ref sheet.size);
            hasChanges |= CopyCheckEqual(Gender.Text, ref sheet.gender);
            hasChanges |= CopyCheckEqual(Age.Text, ref sheet.age);
            hasChanges |= CopyCheckEqual(CharacterHeight.Text, ref sheet.height);
            hasChanges |= CopyCheckEqual(Weight.Text, ref sheet.weight);
            hasChanges |= CopyCheckEqual(Hair.Text, ref sheet.hair);
            hasChanges |= CopyCheckEqual(Eyes.Text, ref sheet.eyes);
            hasChanges |= CopyCheckEqual(Biography.Text, ref sheet.biography);
            return hasChanges;
        }

        private bool CopyCheckEqual(string from, ref string to)
        {
            var hasChanged = from != to;
            to = from;
            return hasChanged;
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            var rename = sheet.Name != CharacterName.Text;
            if (rename)
            {
                MainPage.DeleteCharacter?.Invoke(sheet);
                MainPage.SetSelectedCharacter?.Invoke(sheet);
            }
            var hasChanges = EditToView();
            if (rename || hasChanges)
                MainPage.OnCharacterSheetChanged?.Invoke();
            Navigation.PopAsync();
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            var characterName = string.Empty;
            if ((sheet != null) && !string.IsNullOrWhiteSpace(sheet.name))
                characterName = " \"" + sheet.name + "\"";
            bool allow = await DisplayAlert("Remove character" + characterName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                MainPage.DeleteCharacter?.Invoke(MainPage.GetSelectedCharacter?.Invoke());
                await Navigation.PopToRootAsync();
            }
        }

        private void Experience_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, experience, "Edit Experience", "Experience", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void NextLevel_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, nextLevelExperience, "Edit Next Level Experience", "Next Level Experience", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }
    }
}