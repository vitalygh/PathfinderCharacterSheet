﻿//#define SELECT_CURRENT_ATTACK
using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCombatManeuvers : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private ValueWithIntModifiers cmdSizeModifier = null;
        private ValueWithIntModifiers cmbSizeModifier = null;
#if SELECT_CURRENT_ATTACK
        private int currentAttack = 0;
#endif

        public EditCombatManeuvers ()
		{
			InitializeComponent ();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            cmdSizeModifier = sheet.cmdSizeModifier.Clone;
            cmbSizeModifier = sheet.cmbSizeModifier.Clone;
#if SELECT_CURRENT_ATTACK
            currentAttack = sheet.currentAttack;
            UpdateCurrentAttackPicker();
#else
            BaseAttackBonus.Text = sheet.GetBaseAttackBonus(0).ToString();
#endif
            StrengthModifier.Text = sheet.GetAbilityModifier(Ability.Strength).ToString();
            CMDDexterityModifier.Text = sheet.GetAbilityModifier(Ability.Dexterity).ToString();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            CMDSizeModifier.Text = cmdSizeModifier.GetValue(sheet).ToString();
            CMBSizeModifier.Text = cmbSizeModifier.GetValue(sheet).ToString();
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
#if SELECT_CURRENT_ATTACK
            CMDTotal.Text = sheet.GetCMD(sheet, cmdSizeModifier, currentAttack).ToString();
            CMBTotal.Text = sheet.GetCMB(sheet, cmbSizeModifier, currentAttack).ToString();
#else
            CMDTotal.Text = sheet.GetCMD(sheet, cmdSizeModifier, 0).ToString();
            CMBTotal.Text = sheet.GetCMB(sheet, cmbSizeModifier, 0).ToString();
#endif
        }

        private void EditToView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var hasChanges = false;
            if (!cmdSizeModifier.Equals(sheet.cmdSizeModifier))
            {
                hasChanges = true;
                sheet.cmdSizeModifier = cmdSizeModifier;
            }
            if (!cmbSizeModifier.Equals(sheet.cmbSizeModifier))
            {
                hasChanges = true;
                sheet.cmbSizeModifier = cmbSizeModifier;
            }
#if SELECT_CURRENT_ATTACK
            if (currentAttack != sheet.currentAttack)
            {
                hasChanges = true;
                sheet.currentAttack = currentAttack;
            }
#endif
            if (hasChanges)
                UIMediator.OnCharacterSheetChanged?.Invoke();
        }


#if SELECT_CURRENT_ATTACK
        private void UpdateCurrentAttackPicker()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var bab = sheet.baseAttackBonus;
            if (bab == null)
                return;
            var count = bab.Count;
            if (count <= 0)
                return;
            var items = new List<CharacterSheet.IntPickerItem>();
            var selectedIndex = -1;
            for (var i = 0; i < count; i++)
            {
                if (i == currentAttack)
                    selectedIndex = i;
                var item = new CharacterSheet.IntPickerItem()
                {
                    Name = sheet.GetBaseAttackBonusForPicker(i),
                    Value = i,
                };
                items.Add(item);
            }
            var oneAttack = count < 2;
            CurrentBaseAttackBonus.ItemsSource = items;
            CurrentBaseAttackBonus.SelectedIndex = selectedIndex;
            CurrentBaseAttackBonus.InputTransparent = oneAttack;
            BaseAttackBonusFrame.BackgroundColor = oneAttack ? Color.LightGray : Color.White;
        }

        private void CurrentBaseAttackBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (CurrentBaseAttackBonus.SelectedItem as CharacterSheet.IntPickerItem);
            if (selectedItem == null)
                return;
            currentAttack = selectedItem.Value;
            UpdateTotal();
        }
#endif

        private void CMDSizeModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, cmdSizeModifier, "Edit CMD Size Modifier", "Size Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void CMBSizeModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, cmbSizeModifier, "Edit CMB Size Modifier", "Size Modifier", false);
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