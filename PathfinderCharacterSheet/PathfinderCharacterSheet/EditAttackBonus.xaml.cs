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
	public partial class EditAttackBonus : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet.ValueWithIntModifiers sizeModifiers = null;
        private CharacterSheet.ValueWithIntModifiers attackBonus = null;
        private int currentAttack = 0;

        public EditAttackBonus ()
		{
			InitializeComponent ();
            Init();
		}

        private void Init()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            sizeModifiers = sheet.attackSizeModifier.Clone as CharacterSheet.ValueWithIntModifiers;
            attackBonus = sheet.attackBonusModifiers.Clone as CharacterSheet.ValueWithIntModifiers;
            currentAttack = sheet.currentAttack;
            UpdateCurrentAttackPicker();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            //BaseAttackBonus.Text = sheet.GetBaseAttackBonus().ToString();
            SizeModifier.Text = sizeModifiers.GetTotal(sheet).ToString();
            Value.Text = attackBonus.baseValue.ToString();
            UpdateModifiersSum();
            MainPage.FillIntMLGrid(Modifiers, sheet, attackBonus.modifiers, "Modifiers", EditModifier, EditModifier, ReorderModifiers, (modifiers, modifier) => UpdateModifiersSum());
        }

        private void UpdateCurrentAttackPicker()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var babs = sheet.baseAttackBonus;
            if (babs == null)
                return;
            var count = babs.Count;
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

        private void UpdateTotal()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var total = 0;
            MainPage.StrToInt(Value.Text, ref attackBonus.baseValue);
            total += sheet.GetBaseAttackBonus(currentAttack);
            total += sizeModifiers.GetTotal(sheet);
            total += attackBonus.GetTotal(sheet);
            Total.Text = total.ToString();
        }

        private void UpdateModifiersSum()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            ModifiersSum.Text = attackBonus.modifiers.GetTotal(sheet).ToString();
            UpdateTotal();
        }

        private void ReorderModifiers(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ri = new ReorderIntModifiers();
            pushedPage = ri;
            var items = new CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum>();
            foreach (var item in modifiers)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                modifiers.Clear();
                foreach (var item in reordered)
                    modifiers.Add(item as CharacterSheet.IntModifier);
                CharacterSheetStorage.Instance.SaveCharacter();
            });
            Navigation.PushAsync(pushedPage);
        }

        private void EditModifier(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers)
        {
            EditModifier(modifiers, null);
        }

        private void EditModifier(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers, CharacterSheet.IntModifier modifier)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var page = new EditIntModifier();
            page.Init(sheet, modifiers, modifier);
            pushedPage = page;
            Navigation.PushAsync(page);
        }

        public void SizeModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sizeModifiers, "Edit Size Modifier", "Size Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
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

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            MainPage.StrToInt(Value.Text, ref attackBonus.baseValue);
            if (!sizeModifiers.Equals(sheet.attackSizeModifier) || !attackBonus.Equals(sheet.attackBonusModifiers) || (currentAttack != sheet.currentAttack))
            {
                sheet.currentAttack = currentAttack;
                sheet.attackBonusModifiers = attackBonus;
                sheet.attackSizeModifier = sizeModifiers;
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }
    }
}