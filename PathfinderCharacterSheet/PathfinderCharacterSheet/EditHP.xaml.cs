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
        private List<CharacterSheet.IntModifier> tempHP = null;

		public EditHP ()
		{
			InitializeComponent ();
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            tempHP = new List<CharacterSheet.IntModifier>();
            foreach (var mod in c.hp.tempHP)
                tempHP.Add(new CharacterSheet.IntModifier()
                {
                    active = mod.IsActive,
                    value = mod.Value,
                    name = mod.Name,
                });
            MaxHP.Text = c.hp.maxHP.ToString();
            HP.Text = c.hp.hp.ToString();
            DamageResist.Text = c.hp.damageResist.ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            MainPage.InitIntModifierGrid(TempHPModifiers, tempHP, "Temp HP Modifiers:", EditIntModifier, EditIntModifier);
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var anyChanged = false;
            anyChanged |= MainPage.StrToInt(MaxHP.Text, ref c.hp.maxHP);
            anyChanged |= MainPage.StrToInt(HP.Text, ref c.hp.hp);
            anyChanged |= MainPage.StrToInt(DamageResist.Text, ref c.hp.damageResist);
            if (!CharacterSheet.IsEqual(c.hp.tempHP, tempHP))
            {
                anyChanged = true;
                c.hp.tempHP = tempHP;
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
            page.Init(tempHP, mod);
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
    }
}