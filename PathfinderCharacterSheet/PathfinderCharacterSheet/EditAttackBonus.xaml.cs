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
	public partial class EditAttackBonus : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private ValueWithIntModifiers sizeModifiers = null;
        private ValueWithIntModifiers attackBonus = null;
        private int currentAttack = 0;

        public EditAttackBonus ()
		{
			InitializeComponent ();
            Init();
		}

        private void Init()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            sizeModifiers = sheet.attackSizeModifier.Clone;
            attackBonus = sheet.attackBonusModifiers.Clone;
            currentAttack = sheet.currentAttack;
            UpdateCurrentAttackPicker();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            //BaseAttackBonus.Text = sheet.GetBaseAttackBonus().ToString();
            SizeModifier.Text = sizeModifiers.GetValue(sheet).ToString();
            Value.Text = attackBonus.BaseValue.ToString();
            UpdateModifiersSum();
            MainPage.FillIntMLGrid(Modifiers, sheet, attackBonus.Modifiers, "Modifiers", EditModifier, EditModifier, ReorderModifiers, (modifiers, modifier) => UpdateModifiersSum());
        }

        private void UpdateCurrentAttackPicker()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var babs = sheet.baseAttackBonus;
            if (babs == null)
                return;
            var count = sheet.AttacksCount;
            if (count <= 0)
                return;
            var items = new List<IntPickerItem>();
            var selectedIndex = -1;
            for (var i = 0; i < count; i++)
            {
                if (i == currentAttack)
                    selectedIndex = i;
                var item = new IntPickerItem()
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
            var selectedItem = (CurrentBaseAttackBonus.SelectedItem as IntPickerItem);
            if (selectedItem == null)
                return;
            currentAttack = selectedItem.Value;
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var total = 0;
            var baseValue = attackBonus.BaseValue;
            MainPage.StrToInt(Value.Text, ref baseValue);
            attackBonus.BaseValue = baseValue;
            total += sizeModifiers.GetValue(sheet);
            total += attackBonus.GetValue(sheet);
            var values = string.Empty;
            var count = sheet.AttacksCount;
            if (count <= 0)
                values = "+0";
            else
                for (var i = 0; i < count; i++)
                {
                    var bab = sheet.GetBaseAttackBonus(i);
                    bab += total;
                    if (values.Length > 0)
                        values += ", ";
                    var sbab = bab >= 0 ? "+" + bab : bab.ToString();
                    values += ((count > 1) && (i == currentAttack)) ? "[" + sbab + "]" : sbab;
                }
            Total.Text = values;
        }

        private void UpdateModifiersSum()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            ModifiersSum.Text = attackBonus.Modifiers?.GetValue(sheet).ToString();
            UpdateTotal();
        }

        private void ReorderModifiers(IntModifiersList modifiers)
        {
            if (pushedPage != null)
                return;
            var ri = new ReorderIntModifiers();
            pushedPage = ri;
            var items = new IntModifiersList();
            foreach (var item in modifiers)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                modifiers.Clear();
                foreach (var item in reordered)
                    modifiers.Add(item as IntModifier);
            });
            Navigation.PushAsync(pushedPage);
        }

        private void EditModifier(IntModifiersList modifiers)
        {
            EditModifier(modifiers, null);
        }

        private void EditModifier(IntModifiersList modifiers, IntModifier modifier)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var baseValue = attackBonus.BaseValue;
            MainPage.StrToInt(Value.Text, ref baseValue);
            attackBonus.BaseValue = baseValue;
            if (!sizeModifiers.Equals(sheet.attackSizeModifier) || !attackBonus.Equals(sheet.attackBonusModifiers) || (currentAttack != sheet.currentAttack))
            {
                sheet.currentAttack = currentAttack;
                sheet.attackBonusModifiers = attackBonus;
                sheet.attackSizeModifier = sizeModifiers;
                MainPage.OnCharacterSheetChanged?.Invoke();
            }
        }
    }
}