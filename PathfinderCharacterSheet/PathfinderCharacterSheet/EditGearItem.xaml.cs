using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemsType = PathfinderCharacterSheet.CharacterSheets.V1.GearItem;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditGearItem : ContentPage, ISheetView
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
        private ItemsType source = null;
        private ItemsType item = null;
        private Page pushedPage = null;

        public EditGearItem()
        {
            InitializeComponent();
        }

        public void InitEditor(ItemsType item = null)
        {
            source = item;
            if (item == null)
                this.item = new ItemsType();
            else
                this.item = item.Clone as ItemsType;
            ItemName.Text = this.item.name;
            Description.Text = this.item.description;
            ItemActive.IsChecked = this.item.active;
            HasUseLimit.IsChecked = this.item.hasUseLimit;
            Delete.IsEnabled = source != null;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            Amount.Text = item.amount.GetValue(sheet).ToString();
            Weight.Text = item.weight.GetValue(sheet).ToString();
            Left.Text = item.useLimit.GetValue(sheet).ToString();
            Total.Text = item.dailyUseLimit.GetValue(sheet).ToString();
            UpdateHasUseLimit();
        }

        private void EditToView()
        {
            if (items == null)
                return;
            item.name = ItemName.Text;
            item.description = Description.Text;
            item.active = ItemActive.IsChecked;
            var hasChanges = false;
            if (source != null)
            {
                if (!item.Equals(source))
                {
                    source.Fill(item);
                    hasChanges = true;
                }
            }
            else
            {
                items.Add(item);
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
            eivwm.Init(sheet, item.amount, "Edit Gear Item", "Amount", false);
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
            eivwm.Init(sheet, item.weight, "Edit Gear Item", "Weight", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Left_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (item == null)
                return;
            if (!item.hasUseLimit)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.useLimit, "Edit Gear Item", "Uses Limit", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Total_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (item == null)
                return;
            if (!item.hasUseLimit)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.dailyUseLimit, "Edit Gear Item", "Daily Uses Limit", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void UpdateHasUseLimit()
        {
            if (item == null)
                return;
            item.hasUseLimit = HasUseLimit.IsChecked;
            Left.TextDecorations = item.hasUseLimit ? TextDecorations.Underline : TextDecorations.None;
            LeftFrame.BackgroundColor = item.hasUseLimit ? Color.White : Color.LightGray;
            Total.TextDecorations = item.hasUseLimit ? TextDecorations.Underline : TextDecorations.None;
            TotalFrame.BackgroundColor = item.hasUseLimit ? Color.White : Color.LightGray;
        }

        private void HasUseLimit_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateHasUseLimit();
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
            var itemName = string.Empty;
            if ((source != null) && !string.IsNullOrWhiteSpace(source.name))
                itemName = " \"" + source.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.Remove(source);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}