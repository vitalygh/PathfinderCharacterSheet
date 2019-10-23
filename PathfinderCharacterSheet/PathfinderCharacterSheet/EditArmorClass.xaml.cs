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
	public partial class EditArmorClass : ContentPage, ISheetView
	{
        private CharacterSheet.ArmorClass ac = null;

		public EditArmorClass ()
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
            ac = c.armorClass.Clone as CharacterSheet.ArmorClass;
        }

        public void UpdateView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            Total.Text = ac.GetTotal(sheet).ToString();
            ArmorBonus.Text = ac.GetArmorBonus(sheet).ToString();
            ArmorBonusFromItems.IsChecked = ac.itemsArmorBonus;
            UpdateArmorBonusFromItem();
            ShieldBonus.Text = ac.GetShieldBonus(sheet).ToString();
            ShieldBonusFromItems.IsChecked = ac.itemsShieldBonus;
            UpdateShieldBonusFromItem();
            DexModifier.Text = sheet.GetAbilityModifier(CharacterSheet.Ability.Dexterity).ToString();
            SizeModifier.Text = ac.sizeModifier.GetTotal(sheet).ToString();
            NaturalArmor.Text = ac.naturalArmor.GetTotal(sheet).ToString();
            DeflectionModifier.Text = ac.deflectionModifier.GetTotal(sheet).ToString();
            MiscModifiers.Text = ac.miscModifiers.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (!ac.Equals(sheet.armorClass))
            {
                sheet.armorClass = ac;
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
            }
        }

        private void ArmorBonus_Tapped(object sender, EventArgs e)
        {
            if (ac.itemsArmorBonus)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.armorBonus, "Edit Armor Class Armor Bonus", "Armor Bonus: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void ShieldBonus_Tapped(object sender, EventArgs e)
        {
            if (ac.itemsShieldBonus)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.armorBonus, "Edit Armor Class Shield Bonus", "Shield Bonus: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void SizeModifier_Tapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.sizeModifier, "Edit Armor Class Size Modifier", "Size Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void NaturalArmor_Tapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.naturalArmor, "Edit Armor Class Natural Armor", "Natural Armor: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void DeflectionModifier_Tapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.deflectionModifier, "Edit Armor Class Deflection Modifier", "Deflection Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void MiscModifiers_Tapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.miscModifiers, "Edit Armor Class Misc Modifiers", "Misc Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void UpdateArmorBonusFromItem()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var iab = ArmorBonusFromItems.IsChecked;
            ac.itemsArmorBonus = iab;
            ArmorBonusFrame.BackgroundColor = iab ? Color.LightGray : Color.White;
            ArmorBonus.TextDecorations = iab ? TextDecorations.None : TextDecorations.Underline;
            ArmorBonus.Text = ac.GetArmorBonus(sheet).ToString();
            Total.Text = ac.GetTotal(sheet).ToString();
        }

        private void UpdateShieldBonusFromItem()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var isb = ShieldBonusFromItems.IsChecked;
            ac.itemsShieldBonus = isb;
            ShieldBonusFrame.BackgroundColor = isb ? Color.LightGray : Color.White;
            ShieldBonus.TextDecorations = isb ? TextDecorations.None : TextDecorations.Underline;
            ShieldBonus.Text = ac.GetShieldBonus(sheet).ToString();
            Total.Text = ac.GetTotal(sheet).ToString();
        }

        private void ArmorBonusFromItems_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateArmorBonusFromItem();
        }

        private void ShieldBonusFromItems_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateShieldBonusFromItem();
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