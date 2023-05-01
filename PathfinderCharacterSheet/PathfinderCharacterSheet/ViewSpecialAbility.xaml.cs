using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EditItemType = PathfinderCharacterSheet.EditSpecialAbility;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.SpecialAbility;

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

        public ViewSpecialAbility()
        {
            InitializeComponent();
        }

        private void InitControls()
        {
            if (item == null)
                return;

            SpecialAbilitiesGrid.Children.Clear();

            NameTitle = UIHelpers.CreateLabel("Name:");
            var nameFrame = UIHelpers.CreateFrame(item.name);

            var row = 0;
            SpecialAbilitiesGrid.Children.Add(NameTitle, 0, row);
            SpecialAbilitiesGrid.Children.Add(nameFrame, 1, row);
            row += 1;

            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet != null)
            {
                if (item.hasUseLimit)
                {
                    var ul = item.useLimit.GetValue(sheet);
                    LeftTitle = UIHelpers.CreateLabel("Use Left:");
                    var leftFrame = UIHelpers.CreateFrame(ul.ToString());

                    SpecialAbilitiesGrid.Children.Add(LeftTitle, 0, row);
                    SpecialAbilitiesGrid.Children.Add(leftFrame, 1, row);
                    row += 1;

                    UIHelpers.AddTapHandler(leftFrame, Left_DoubleTapped, 2);

                    var dul = item.dailyUseLimit.GetValue(sheet);
                    if (dul > 0)
                    {
                        TotalTitle = UIHelpers.CreateLabel("Daily Use Limit:");
                        var totalFrame = UIHelpers.CreateFrame(dul.ToString());

                        SpecialAbilitiesGrid.Children.Add(TotalTitle, 0, row);
                        SpecialAbilitiesGrid.Children.Add(totalFrame, 1, row);
                        row += 1;

                        UIHelpers.AddTapHandler(totalFrame, Total_DoubleTapped, 2);
                    }
                }
            }

            DescriptionTitle = UIHelpers.CreateLabel("Description:");
            var descriptionFrame = UIHelpers.CreateFrame(item.description);

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

            UIHelpers.AddTapHandler(SpecialAbilitiesGrid, Edit_Clicked, 2);
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
            if ((Items != null) && !Items.Contains(item))
            {
                Navigation.PopAsync();
                return;
            }
            InitControls();
        }

        private void Left_DoubleTapped(object sender, EventArgs e)
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

        private void Total_DoubleTapped(object sender, EventArgs e)
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
            if (Items == null)
                return;
            var itemIndex = Items.IndexOf(this.item);
            if (itemIndex < 0)
                return;
            if (itemIndex >= Items.Count)
                return;
            var item = Items[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                Items.RemoveAt(itemIndex);
                UIMediator.OnCharacterSheetChanged?.Invoke();
                await Navigation.PopAsync();
            }
        }
    }
}