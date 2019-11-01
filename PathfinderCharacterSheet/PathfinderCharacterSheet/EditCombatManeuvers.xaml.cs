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
            CMDBaseAttackBonus.Text = sheet.baseAttackBonus.GetTotal(sheet).ToString();
            CMBBaseAttackBonus.Text = CMDBaseAttackBonus.Text;
            CMDStrengthModifier.Text = sheet.GetAbilityModifier(CharacterSheet.Ability.Strength).ToString();
            CMBStrengthModifier.Text = CMDStrengthModifier.Text;
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
            CMDTotal.Text = sheet.GetCMD(sheet, cmdSizeModifier).ToString();
            CMBTotal.Text = sheet.GetCMB(sheet, cmbSizeModifier).ToString();
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
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
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