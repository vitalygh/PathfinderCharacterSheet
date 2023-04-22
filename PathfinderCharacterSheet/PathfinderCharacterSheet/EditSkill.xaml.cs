using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;
using ItemsType = PathfinderCharacterSheet.CharacterSheets.V1.SkillRank;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditSkill : ContentPage, ISheetView
	{
        private int itemIndex = -1;
        private List<ItemsType> Items
        {
            get
            {
                var sheet = MainPage.GetSelectedCharacter?.Invoke();
                if (sheet != null)
                    return sheet.skills;
                return null;
            }
        }
        private ItemsType item = null;
        private readonly List<AbilityPickerItem> abilities = new List<AbilityPickerItem>();
        private Page pushedPage = null;

        public EditSkill()
        {
            InitializeComponent();
            InitAbilityPicker();
        }

        private void InitAbilityPicker()
        {
            abilities.Clear();
            var values = Enum.GetValues(typeof(Ability));
            foreach (var v in values)
            {
                var value = (Ability)v;
                if (value == Ability.Total)
                    continue;
                abilities.Add(new AbilityPickerItem()
                {
                    Name = v.ToString(),
                    Value = value,
                });
            }
            AbilityModifierSource.ItemsSource = abilities;
        }

        private void SelectAbilityPickerValue(Ability ability)
        {
            var count = abilities.Count;
            for (var i = 0; i < count; i++)
                if (ability == abilities[i].Value)
                    AbilityModifierSource.SelectedIndex = i;
        }

        private void UpdateItemSourceAbility()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (item == null)
                return;
            if (AbilityModifierSource.SelectedItem is AbilityPickerItem selectedItem)
            {
                item.AbilityModifierSource = selectedItem.Value;
                AbilityModifier.Text = item.GetAbilityModifier(sheet).ToString();
                switch (item.AbilityModifierSource)
                {
                    case Ability.Strength:
                    case Ability.Dexterity:
                        if (!item.armorPenalty)
                            HasArmorPenalty.IsChecked = true;
                        break;
                    default:
                        if (item.armorPenalty)
                            HasArmorPenalty.IsChecked = false;
                        break;
                }
                UpdateTotal();
            }
        }

        public void InitEditor(ItemsType item = null, int index = -1)
        {
            if (item == null)
            {
                this.item = new ItemsType(string.Empty, Ability.None, false, false, true);
                index = -1;
                SelectAbilityPickerValue(this.item.AbilityModifierSource);
            }
            else
            {
                SelectAbilityPickerValue(item.AbilityModifierSource);
                this.item = item.Clone;
            }
            itemIndex = index;

            SkillName.Text = this.item.name;
            Subject.Text = this.item.subject;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            HasSubject.IsChecked = item.hasSubject;
            ClassSkill.IsChecked = item.classSkill;
            Rank.Text = item.rank.GetValue(sheet).ToString();
            AbilityModifier.Text = item.GetAbilityModifier(sheet).ToString();
            MiscModifiers.Text = item.miscModifiers.GetValue(sheet).ToString();
            HasArmorPenalty.IsChecked = item.armorPenalty;
            HasSubject.IsEnabled = item.custom;
            TrainedOnly.IsChecked = item.trainedOnly;
            Custom.IsChecked = item.custom;
            AbilityModifierSource.IsEnabled = item.custom;
            Subject.IsReadOnly = !item.hasSubject;
            HasArmorPenalty.IsEnabled = item.custom;
            TrainedOnly.IsEnabled = item.custom;
            Delete.IsEnabled = item.custom;
            UpdateSkillNameFrame();
            UpdateSubjectFrame();
            UpdateTotal();
        }

        private void UpdateArmorPenalty()
        {
            ArmorPenalty.Text = "-";
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if ((item != null) && item.armorPenalty)
                ArmorPenalty.Text = sheet.CheckPenalty().ToString();
        }

        private void UpdateSkillNameFrame()
        {
            if (item == null)
                return;
            SkillName.IsReadOnly = !item.custom;
            SkillNameFrame.BackgroundColor = SkillName.IsReadOnly ? Color.LightGray : Color.White;
        }

        private void UpdateSubjectFrame()
        {
            if (item == null)
                return;
            Subject.IsReadOnly = !item.hasSubject;
            SubjectFrame.BackgroundColor = Subject.IsReadOnly ? Color.LightGray : Color.White;
        }

        private void UpdateTotal()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            Total.Text = item.GetValue(sheet).ToString();
        }

        private void EditToView()
        {
            if (Items == null)
                return;
            item.name = SkillName.Text;
            item.subject = Subject.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                var sourceItem = Items[itemIndex];
                if (!item.Equals(sourceItem))
                {
                    Items[itemIndex] = item;
                    hasChanges = true;
                }
            }
            else
            {
                Items.Add(item);
                hasChanges = true;
            }
            if (hasChanges)
                MainPage.SaveSelectedCharacter?.Invoke();
        }

        private void Rank_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.rank, "Edit Rank", "Rank", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void MiscModifiers_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.miscModifiers, "Edit Misc Modifiers", "Misc Modifiers", false);
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
            if (Items == null)
                return;
            if (itemIndex < 0)
                return;
            if (itemIndex >= Items.Count)
                return;
            var item = Items[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove skill" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                Items.RemoveAt(itemIndex);
                MainPage.SaveSelectedCharacter?.Invoke();
                await Navigation.PopAsync();
            }
        }

        private void HasSubject_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            item.hasSubject = e.Value;
            UpdateSubjectFrame();
        }

        private void ClassSkill_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            item.classSkill = e.Value;
            UpdateTotal();
        }

        private void Custom_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            item.custom = e.Value;
            UpdateSkillNameFrame();
            HasSubject.IsEnabled = item.custom;
            AbilityModifierSource.IsEnabled = item.custom;
            HasArmorPenalty.IsEnabled = item.custom;
            TrainedOnly.IsEnabled = item.custom;
            Delete.IsEnabled = item.custom;
        }

        private void ArmorPenalty_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            item.armorPenalty = e.Value;
            UpdateArmorPenalty();
            UpdateTotal();
        }

        private void TrainedOnly_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            item.trainedOnly = e.Value;
        }

        private void AbilityModifierSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateItemSourceAbility();
        }
    }
}