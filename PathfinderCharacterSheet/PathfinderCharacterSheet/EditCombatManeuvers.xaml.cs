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
	public partial class EditCombatManeuvers : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet.ValueWithIntModifiers cmdSizeModifier = null;
        private CharacterSheet.ValueWithIntModifiers cmbSizeModifier = null;
        private int currentAttack = 0;

        public EditCombatManeuvers ()
		{
			InitializeComponent ();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            cmdSizeModifier = sheet.cmdSizeModifier.Clone as CharacterSheet.ValueWithIntModifiers;
            cmbSizeModifier = sheet.cmbSizeModifier.Clone as CharacterSheet.ValueWithIntModifiers;
            currentAttack = sheet.currentAttack;
            UpdateCurrentAttackPicker();
            StrengthModifier.Text = sheet.GetAbilityModifier(CharacterSheet.Ability.Strength).ToString();
            CMDDexterityModifier.Text = sheet.GetAbilityModifier(CharacterSheet.Ability.Dexterity).ToString();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            CMDSizeModifier.Text = cmdSizeModifier.GetTotal(sheet).ToString();
            CMBSizeModifier.Text = cmbSizeModifier.GetTotal(sheet).ToString();
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            CMDTotal.Text = sheet.GetCMD(sheet, cmdSizeModifier, currentAttack).ToString();
            CMBTotal.Text = sheet.GetCMB(sheet, cmbSizeModifier, currentAttack).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            if (currentAttack != sheet.currentAttack)
            {
                hasChanges = true;
                sheet.currentAttack = currentAttack;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
        }


        private void UpdateCurrentAttackPicker()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            CurrentBaseAttackBonus.ItemsSource = items;
            CurrentBaseAttackBonus.SelectedIndex = selectedIndex;
            var oneAttack = count < 2;
            CurrentBaseAttackBonus.InputTransparent = oneAttack;
            CurrentBaseAttackBonusFrame.BackgroundColor = oneAttack ? Color.LightGray : Color.White;
        }

        private void CurrentBaseAttackBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (CurrentBaseAttackBonus.SelectedItem as CharacterSheet.IntPickerItem);
            if (selectedItem == null)
                return;
            currentAttack = selectedItem.Value;
            UpdateTotal();
        }

        private void CMDSizeModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, cmdSizeModifier, "Edit CMD Size Modifier", "Size Modifier: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void CMBSizeModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, cmbSizeModifier, "Edit CMB Size Modifier", "Size Modifier: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
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
    }
}