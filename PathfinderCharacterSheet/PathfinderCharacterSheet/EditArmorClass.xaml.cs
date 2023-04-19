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
	public partial class EditArmorClass : ContentPage, ISheetView
	{
        public class DexterityModifierSourcePickerItem
        {
            public string Name { get; set; }
            public ArmorClass.DexterityModifierSources Value { get; set; }
        }

        private Page pushedPage = null;
        private ArmorClass ac = null;

		public EditArmorClass ()
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
            ac = sheet.armorClass.Clone as ArmorClass;
            var values = Enum.GetValues(typeof(ArmorClass.DexterityModifierSources));
            var count = -1;
            var valueIndex = 0;
            var dexModSrcPickerItems = new List<DexterityModifierSourcePickerItem>();
            foreach (var v in values)
            {
                count += 1;
                var value = (ArmorClass.DexterityModifierSources)v;
                if (value == ac.DexterityModifierSource)
                    valueIndex = count;
                dexModSrcPickerItems.Add(new DexterityModifierSourcePickerItem()
                {
                    Name = v.ToString(),
                    Value = value,
                });
            }
            DexModifierSource.ItemsSource = dexModSrcPickerItems;
            DexModifierSource.SelectedIndex = valueIndex;
        }

        public void UpdateView()
        {
            pushedPage = null;
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
            DexModifier.Text = ac.GetDexterityModifier(sheet).ToString();
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
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void ArmorBonus_Tapped(object sender, EventArgs e)
        {
            if (ac.itemsArmorBonus)
                return;
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.armorBonus, "Edit Armor Class Armor Bonus", "Armor Bonus", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ShieldBonus_Tapped(object sender, EventArgs e)
        {
            if (ac.itemsShieldBonus)
                return;
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.armorBonus, "Edit Armor Class Shield Bonus", "Shield Bonus", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DexModifier_Tapped(object sender, EventArgs e)
        {
            if (ac.DexterityModifierSource != ArmorClass.DexterityModifierSources.Custom)
                return;
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.dexterityModifier, "Edit Armor Class Dexterity Modifier", "Dexterity Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SizeModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.sizeModifier, "Edit Armor Class Size Modifier", "Size Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void NaturalArmor_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.naturalArmor, "Edit Armor Class Natural Armor", "Natural Armor", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DeflectionModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.deflectionModifier, "Edit Armor Class Deflection Modifier", "Deflection Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void MiscModifiers_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.miscModifiers, "Edit Armor Class Misc Modifiers", "Misc Modifier", false);
            pushedPage = eivwm;
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

        private void DexModifierSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var pickerItem = DexModifierSource.SelectedItem as DexterityModifierSourcePickerItem;
            if (pickerItem == null)
                return;
            ac.DexterityModifierSource = pickerItem.Value;
            var custom = pickerItem.Value == ArmorClass.DexterityModifierSources.Custom;
            DexModifier.Text = ac.GetDexterityModifier(sheet).ToString();
            DexModifier.TextDecorations = custom ? TextDecorations.Underline : TextDecorations.None;
            DexModifierFrame.BackgroundColor = custom ? Color.White : Color.LightGray;
            Total.Text = ac.GetTotal(sheet).ToString();
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