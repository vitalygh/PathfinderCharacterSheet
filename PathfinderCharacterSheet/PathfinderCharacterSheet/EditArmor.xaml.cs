using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.ArmorClassItem;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditArmor : ContentPage, ISheetView
	{
        public class ArmorTypePickerItem
        {
            public string Name { get; set; }
            public ItemType.ArmorTypes Value { get; set; }
        }

        private Page pushedPage = null;
        private ItemType source = null;
        private ItemType item = null;
        private List<ItemType> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.armorClassItems;
                return null;
            }
        }

        public EditArmor ()
		{
			InitializeComponent ();
		}

        public void InitEditor(ItemType item = null)
        {
            source = item;
            if (item == null)
                this.item = new ItemType();
            else
                this.item = item.Clone as ItemType;
            var armorTypeIndex = -1;
            var armorTypeValues = Enum.GetValues(typeof(ItemType.ArmorTypes));
            var pickerItems = new List<ArmorTypePickerItem>();
            var armorTypeCounter = -1;
            foreach (var atv in armorTypeValues)
            {
                armorTypeCounter += 1;
                var value = (ItemType.ArmorTypes)atv;
                if (value == this.item.ArmorType)
                    armorTypeIndex = armorTypeCounter;
                pickerItems.Add(new ArmorTypePickerItem()
                {
                    Name = value.ToString(),
                    Value = value,
                });
            }
            ArmorType.ItemsSource = pickerItems;
            ArmorType.SelectedIndex = armorTypeIndex;
            ArmorName.Text = this.item.name;
            ArmorActive.IsChecked = this.item.active;
            LimitMaxDexBonus.IsChecked = this.item.limitMaxDexBonus;
            UpdateMaxDexBonus();
            Properties.Text = this.item.properties;
            Description.Text = this.item.description;
            Delete.IsEnabled = source != null;
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
            ArmorBonus.Text = item.ArmorBonus(sheet);
            MaxDexBonus.Text = item.maxDexBonus.GetTotal(sheet).ToString();
            CheckPenalty.Text = item.CheckPenalty(sheet);
            SpellFailure.Text = item.SpellFailure(sheet);
            Weight.Text = item.weight.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (item == null)
                return;
            item.active = ArmorActive.IsChecked;
            item.name = ArmorName.Text;
            var selectedArmorType = ArmorType.SelectedItem as ArmorTypePickerItem;
            if (selectedArmorType != null)
                item.ArmorType = selectedArmorType.Value;
            item.limitMaxDexBonus = LimitMaxDexBonus.IsChecked;
            item.properties = Properties.Text;
            item.description = Description.Text;
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

        private void ArmorBonus_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.armorBonus, "Edit Armor Bonus", "Armor Bonus", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void MaxDexBonus_Tapped(object sender, EventArgs e)
        {
            if (!LimitMaxDexBonus.IsChecked)
                return;
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.maxDexBonus, "Edit Max Dex Bonus", "Max Dex Bonus", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void CheckPenalty_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.checkPenalty, "Edit Check Penalty", "Check Penalty", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SpellFailure_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.spellFailure, "Edit Spell Failure", "Spell Failure", false);
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
            eivwm.Init(sheet, item.weight, "Edit Armor Weight", "Weight", false);
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
            var itemName = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.name))
                itemName = " \"" + source.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.Remove(source);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }

        private void LimitMaxDexBonus_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateMaxDexBonus();
        }

        private void UpdateMaxDexBonus()
        {
            var lmdb = LimitMaxDexBonus.IsChecked;
            MaxDexBonus.TextDecorations = lmdb ? TextDecorations.Underline : TextDecorations.None;
            MaxDexBonusFrame.BackgroundColor = lmdb ? Color.White : Color.LightGray;
        }
    }
}