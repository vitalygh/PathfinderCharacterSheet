﻿using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.SpecialAbility;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSpecialAbility : ContentPage, ISheetView
    {
        private static List<ItemType> Items
        {
            get
            {
                var sheet = UIMediator.GetSelectedCharacter?.Invoke();
                if (sheet != null)
                    return sheet.specialAbilities;
                return null;
            }
        }
        private Page pushedPage = null;
        private ItemType source = null;
        private ItemType item = null;

        public EditSpecialAbility()
        {
            InitializeComponent();
        }

        public void InitEditor(ItemType item = null)
        {
            pushedPage = null;
            source = item;
            if (item == null)
                this.item = new ItemType
                {
                    uid = UIMediator.GetUID?.Invoke() ?? CharacterSheet.InvalidUID
                };
            else
                this.item = item.Clone as ItemType;
            ItemName.Text = this.item.name;
            Description.Text = this.item.description;
            HasUseLimit.IsChecked = this.item.hasUseLimit;
            Delete.IsEnabled = source != null;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            Left.Text = item.useLimit.GetValue(sheet).ToString();
            Total.Text = item.dailyUseLimit.GetValue(sheet).ToString();
            UpdateHasUseLimit();
        }

        private void EditToView()
        {
            if (Items == null)
                return;
            item.name = ItemName.Text;
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
                Items.Add(item);
                hasChanges = true;
            }
            if (hasChanges)
                UIMediator.OnCharacterSheetChanged?.Invoke();
        }

        private void Left_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.useLimit, "Edit Special Ability", "Use Limit", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Total_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.dailyUseLimit, "Edit Special Ability", "Daily Use Limit", false);
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
            if (Items == null)
                return;
            if (source == null)
                return;
            var itemName = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                Items.Remove(source);
                UIMediator.OnCharacterSheetChanged?.Invoke();
                await Navigation.PopAsync();
            }
        }
    }
}