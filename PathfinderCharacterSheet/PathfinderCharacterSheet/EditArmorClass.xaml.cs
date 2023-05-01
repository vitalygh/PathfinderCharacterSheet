using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DexterityModifierSourcePickerItem = System.Tuple<string, PathfinderCharacterSheet.CharacterSheets.V1.DexterityModifierSource>;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditArmorClass : ContentPage, ISheetView
	{
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            ac = sheet.armorClass.Clone;
            var values = Enum.GetValues(typeof(DexterityModifierSource));
            var count = -1;
            var valueIndex = 0;
            var dexModSrcPickerItems = new List<DexterityModifierSourcePickerItem>();
            foreach (var v in values)
            {
                count += 1;
                var value = (DexterityModifierSource)v;
                if (value == ac.DexterityModifierSource)
                    valueIndex = count;
                dexModSrcPickerItems.Add(new DexterityModifierSourcePickerItem(v.ToString(), value));
            }
            DexModifierSource.ItemsSource = dexModSrcPickerItems;
            DexModifierSource.SelectedIndex = valueIndex;
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            SizeModifier.Text = ac.sizeModifier.GetValue(sheet).ToString();
            NaturalArmor.Text = ac.naturalArmor.GetValue(sheet).ToString();
            DeflectionModifier.Text = ac.deflectionModifier.GetValue(sheet).ToString();
            MiscModifiers.Text = ac.miscModifiers.GetValue(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (!ac.Equals(sheet.armorClass))
            {
                sheet.armorClass = ac;
                UIMediator.OnCharacterSheetChanged?.Invoke();
            }
        }

        private void ArmorBonus_Tapped(object sender, EventArgs e)
        {
            if (ac.itemsArmorBonus)
                return;
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.armorBonus, "Edit Armor Class Shield Bonus", "Shield Bonus", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DexModifier_Tapped(object sender, EventArgs e)
        {
            if (ac.DexterityModifierSource != DexterityModifierSource.Custom)
                return;
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, ac.miscModifiers, "Edit Armor Class Misc Modifiers", "Misc Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void UpdateArmorBonusFromItem()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (!(DexModifierSource.SelectedItem is DexterityModifierSourcePickerItem pickerItem))
                return;
            ac.DexterityModifierSource = pickerItem.Item2;
            var custom = pickerItem.Item2 == DexterityModifierSource.Custom;
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