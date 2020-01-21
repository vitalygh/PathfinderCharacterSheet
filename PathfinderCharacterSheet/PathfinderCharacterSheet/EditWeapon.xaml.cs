using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.WeaponItem;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditWeapon : ContentPage, ISheetView
	{
        private CharacterSheet.WeaponItem source = null;
        private CharacterSheet.WeaponItem item = null;
        private List<ItemType> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.weaponItems;
                return null;
            }
        }

        private Page pushedPage = null;

        public EditWeapon ()
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
            ArmorActive.IsChecked = this.item.active;
            WeaponName.Text = this.item.name;
            WeaponType.Text = this.item.type;
            Special.Text = this.item.special;
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
            AttackBonus.Text = item.AttackBonus(sheet);
            Critical.Text = item.critical.AsString(sheet);
            Damage.Text = item.Damage(sheet);
            DamageBonus.Text = item.DamageBonus(sheet).ToString();
            Range.Text = item.range.GetTotal(sheet).ToString();
            Ammunition.Text = item.ammunition.GetTotal(sheet).ToString();
            Weight.Text = item.weight.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            if (items == null)
                return;
            item.active = ArmorActive.IsChecked;
            item.name = WeaponName.Text;
            item.type = WeaponType.Text;
            item.special = Special.Text;
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

        private void AttackBonus_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.attackBonus, "Edit Attack Bonus", "Attack Bonus", false);
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
            var ed = new EditDamage();
            ed.Init(sheet, item.damageRolls);
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
            eivwm.Init(sheet, item.damageBonus, "Edit Weapon Damage Bonus", "Damage Bonus", false);
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
            eivwm.Init(sheet, item.range, "Edit Weapon Range", "Range", false);
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
            eivwm.Init(sheet, item.ammunition, "Edit Weapon Ammunition", "Ammunition", false);
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
            eivwm.Init(sheet, item.weight, "Edit Weapon Weight", "Weight", false);
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
            bool allow = await DisplayAlert("Remove weapon" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.Remove(source);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}