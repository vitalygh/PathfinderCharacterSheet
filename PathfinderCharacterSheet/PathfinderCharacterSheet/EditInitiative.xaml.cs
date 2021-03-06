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
    public partial class EditInitiative : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private CharacterSheet.ValueWithIntModifiers modifiers = null;

        public EditInitiative()
        {
            InitializeComponent();
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            modifiers = sheet.initiative.miscModifiers.Clone as CharacterSheet.ValueWithIntModifiers;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var dexMod = sheet.GetAbilityModifier(CharacterSheet.Ability.Dexterity);
            DexModifier.Text = dexMod.ToString();
            var miscMod = modifiers.GetTotal(sheet);
            MiscModifiers.Text = miscMod.ToString();
            Total.Text = (dexMod + miscMod).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var hasChanges = !sheet.initiative.miscModifiers.Equals(modifiers);
            if (hasChanges)
            {
                sheet.initiative.miscModifiers.Fill(modifiers);
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void MiscModifiers_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, modifiers, "Edit Initiative Misc Modifiers", "Misc Modifiers", false);
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