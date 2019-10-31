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
	public partial class EditWeapon : ContentPage, ISheetView
	{
        private int itemIndex = -1;
        private CharacterSheet.WeaponItem item = null;
        private Page pushedPage = null;

        public EditWeapon ()
		{
			InitializeComponent ();
        }

        public void InitEditor(CharacterSheet.WeaponItem item = null, int index = -1)
        {
            if (item == null)
            {
                this.item = new CharacterSheet.WeaponItem();
                index = -1;
            }
            else
                this.item = item.Clone as CharacterSheet.WeaponItem;
            itemIndex = index;
            WeaponName.Text = this.item.name;
            WeaponType.Text = this.item.type;
            Special.Text = this.item.special;
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
            AttackBonus.Text = item.AttackBonus(sheet);
            Critical.Text = item.critical.AsString(sheet);
            Damage.Text = item.damage.AsString(sheet);
            DamageBonus.Text = item.DamageBonus(sheet).ToString();
            Range.Text = item.range.GetTotal(sheet).ToString();
            Ammunition.Text = item.ammunition.GetTotal(sheet).ToString();
            Weight.Text = item.weight.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            item.name = WeaponName.Text;
            item.type = WeaponType.Text;
            item.special = Special.Text;
            item.description = Description.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                var sourceItem = sheet.weaponItems[itemIndex];
                if (!item.Equals(sourceItem))
                {
                    sheet.weaponItems[itemIndex] = item;
                    hasChanges = true;
                }
            }
            else
            {
                sheet.weaponItems.Add(item);
                hasChanges = true;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
        }

        private void AttackBonus_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.attackBonus, "Edit Attack Bonus", "Attack Bonus: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Critical_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ec = new EditCritical();
            ec.Init(sheet, item.critical);
            pushedPage = ec;
            Navigation.PushAsync(ec);
        }

        private void Damage_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ed = new EditDiceRoll();
            ed.Init(sheet, item.damage);
            pushedPage = ed;
            Navigation.PushAsync(ed);
        }

        private void DamageBonus_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.damageBonus, "Edit Weapon Damage Bonus", "Damage Bonus: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Range_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.range, "Edit Weapon Range", "Range: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Ammunition_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.ammunition, "Edit Weapon Ammunition", "Ammunition: ", false);
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
            if (itemIndex >= sheet.weaponItems.Count)
                return;
            var item = sheet.weaponItems[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove weapon" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                sheet.weaponItems.RemoveAt(itemIndex);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}