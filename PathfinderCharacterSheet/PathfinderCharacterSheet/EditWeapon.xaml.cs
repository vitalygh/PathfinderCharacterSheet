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
                item = new CharacterSheet.WeaponItem();
                index = -1;
            }
            itemIndex = index;
            this.item = item;
            WeaponName.Text = item.name;
            WeaponType.Text = item.type;
            Special.Text = item.special;
            Description.Text = item.description;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            AttackBonus.Text = item.attackBonus.GetTotal(sheet).ToString();
            Critical.Text = item.critical.AsString(sheet);
            Damage.Text = item.damage.AsString(sheet);
            DamageBonus.Text = item.damageBonus.GetTotal(sheet).ToString();
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
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private void AttackBonus_Tapped(object sender, EventArgs e)
        {
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditCritical();
            eivwm.Init(sheet, item.critical);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Damage_Tapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditDiceRoll();
            eivwm.Init(sheet, item.damage);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DamageBonus_Tapped(object sender, EventArgs e)
        {
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

        private void Delete_Clicked(object sender, EventArgs e)
        {
            if (itemIndex >= 0)
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                {
                    sheet.weaponItems.RemoveAt(itemIndex);
                    CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
                }
            }
            Navigation.PopAsync();
        }
    }
}