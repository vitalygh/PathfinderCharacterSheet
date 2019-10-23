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
	public partial class EditDiceRoll : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private CharacterSheet.DiceRoll source = null;
        private CharacterSheet.DiceRoll roll = null;

        public EditDiceRoll()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (sheet == null)
                return;
            DiceCount.Text = roll.diceCount.GetTotal(sheet).ToString();
            DiceSides.Text = roll.diceSides.GetTotal(sheet).ToString();
            Additional.Text = roll.additional.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            if (!roll.Equals(source))
            {
                source.Fill(roll);
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        public void Init(CharacterSheet sheet, CharacterSheet.DiceRoll roll)
        {
            if (sheet == null)
                return;
            if (roll == null)
                return;
            this.sheet = sheet;
            source = roll;
            this.roll = roll.Clone as CharacterSheet.DiceRoll;
            UpdateView();
        }

        private void DiceCount_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (sheet == null)
                return;
            if (roll == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, roll.diceCount, "Edit Dice Count", "Dice Count: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DiceSides_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (sheet == null)
                return;
            if (roll == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, roll.diceSides, "Edit Dice Sides", "Dice Sides: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Additional_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (sheet == null)
                return;
            if (roll == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, roll.additional, "Edit Dice Additional", "Dice Additional: ", false);
            pushedPage = eivwm;
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