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
        private CharacterSheet.IntVWM maxHP = null;
        private CharacterSheet.IntVWM hp = null;
        private CharacterSheet.IntVWM damageResist = null;

        public EditHP ()
		{
			InitializeComponent ();
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            maxHP = c.hp.maxHP.Clone as CharacterSheet.IntVWM;
            hp = c.hp.hp.Clone as CharacterSheet.IntVWM;
            damageResist = c.hp.damageResist.Clone as CharacterSheet.IntVWM;
            MaxHP.Text = c.hp.maxHP.Total.ToString();
            HP.Text = c.hp.hp.Total.ToString();
            DamageResist.Text = c.hp.damageResist.Total.ToString();
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
            MaxHPModifiersSum.Text = maxHP.modifiers.Sum.ToString();
            MaxHPTotal.Text = maxHP.Total.ToString();
        }

        private void UpdateTempHP()
        {
            HPModifiersSum.Text = hp.modifiers.Sum.ToString();
            HPTotal.Text = hp.Total.ToString();
        }

        private void UpdateDamageResist()
        {
            DamageResistModifiersSum.Text = damageResist.modifiers.Sum.ToString();
            DamageResistTotal.Text = damageResist.Total.ToString();
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
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private void EditIntModifier(CharacterSheet.IntML modifiers)
        {
            EditIntModifier(modifiers, null);
        }

        private void EditIntModifier(CharacterSheet.IntML modifiers, CharacterSheet.Modifier<int> mod)
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