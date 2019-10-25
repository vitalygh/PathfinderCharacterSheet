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
        private CharacterSheet sheet = null;
        private CharacterSheet.ValueWithIntModifiers maxHP = null;
        private CharacterSheet.ValueWithIntModifiers hp = null;
        private CharacterSheet.ValueWithIntModifiers damageResist = null;

        public EditHP ()
		{
			InitializeComponent ();
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            sheet = CharacterSheetStorage.Instance.selectedCharacter;
            maxHP = sheet.hp.maxHP.Clone as CharacterSheet.ValueWithIntModifiers;
            hp = sheet.hp.hp.Clone as CharacterSheet.ValueWithIntModifiers;
            damageResist = sheet.hp.damageResist.Clone as CharacterSheet.ValueWithIntModifiers;
            MaxHP.Text = sheet.hp.maxHP.GetTotal(sheet).ToString();
            HP.Text = sheet.hp.hp.GetTotal(sheet).ToString();
            DamageResist.Text = sheet.hp.damageResist.GetTotal(sheet).ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            UpdateMaxHP();
            UpdateTempHP();
            UpdateDamageResist();
            //MainPage.FillIntModifierGrid(MaxHPModifiers, maxHPModifiers, "Max HP Modifiers:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateMaxHP());
            //MainPage.FillIntModifierGrid(TempHPModifiers, tempHPModifiers, "Temp HP Modifiers:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateTempHP());
            //MainPage.FillIntModifierGrid(DamageResistModifiers, damageResistModifiers, "Damage Resist Modifiers:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateDamageResist());
        }

        private void UpdateMaxHP()
        {
            MaxHPModifiersSum.Text = maxHP.modifiers.GetTotal(sheet).ToString();
            MaxHPTotal.Text = maxHP.GetTotal(sheet).ToString();
        }

        private void UpdateTempHP()
        {
            HPModifiersSum.Text = hp.modifiers.GetTotal(sheet).ToString();
            HPTotal.Text = hp.GetTotal(sheet).ToString();
        }

        private void UpdateDamageResist()
        {
            DamageResistModifiersSum.Text = damageResist.modifiers.GetTotal(sheet).ToString();
            DamageResistTotal.Text = damageResist.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var anyChanged = false;
            if (!maxHP.Equals(c.hp.maxHP))
            {
                anyChanged = true;
                c.hp.maxHP = maxHP;
            }
            if (!hp.Equals(c.hp.hp))
            {
                anyChanged = true;
                c.hp.hp = hp;
            }
            if (!damageResist.Equals(c.hp.damageResist))
            {
                anyChanged = true;
                c.hp.damageResist = damageResist;
            }
            if (anyChanged)
                CharacterSheetStorage.Instance.SaveCharacter();
        }

        private void EditIntModifier(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers)
        {
            EditIntModifier(modifiers, null);
        }

        private void EditIntModifier(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers, CharacterSheet.IntModifier mod)
        {
            var page = new EditIntModifier();
            page.Init(sheet, modifiers, mod);
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