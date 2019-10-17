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
	public partial class EditHP : ContentPage, ISheetView
	{
        private List<CharacterSheet.IntModifier> maxHPModifiers = null;
        private List<CharacterSheet.IntModifier> tempHPModifiers = null;
        private List<CharacterSheet.IntModifier> damageResistModifiers = null;

        public EditHP ()
		{
			InitializeComponent ();
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            maxHPModifiers = CharacterSheet.IntModifier.Clone(c.hp.maxHPModifiers);
            tempHPModifiers = CharacterSheet.IntModifier.Clone(c.hp.tempHPModifiers);
            damageResistModifiers = CharacterSheet.IntModifier.Clone(c.hp.damageResistModifiers);
            MaxHP.Text = c.hp.maxHP.ToString();
            HP.Text = c.hp.hp.ToString();
            DamageResist.Text = c.hp.damageResist.ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            UpdateMaxHP();
            UpdateTempHP();
            UpdateDamageResist();
            MainPage.FillIntModifierGrid(MaxHPModifiers, maxHPModifiers, "Max HP Modifiers:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateMaxHP());
            MainPage.FillIntModifierGrid(TempHPModifiers, tempHPModifiers, "Temp HP Modifiers:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateTempHP());
            MainPage.FillIntModifierGrid(DamageResistModifiers, damageResistModifiers, "Damage Resist Modifiers:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateDamageResist());
        }

        private void UpdateMaxHP()
        {
            var sum = CharacterSheet.Sum(maxHPModifiers);
            MaxHPModifiersSum.Text = sum.ToString();
            var maxHP = 0;
            MainPage.StrToInt(MaxHP.Text, ref maxHP);
            MaxHPTotal.Text = (sum + maxHP).ToString();
        }

        private void UpdateTempHP()
        {
            var sum = CharacterSheet.Sum(tempHPModifiers);
            HPModifiersSum.Text = sum.ToString();
            var hp = 0;
            MainPage.StrToInt(HP.Text, ref hp);
            HPTotal.Text = (sum + hp).ToString();
        }

        private void UpdateDamageResist()
        {
            var sum = CharacterSheet.Sum(damageResistModifiers);
            DamageResistModifiersSum.Text = sum.ToString();
            var dr = 0;
            MainPage.StrToInt(DamageResist.Text, ref dr);
            DamageResistTotal.Text = (sum + dr).ToString();
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var anyChanged = false;
            anyChanged |= MainPage.StrToInt(MaxHP.Text, ref c.hp.maxHP);
            anyChanged |= MainPage.StrToInt(HP.Text, ref c.hp.hp);
            anyChanged |= MainPage.StrToInt(DamageResist.Text, ref c.hp.damageResist);
            if (!CharacterSheet.IsEqual(c.hp.maxHPModifiers, maxHPModifiers))
            {
                anyChanged = true;
                c.hp.maxHPModifiers = maxHPModifiers;
            }
            if (!CharacterSheet.IsEqual(c.hp.tempHPModifiers, tempHPModifiers))
            {
                anyChanged = true;
                c.hp.tempHPModifiers = tempHPModifiers;
            }
            if (!CharacterSheet.IsEqual(c.hp.damageResistModifiers, damageResistModifiers))
            {
                anyChanged = true;
                c.hp.damageResistModifiers = damageResistModifiers;
            }
            if (anyChanged)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private void EditIntModifier(List<CharacterSheet.IntModifier> modifiers)
        {
            EditIntModifier(modifiers, null);
        }

        private void EditIntModifier(List<CharacterSheet.IntModifier> modifiers, CharacterSheet.IntModifier mod)
        {
            var page = new EditIntModifier();
            page.Init(modifiers, mod);
            Navigation.PushAsync(page);
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

        private void MaxHP_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMaxHP();
        }

        private void HP_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTempHP();
        }

        private void DamageResist_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDamageResist();
        }

        private void MaxHPModifiersSum_Tapped(object sender, EventArgs e)
        {
            var visible = !MaxHPModifiers.IsVisible;
            MaxHPModifiers.IsVisible = visible;
            MaxHPModifiersSumFrame.BackgroundColor = visible ? Color.LightGray : Color.White;
            MaxHPModifiersSum.TextDecorations = visible ? TextDecorations.None : TextDecorations.Underline;
        }

        private void HPModifiersSum_Tapped(object sender, EventArgs e)
        {
            var visible = !TempHPModifiers.IsVisible;
            TempHPModifiers.IsVisible = visible;
            HPModifiersSumFrame.BackgroundColor = visible ? Color.LightGray : Color.White;
            HPModifiersSum.TextDecorations = visible ? TextDecorations.None : TextDecorations.Underline;
        }

        private void DamageResistModifiersSum_Tapped(object sender, EventArgs e)
        {
            var visible = !DamageResistModifiers.IsVisible;
            DamageResistModifiers.IsVisible = visible;
            DamageResistModifiersSumFrame.BackgroundColor = visible ? Color.LightGray : Color.White;
            DamageResistModifiersSum.TextDecorations = visible ? TextDecorations.None : TextDecorations.Underline;
        }

    }
}