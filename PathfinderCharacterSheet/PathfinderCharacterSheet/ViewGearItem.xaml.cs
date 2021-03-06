﻿using System;
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
        Label NameTitle = null;
        Label ActiveTitle = null;
        Label AmountTitle = null;
        Label WeightTitle = null;
        Label LeftTitle = null;
        Label TotalTitle = null;
        Label DescriptionTitle = null;

        public ViewGearItem()
        {
            InitializeComponent();
        }

        private void InitControls()
        {
            if (item == null)
                return;

            GearItemGrid.Children.Clear();

            NameTitle = CreateLabel("Name:");
            var nameFrame = CreateFrame(item.name);

            var row = 0;
            GearItemGrid.Children.Add(NameTitle, 0, row);
            GearItemGrid.Children.Add(nameFrame, 1, row);
            row += 1;

            ActiveTitle = CreateLabel("Active:");
            var activecb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.active,
                IsEnabled = false,
            };

            GearItemGrid.Children.Add(ActiveTitle, 0, row);
            GearItemGrid.Children.Add(activecb, 1, row);
            row += 1;

            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet != null)
            {
                AmountTitle = CreateLabel("Amount:");
                var amountFrame = CreateFrame(item.amount.GetTotal(sheet).ToString());
                MainPage.AddTapHandler(amountFrame, Amount_DoubleTapped, 2);

                GearItemGrid.Children.Add(AmountTitle, 0, row);
                GearItemGrid.Children.Add(amountFrame, 1, row);
                row += 1;

                WeightTitle = CreateLabel("Weight:");
                var weightFrame = CreateFrame(item.weight.GetTotal(sheet).ToString());
                MainPage.AddTapHandler(weightFrame, Weight_DoubleTapped, 2);

                GearItemGrid.Children.Add(WeightTitle, 0, row);
                GearItemGrid.Children.Add(weightFrame, 1, row);
                row += 1;

                if (item.hasUseLimit)
                {
                    var ul = item.useLimit.GetTotal(sheet);
                    LeftTitle = CreateLabel("Use Limit:");
                    var leftFrame = CreateFrame(ul.ToString());

                    GearItemGrid.Children.Add(LeftTitle, 0, row);
                    GearItemGrid.Children.Add(leftFrame, 1, row);
                    row += 1;

                    MainPage.AddTapHandler(leftFrame, Left_DoubleTapped, 2);

                    var dul = item.dailyUseLimit.GetTotal(sheet);
                    if (dul > 0)
                    {
                        TotalTitle = CreateLabel("Daily Use Limit:");
                        var totalFrame = CreateFrame(dul.ToString());

                        GearItemGrid.Children.Add(TotalTitle, 0, row);
                        GearItemGrid.Children.Add(totalFrame, 1, row);
                        row += 1;

                        MainPage.AddTapHandler(totalFrame, Total_DoubleTapped, 2);
                    }
                }
            }

            DescriptionTitle = CreateLabel("Description:");
            var descriptionFrame = CreateFrame(item.description);

            GearItemGrid.Children.Add(DescriptionTitle, 0, 2, row, row + 1);
            row += 1;
            GearItemGrid.Children.Add(descriptionFrame, 0, 2, row, row + 1);
            row += 1;

            if (GearItemGrid.RowDefinitions == null)
                GearItemGrid.RowDefinitions = new RowDefinitionCollection();
            else
                GearItemGrid.RowDefinitions.Clear();
            for (var i = 0; i < row; i++)
                GearItemGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            MainPage.AddTapHandler(GearItemGrid, Edit_Clicked, 2);
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

        private void Amount_DoubleTapped(object sender, EventArgs e)
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

        private void Weight_DoubleTapped(object sender, EventArgs e)
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

        private void Left_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, item.useLimit, "Edit Gear Item", "Use Limit", false);
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
            eivwm.Init(sheet, item.dailyUseLimit, "Edit Gear Item", "Daily Use Limit", false);
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