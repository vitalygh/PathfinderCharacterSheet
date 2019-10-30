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
	public partial class EditArmor : ContentPage, ISheetView
	{
        public class ArmorTypePickerItem
        {
            public string Name { get; set; }
            public CharacterSheet.ArmorClassItem.ArmorType Value { get; set; }
        }

        private int itemIndex = -1;
        private CharacterSheet.ArmorClassItem item = null;
        private Page pushedPage = null;

        public EditArmor ()
		{
			InitializeComponent ();
		}

        public void InitEditor(CharacterSheet.ArmorClassItem item = null, int index = -1)
        {
            if (item == null)
            {
                item = new CharacterSheet.ArmorClassItem();
                index = -1;
            }
            itemIndex = index;
            this.item = item;
            var armorTypeIndex = -1;
            var armorTypeValues = Enum.GetValues(typeof(CharacterSheet.ArmorClassItem.ArmorType));
            var pickerItems = new List<ArmorTypePickerItem>();
            var armorTypeCounter = -1;
            foreach (var atv in armorTypeValues)
            {
                armorTypeCounter += 1;
                var value = (CharacterSheet.ArmorClassItem.ArmorType)atv;
                if (value == item.armorType)
                    armorTypeIndex = armorTypeCounter;
                pickerItems.Add(new ArmorTypePickerItem()
                {
                    Name = value.ToString(),
                    Value = value,
                });
            }
            ArmorType.ItemsSource = pickerItems;
            ArmorType.SelectedIndex = armorTypeIndex;
            ArmorName.Text = item.name;
            ArmorActive.IsChecked = item.active;
            LimitMaxDexBonus.IsChecked = item.limitMaxDexBonus;
            UpdateMaxDexBonus();
            Properties.Text = item.properties;
            Description.Text = item.description;
            Delete.IsEnabled = index >= 0;
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
                item.armorType = selectedArmorType.Value;
            item.limitMaxDexBonus = LimitMaxDexBonus.IsChecked;
            item.properties = Properties.Text;
            item.description = Description.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                var sourceItem = sheet.armorClassItems[itemIndex];
                if (!item.Equals(sourceItem))
                {
                    sheet.armorClassItems[itemIndex] = item;
                    hasChanges = true;
                }
            }
            else
            {
                sheet.armorClassItems.Add(item);
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
            eivwm.Init(sheet, item.armorBonus, "Edit Armor Bonus", "Armor Bonus: ", false);
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
            eivwm.Init(sheet, item.maxDexBonus, "Edit Max Dex Bonus", "Max Dex Bonus: ", false);
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
            eivwm.Init(sheet, item.checkPenalty, "Edit Check Penalty", "Check Penalty: ", false);
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
            eivwm.Init(sheet, item.spellFailure, "Edit Spell Failure", "Spell Failure: ", false);
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
            eivwm.Init(sheet, item.weight, "Edit Armor Weight", "Weight: ", false);
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
            if (itemIndex >= sheet.armorClassItems.Count)
                return;
            var item = sheet.armorClassItems[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                sheet.weaponItems.RemoveAt(itemIndex);
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