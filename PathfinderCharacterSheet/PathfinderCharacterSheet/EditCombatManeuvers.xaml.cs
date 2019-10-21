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
        private CharacterSheet.ValueWithModifiers<int, CharacterSheet.IntSum> cmdSizeModifier = null;
        private CharacterSheet.ValueWithModifiers<int, CharacterSheet.IntSum> cmbSizeModifier = null;

        public EditCombatManeuvers ()
		{
			InitializeComponent ();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            cmdSizeModifier = c.cmdSizeModifier.Clone;
            cmbSizeModifier = c.cmbSizeModifier.Clone;
            CMDBaseAttackBonus.Text = c.baseAttackBonus.Total.ToString();
            CMBBaseAttackBonus.Text = CMDBaseAttackBonus.Text;
            CMDStrengthModifier.Text = c.GetAbilityModifier(CharacterSheet.Ability.Strength).ToString();
            CMBStrengthModifier.Text = CMDStrengthModifier.Text;
            CMDDexterityModifier.Text = c.GetAbilityModifier(CharacterSheet.Ability.Dexterity).ToString();
        }

        public void UpdateView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            CMDSizeModifier.Text = cmdSizeModifier.Total.ToString();
            CMBSizeModifier.Text = cmbSizeModifier.Total.ToString();
            CMDTotal.Text = c.GetCMD(cmdSizeModifier).ToString();
            CMBTotal.Text = c.GetCMB(cmbSizeModifier).ToString();
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var hasChanges = false;
            if (!cmdSizeModifier.Equals(c.cmdSizeModifier))
            {
                hasChanges = true;
                c.cmdSizeModifier = cmdSizeModifier;
            }
            if (!cmbSizeModifier.Equals(c.cmbSizeModifier))
            {
                hasChanges = true;
                c.cmbSizeModifier = cmbSizeModifier;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private void CMDSizeModifier_Tapped(object sender, EventArgs e)
        {
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(cmdSizeModifier, "Edit CMD Size Modifier", "Size Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void CMBSizeModifier_Tapped(object sender, EventArgs e)
        {
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(cmbSizeModifier, "Edit CMB Size Modifier", "Size Modifier: ", false);
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