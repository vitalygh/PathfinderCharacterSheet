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
    public partial class ViewArmor : ContentPage, ISheetView
    {
        public class SelectedArmorGrid
        {
            public Grid grid = null;
            public EventHandler<CheckedChangedEventArgs> selectedHandler = null;
            public CheckBox selected = null;

            public EventHandler<CheckedChangedEventArgs> activeHandler = null;
            public CheckBox active = null;

            public Label nameTitle = null;
            public Label name = null;

            public Label armorBonusTitle = null;
            public Label armorBonus = null;

            public Label armorTypeTitle = null;
            public Label armorType = null;

            //public Label limitMaxDexBonusTitle = null;
            //public Label limitMaxDexBonus = null;

            public Label maxDexBonusTitle = null;
            public Label maxDexBonus = null;

            public Label checkPenaltyTitle = null;
            public Label checkPenalty = null;

            public Label spellFailureTitle = null;
            public Label spellFailure = null;

            public Label propertiesTitle = null;
            public Label properties = null;

            public Label weightTitle = null;
            public Label weight = null;

            public Label descriptionTitle = null;
            public Label description = null;
        }

        public class ArmorGrid
        {
            public Grid grid = null;
            public EventHandler<CheckedChangedEventArgs> handler = null;
            public CheckBox selected = null;
            public Label text = null;
        }

        private Page pushedPage = null;
        List<SelectedArmorGrid> selectedArmorGrids = new List<SelectedArmorGrid>();
        List<ArmorGrid> armorGrids = new List<ArmorGrid>();

        public ViewArmor()
        {
            InitializeComponent();
            CreateHeaderGrids();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            List<KeyValuePair<CharacterSheet.ArmorClassItem, int>> selectedArmorClassItems = new List<KeyValuePair<CharacterSheet.ArmorClassItem, int>>();
            List<KeyValuePair<CharacterSheet.ArmorClassItem, int>> armorClassItems = new List<KeyValuePair<CharacterSheet.ArmorClassItem, int>>();
            var totalItemsCount = sheet.armorClassItems.Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var arm = sheet.armorClassItems[i];
                if (arm == null)
                    continue;
                if (arm.selected)
                    selectedArmorClassItems.Add(new KeyValuePair<CharacterSheet.ArmorClassItem, int>(arm, i));
                else
                    armorClassItems.Add(new KeyValuePair<CharacterSheet.ArmorClassItem, int>(arm, i));
            }
            var count = armorClassItems.Count;
            var selectedCount = selectedArmorClassItems.Count;
            if ((selectedCount != selectedArmorGrids.Count) || (count != armorGrids.Count))
            {
                foreach (var arm in selectedArmorGrids)
                    Armor.Children.Remove(arm.grid);
                selectedArmorGrids.Clear();
                foreach (var arm in armorGrids)
                    Armor.Children.Remove(arm.grid);
                armorGrids.Clear();
                foreach (var kvp in selectedArmorClassItems)
                    CreateSelectedArmorGrid(kvp.Key, kvp.Value);
                CreateArmorGrid(armorClassItems);
                return;
            }
            for (var i = 0; i < selectedCount; i++)
            {
                var grid = selectedArmorGrids[i];
                var kvp = selectedArmorClassItems[i];
                var arm = kvp.Key;
                var index = kvp.Value;
                if (grid.selectedHandler != null)
                    grid.selected.CheckedChanged -= grid.selectedHandler;
                grid.selectedHandler = (s, e) => ArmorSelected_CheckedChanged(arm, e.Value);
                grid.selected.CheckedChanged += grid.selectedHandler;
                if (grid.activeHandler != null)
                    grid.active.CheckedChanged -= grid.activeHandler;
                grid.activeHandler = (s, e) => ArmorActive_CheckedChanged(arm, e.Value);
                grid.active.CheckedChanged += grid.activeHandler;
                grid.name.Text = arm.name;
                grid.armorBonus.Text = arm.armorBonus.GetTotal(sheet).ToString();
                grid.armorType.Text = arm.armorType.ToString();
                grid.maxDexBonus.Text = arm.limitMaxDexBonus ? arm.maxDexBonus.GetTotal(sheet).ToString() : "-";
                grid.checkPenalty.Text = arm.checkPenalty.GetTotal(sheet).ToString();
                grid.spellFailure.Text = arm.spellFailure.GetTotal(sheet).ToString() + "%";
                grid.properties.Text = arm.properties;
                grid.weight.Text = arm.weight.GetTotal(sheet).ToString();
                grid.description.Text = arm.description;
                grid.grid.GestureRecognizers.Clear();
                MainPage.AddTapHandler(grid.grid, (s, e) => Armor_DoubleTap(arm, index), 2);
            }
            for (var i = 0; i < count; i++)
            {
                var grid = armorGrids[i];
                var kvp = armorClassItems[i];
                var arm = kvp.Key;
                var index = kvp.Value;
                if (grid.handler != null)
                    grid.selected.CheckedChanged -= grid.handler;
                grid.handler = (s, e) => ArmorSelected_CheckedChanged(arm, e.Value);
                grid.selected.CheckedChanged += grid.handler;
                grid.text.Text = arm.AsString(sheet);
                grid.text.FontAttributes = arm.active ? FontAttributes.Bold : FontAttributes.None;
                grid.grid.GestureRecognizers.Clear();
                MainPage.AddTapHandler(grid.grid, (s, e) => Armor_DoubleTap(arm, index), 2);
            }
        }

        private void CreateHeaderGrids()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Auto },
            };
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
            var armorTitle = CreateLabel("Armor:");
            var armorAddButton = new Button()
            {
                Text = "Add",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            armorAddButton.Clicked += (s, e) => Armor_DoubleTap();
            grid.Children.Add(armorTitle, 0, 0);
            grid.Children.Add(armorAddButton, 1, 0);
            Armor.Children.Add(grid);
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Center)
        {
            return new Label()
            {
                Text = text,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                HorizontalTextAlignment = horz,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
        }

        private Frame CreateFrame(string text)
        {
            return new Frame()
            {
                Content = new Label()
                {
                    Text = text,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    HorizontalTextAlignment = TextAlignment.Start,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                BorderColor = Color.Black,
                Padding = 5,
            };
        }

        private void CreateSelectedArmorGrid(CharacterSheet.ArmorClassItem armor, int index)
        {
            if (armor == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
            };
            const int count = 14;
            var rowDefinitions = new RowDefinitionCollection();
            for (var i = 0; i < count; i++)
                rowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions = rowDefinitions;
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = armor.selected,
            };
            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => ArmorSelected_CheckedChanged(armor, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
            var nameTitle = CreateLabel("Name: ", TextAlignment.Start);
            var nameStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            nameStack.Children.Add(selectedcb);
            nameStack.Children.Add(nameTitle);

            var row = 0;
            var nameValue = CreateFrame(armor.name);
            grid.Children.Add(nameStack, 0, row);
            grid.Children.Add(nameValue, 1, row);
            row += 1;

            var activeTitle = CreateLabel("Active: ", TextAlignment.Start);
            var activecb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = armor.active,
            };
            EventHandler<CheckedChangedEventArgs> activeHandler = (s, e) => ArmorSelected_CheckedChanged(armor, e.Value);
            activecb.CheckedChanged += activeHandler;
            grid.Children.Add(activeTitle, 0, row);
            grid.Children.Add(activecb, 1, row);
            row += 1;

            var armorBonusTitle = CreateLabel("Armor Bonus: ", TextAlignment.Start);
            var armorBonusValue = CreateFrame(armor.armorBonus.GetTotal(sheet).ToString());
            grid.Children.Add(armorBonusTitle, 0, row);
            grid.Children.Add(armorBonusValue, 1, row);
            row += 1;

            var armorTypeTitle = CreateLabel("Armor Type: ", TextAlignment.Start);
            var armorTypeValue = CreateFrame(armor.armorType.ToString());
            grid.Children.Add(armorTypeTitle, 0, row);
            grid.Children.Add(armorTypeValue, 1, row);
            row += 1;

            var maxDexBonusTitle = CreateLabel("Max Dex Bonus: ", TextAlignment.Start);
            var maxDexBonusValue = CreateFrame(armor.maxDexBonus.GetTotal(sheet).ToString());
            grid.Children.Add(maxDexBonusTitle, 0, row);
            grid.Children.Add(maxDexBonusValue, 1, row);
            row += 1;

            var checkPenaltyTitle = CreateLabel("Check Penalty: ", TextAlignment.Start);
            var checkPenaltyValue = CreateFrame(armor.checkPenalty.GetTotal(sheet).ToString());
            grid.Children.Add(checkPenaltyTitle, 0, row);
            grid.Children.Add(checkPenaltyValue, 1, row);
            row += 1;

            var spellFailureTitle = CreateLabel("Spell Failure: ", TextAlignment.Start);
            var spellFailureValue = CreateFrame(armor.spellFailure.GetTotal(sheet).ToString());
            grid.Children.Add(spellFailureTitle, 0, row);
            grid.Children.Add(spellFailureValue, 1, row);
            row += 1;

            var propertiesTitle = CreateLabel("Properties: ", TextAlignment.Start);
            var propertiesValue = CreateFrame(armor.properties);
            grid.Children.Add(propertiesTitle, 0, row);
            grid.Children.Add(propertiesValue, 1, row);
            row += 1;

            var weightTitle = CreateLabel("Weight: ", TextAlignment.Start);
            var weightValue = CreateFrame(armor.weight.GetTotal(sheet).ToString());
            grid.Children.Add(weightTitle, 0, row);
            grid.Children.Add(weightValue, 1, row);
            row += 1;

            var descriptionTitle = CreateLabel("Description: ", TextAlignment.Start);
            grid.Children.Add(descriptionTitle, 0, 2, row, row + 1);
            row += 1;

            var descriptionValue = CreateFrame(armor.description);
            grid.Children.Add(descriptionValue, 0, 2, row, row + 1);
            row += 1;

            MainPage.AddTapHandler(grid, (s, e) => Armor_DoubleTap(armor, index), 2);

            Armor.Children.Add(grid);

            selectedArmorGrids.Add(new SelectedArmorGrid()
            {
                grid = grid,
                selectedHandler = selectedHandler,
                selected = selectedcb,
                activeHandler = activeHandler,
                active = activecb,
                nameTitle = nameTitle,
                name = nameValue.Content as Label,
                armorBonusTitle = armorBonusTitle,
                armorBonus = armorBonusValue.Content as Label,
                armorTypeTitle = armorTypeTitle,
                armorType = armorTypeValue.Content as Label,
                maxDexBonusTitle = maxDexBonusTitle,
                maxDexBonus = maxDexBonusValue.Content as Label,
                checkPenaltyTitle = checkPenaltyTitle,
                checkPenalty = checkPenaltyValue.Content as Label,
                spellFailureTitle = spellFailureTitle,
                spellFailure = spellFailureValue.Content as Label,
                propertiesTitle = propertiesTitle,
                properties = propertiesValue.Content as Label,
                weightTitle = weightTitle,
                weight = weightValue.Content as Label,
                descriptionTitle = descriptionTitle,
                description = descriptionValue.Content as Label,
            });
        }

        private void CreateArmorGrid(List<KeyValuePair<CharacterSheet.ArmorClassItem, int>> armorItems)
        {
            if (armorItems == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var count = armorItems.Count;
            if (count <= 0)
                return;
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
            };
            var rowDefinitions = new RowDefinitionCollection();
            for (var i = 0; i < count; i++)
                rowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions = rowDefinitions;
            for (var i = 0; i < count; i++)
            {
                var arm = armorItems[i].Key;
                var index = armorItems[i].Value;
                if (arm == null)
                    continue;
                var selectedcb = new CheckBox()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    IsChecked = arm.selected,
                };
                EventHandler<CheckedChangedEventArgs> handler = (s, e) => ArmorSelected_CheckedChanged(arm, e.Value);
                selectedcb.CheckedChanged += handler;
                var armorNameFrame = new Frame()
                {
                    Content = new Label()
                    {
                        Text = arm.AsString(sheet),
                        FontAttributes = arm.active ? FontAttributes.Bold : FontAttributes.None,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                MainPage.AddTapHandler(armorNameFrame, (s, e) => Armor_DoubleTap(arm, index), 2);
                grid.Children.Add(selectedcb, 0, i);
                grid.Children.Add(armorNameFrame, 1, i);

                armorGrids.Add(new ArmorGrid()
                {
                    grid = grid,
                    handler = handler,
                    selected = selectedcb,
                    text = armorNameFrame.Content as Label,
                });
            }
            Armor.Children.Add(grid);
        }

        public void ArmorSelected_CheckedChanged(CharacterSheet.ArmorClassItem armor, bool value)
        {
            if (armor == null)
                return;
            if (armor.selected == value)
                return;
            armor.selected = value;
            CharacterSheetStorage.Instance.SaveCharacter();
            UpdateView();
        }

        public void ArmorActive_CheckedChanged(CharacterSheet.ArmorClassItem armor, bool value)
        {
            if (armor == null)
                return;
            if (armor.active == value)
                return;
            armor.active = value;
            CharacterSheetStorage.Instance.SaveCharacter();
            UpdateView();
        }

        public void Armor_DoubleTap(CharacterSheet.ArmorClassItem item = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            /*
            var ea = new EditArmor();
            ea.InitEditor(item, index);
            pushedPage = ea;
            Navigation.PushAsync(pushedPage);
            */
        }
    }
}