﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.SpecialAbility;
using EditItemType = PathfinderCharacterSheet.EditSpecialAbility;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewSpecialAbility : ContentPage, ISheetView
    {
        private ItemType item = null;
        Label NameTitle = null;
        Label LeftTitle = null;
        Label TotalTitle = null;
        Label DescriptionTitle = null;

        private List<ItemType> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.specialAbilities;
                return null;
            }
        }
        private Page pushedPage = null;

        public ViewSpecialAbility()
        {
            InitializeComponent();
        }

        private void InitControls()
        {
            if (item == null)
                return;

            SpecialAbilitiesGrid.Children.Clear();

            NameTitle = CreateLabel("Name:");
            var nameFrame = CreateFrame(item.name);

            var row = 0;
            SpecialAbilitiesGrid.Children.Add(NameTitle, 0, row);
            SpecialAbilitiesGrid.Children.Add(nameFrame, 1, row);
            row += 1;

            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet != null)
            {
                if (item.hasUseLimit)
                {
                    var ul = item.useLimit.GetTotal(sheet);
                    LeftTitle = CreateLabel("Use Left:");
                    var leftFrame = CreateFrame(ul.ToString());

                    SpecialAbilitiesGrid.Children.Add(LeftTitle, 0, row);
                    SpecialAbilitiesGrid.Children.Add(leftFrame, 1, row);
                    row += 1;

                    MainPage.AddTapHandler(leftFrame, Left_DoubleTapped, 2);

                    var dul = item.dailyUseLimit.GetTotal(sheet);
                    if (dul > 0)
                    {
                        TotalTitle = CreateLabel("Daily Use Limit:");
                        var totalFrame = CreateFrame(dul.ToString());

                        SpecialAbilitiesGrid.Children.Add(TotalTitle, 0, row);
                        SpecialAbilitiesGrid.Children.Add(totalFrame, 1, row);
                        row += 1;

                        MainPage.AddTapHandler(totalFrame, Total_DoubleTapped, 2);
                    }
                }
            }

            DescriptionTitle = CreateLabel("Description:");
            var descriptionFrame = CreateFrame(item.description);

            SpecialAbilitiesGrid.Children.Add(DescriptionTitle, 0, 2, row, row + 1);
            row += 1;
            SpecialAbilitiesGrid.Children.Add(descriptionFrame, 0, 2, row, row + 1);
            row += 1;

            if (SpecialAbilitiesGrid.RowDefinitions == null)
                SpecialAbilitiesGrid.RowDefinitions = new RowDefinitionCollection();
            else
                SpecialAbilitiesGrid.RowDefinitions.Clear();
            for (var i = 0; i < row; i++)
                SpecialAbilitiesGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            MainPage.AddTapHandler(SpecialAbilitiesGrid, Edit_Clicked, 2);
        }

        public void InitView(ItemType item)
        {
            this.item = item;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (item == null)
                return;
            if ((items != null) && !items.Contains(item))
            {
                Navigation.PopAsync();
                return;
            }
            InitControls();
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void Left_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.useLimit, "Edit Special Ability", "Use Limit", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Total_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.dailyUseLimit, "Edit Special Ability", "Daily Use Limit", false);
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
            var eit = new EditItemType();
            eit.InitEditor(item);
            pushedPage = eit;
            Navigation.PushAsync(pushedPage);
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (items == null)
                return;
            var itemIndex = items.IndexOf(this.item);
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