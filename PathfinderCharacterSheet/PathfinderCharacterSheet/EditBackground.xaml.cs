﻿using System;
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
        private CharacterSheet.ValueWithIntModifiers experience = null;
        private CharacterSheet.ValueWithIntModifiers nextLevelExperience = null;

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
            experience = sheet.experience.Clone as CharacterSheet.ValueWithIntModifiers;
            nextLevelExperience = sheet.nextLevelExperience.Clone as CharacterSheet.ValueWithIntModifiers;
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            Level.Text = CharacterSheet.LevelOfClass.AsString(sheet, levelOfClass);
            Experience.Text = experience.GetTotal(sheet).ToString();
            NextLevel.Text = nextLevelExperience.GetTotal(sheet).ToString();
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

        private bool EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return false;
            var hasChanges = false;
            hasChanges |= CopyCheckEqual(CharacterName.Text, ref sheet.name);
            var alignment = Alignment.SelectedItem as AlignmentPickerItem;
            if ((alignment != null) && (sheet.Alignment != alignment.Value))
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
            if (!CharacterSheet.IsEqual(sheet.levelOfClass, levelOfClass))
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var rename = (sheet.Name != CharacterName.Text);
            if (rename)
            {
                CharacterSheetStorage.Instance.DeleteCharacter(sheet);
                CharacterSheetStorage.Instance.selectedCharacter = sheet;
            }
            var hasChanges = EditToView();
            if (rename || hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
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

        private void Experience_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, nextLevelExperience, "Edit Next Level Experience", "Next Level Experience", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }
    }
}