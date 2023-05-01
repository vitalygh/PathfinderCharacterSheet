using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditInitiative : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private readonly ValueWithIntModifiers modifiers = null;

        public EditInitiative()
        {
            InitializeComponent();
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            modifiers = sheet.initiative.miscModifiers.Clone;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            var dexMod = sheet.GetAbilityModifier(Ability.Dexterity);
            DexModifier.Text = dexMod.ToString();
            var miscMod = modifiers.GetValue(sheet);
            MiscModifiers.Text = miscMod.ToString();
            Total.Text = (dexMod + miscMod).ToString();
        }

        private void EditToView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            var hasChanges = !sheet.initiative.miscModifiers.Equals(modifiers);
            if (hasChanges)
            {
                sheet.initiative.miscModifiers.Fill(modifiers);
                UIMediator.OnCharacterSheetChanged?.Invoke();
            }
        }

        private void MiscModifiers_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, modifiers, "Edit Initiative Misc Modifiers", "Misc Modifiers", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
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