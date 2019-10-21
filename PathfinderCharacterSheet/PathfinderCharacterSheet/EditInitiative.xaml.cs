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
    public partial class EditInitiative : ContentPage, ISheetView
    {
        CharacterSheet.ValueWithModifiers<int, CharacterSheet.IntSum> modifiers = null;

        public EditInitiative()
        {
            InitializeComponent();
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            modifiers = c.initiative.miscModifiers.Clone;
            UpdateView();
        }

        public void UpdateView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var dexMod = c.GetAbilityModifier(CharacterSheet.Ability.Dexterity);
            DexModifier.Text = dexMod.ToString();
            var miscMod = modifiers.Total;
            MiscModifiers.Text = miscMod.ToString();
            Total.Text = (dexMod + miscMod).ToString();
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var hasChanges = !c.initiative.miscModifiers.Equals(modifiers);
            if (hasChanges)
            {
                c.initiative.miscModifiers = modifiers;
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
            }
        }

        private void MiscModifiers_Tapped(object sender, EventArgs e)
        {
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(modifiers, "Edit Initiative Misc Modifiers", "Misc Modifiers: ", false);
            Navigation.PushAsync(eivwm);
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