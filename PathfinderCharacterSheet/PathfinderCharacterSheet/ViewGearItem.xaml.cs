using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemsType = PathfinderCharacterSheet.CharacterSheet.GearItem;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewGearItem : ContentPage, ISheetView
    {
        private int itemIndex = -1;
        private List<ItemsType> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.gear;
                return null;
            }
        }
        private ItemsType item = null;
        private Page pushedPage = null;

        public ViewGearItem()
        {
            InitializeComponent();
        }

        public void InitView(ItemsType item, int index)
        {
            this.item = item.Clone as ItemsType;
            itemIndex = index;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            ItemName.Text = item.name;
            Amount.Text = item.amount.GetTotal(sheet).ToString();
            Weight.Text = item.weight.GetTotal(sheet).ToString();
            Description.Text = item.description;
        }

        private void Amount_DoubleTapped(object sender, EventArgs e)
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

        private void Weight_DoubleTapped(object sender, EventArgs e)
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

        private void Back_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var egi = new EditGearItem();
            egi.InitEditor(item, itemIndex);
            pushedPage = egi;
            Navigation.PushAsync(pushedPage);
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (items == null)
                return;
            if (itemIndex < 0)
                return;
            if (itemIndex >= items.Count)
                return;
            var item = items[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.RemoveAt(itemIndex);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}