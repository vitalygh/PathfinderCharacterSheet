﻿using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSkillRanks : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private ValueWithIntModifiers skillRanks = null;

        public EditSkillRanks()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            skillRanks = sheet.skillRanks.Clone;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            var ranksSpent = 0;
            foreach (var skill in sheet.skills)
                ranksSpent += skill.rank.GetValue(sheet);
            var ranks = skillRanks.GetValue(sheet);
            var ranksLeft = ranks - ranksSpent;
            Left.Text = ranksLeft.ToString();
            Ranks.Text = ranks.ToString();
            Spent.Text = ranksSpent.ToString();
        }

        private void EditToView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (!sheet.skillRanks.Equals(skillRanks))
            {
                sheet.skillRanks.Fill(skillRanks);
                UIMediator.OnCharacterSheetChanged?.Invoke();
            }
        }

        private void Ranks_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, skillRanks, "Edit Skill Ranks", "Skill Ranks", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
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
    }
}