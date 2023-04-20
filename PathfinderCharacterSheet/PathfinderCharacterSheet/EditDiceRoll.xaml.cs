using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.DiceRoll;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDiceRoll : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private List<ItemType> items = null;
        private ItemType source = null;
        private ItemType roll = null;
        private bool saveCharacter = false;

        public EditDiceRoll()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (sheet == null)
                return;
            DiceCount.Text = roll.diceCount.GetValue(sheet).ToString();
            DiceSides.Text = roll.diceSides.GetValue(sheet).ToString();
            Additional.Text = roll.additional.GetValue(sheet).ToString();
            Description.Text = roll.description;
        }

        private void EditToView()
        {
            roll.description = Description.Text;
            var hasChanges = false;
            if (source != null)
            {
                if (!roll.Equals(source))
                {
                    source.Fill(roll);
                    hasChanges = true;
                }
            }
            else if (items != null)
            {
                items.Add(roll);
                hasChanges = true;
            }
            if (hasChanges && saveCharacter)
                CharacterSheetStorage.Instance.SaveCharacter();
        }

        public void Init(CharacterSheet sheet, ItemType roll, List<ItemType> items, bool saveCharacter)
        {
            if (sheet == null)
                return;
            this.sheet = sheet;
            this.items = items;
            this.saveCharacter = saveCharacter;
            source = roll;
            if (roll == null)
                this.roll = new ItemType();
            else
                this.roll = roll.Clone as ItemType;
            Delete.IsEnabled = items != null;
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
            eivwm.Init(sheet, roll.diceCount, "Edit Dice Count", "Dice Count", false);
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
            eivwm.Init(sheet, roll.diceSides, "Edit Dice Sides", "Dice Sides", false);
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
            eivwm.Init(sheet, roll.additional, "Edit Dice Additional", "Dice Additional", false);
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

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (items == null)
                return;
            if (source == null)
                return;
            bool allow = await DisplayAlert("Remove dice roll", "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.Remove(source);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}