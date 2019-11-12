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
            MainPage.AddTapHandler(GearItemGrid, Edit_Clicked, 2);
            MainPage.AddTapHandler(AmountFrame, Amount_DoubleTapped, 2);
            MainPage.AddTapHandler(WeightFrame, Weight_DoubleTapped, 2);
        }

        public void InitView(ItemsType gearItem)
        {
            item = gearItem;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (item == null)
                return;
            if ((items != null) && !items.Contains(item))
            {
                Navigation.PopAsync();
                return;
            }
            ItemName.Text = item.name;
            ItemActive.IsChecked = item.active;
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
            egi.InitEditor(item);
            pushedPage = egi;
            Navigation.PushAsync(pushedPage);
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (items == null)
                return;
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.Remove(item);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}