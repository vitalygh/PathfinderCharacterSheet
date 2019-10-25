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
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            BaseAttackBonus.Text = sheet.baseAttackBonus.GetTotal(sheet).ToString();
            SizeModifier.Text = sizeModifiers.GetTotal(sheet).ToString();
            Value.Text = attackBonus.baseValue.ToString();
            UpdateModifiersSum();
            MainPage.FillIntMLGrid(Modifiers, sheet, attackBonus.modifiers, "Modifiers:", EditModifier, EditModifier, (modifiers, modifier) => UpdateModifiersSum());
        }

        private void UpdateTotal()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var total = 0;
            MainPage.StrToInt(Value.Text, ref attackBonus.baseValue);
            total += sheet.baseAttackBonus.GetTotal(sheet);
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
            eivwm.Init(sheet, sizeModifiers, "Edit Size Modifier", "Size Modifier: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
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

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            MainPage.StrToInt(Value.Text, ref attackBonus.baseValue);
            if (!sizeModifiers.Equals(sheet.attackSizeModifier) || !attackBonus.Equals(sheet.attackBonusModifiers))
            {
                sheet.attackBonusModifiers = attackBonus;
                sheet.attackSizeModifier = sizeModifiers;
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }
    }
}