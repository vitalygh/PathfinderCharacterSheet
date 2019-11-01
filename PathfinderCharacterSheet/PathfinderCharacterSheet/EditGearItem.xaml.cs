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
	public partial class EditGearItem : ContentPage, ISheetView
    {
        private int itemIndex = -1;
        private CharacterSheet.GearItem item = null;
        private Page pushedPage = null;

        public EditGearItem()
        {
            InitializeComponent();
        }

        public void InitEditor(CharacterSheet.GearItem item = null, int index = -1)
        {
            if (item == null)
            {
                this.item = new CharacterSheet.GearItem();
                index = -1;
            }
            else
                this.item = item.Clone as CharacterSheet.GearItem;
            itemIndex = index;
            ItemName.Text = this.item.name;
            Description.Text = this.item.description;
            Delete.IsEnabled = index >= 0;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            Amount.Text = item.amount.GetTotal(sheet).ToString();
            Weight.Text = item.weight.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            item.name = ItemName.Text;
            item.description = Description.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                var sourceItem = sheet.gear[itemIndex];
                if (!item.Equals(sourceItem))
                {
                    sheet.gear[itemIndex] = item;
                    hasChanges = true;
                }
            }
            else
            {
                sheet.gear.Add(item);
                hasChanges = true;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
        }

        private void Amount_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.amount, "Edit Amount", "Amount: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Weight_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.weight, "Edit Weapon Weight", "Weight: ", false);
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

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (itemIndex < 0)
                return;
            if (itemIndex >= sheet.gear.Count)
                return;
            var item = sheet.gear[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                sheet.gear.RemoveAt(itemIndex);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}