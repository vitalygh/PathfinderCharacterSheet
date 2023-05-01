#define EXPAND_SELECTED
//#define EXPAND_CHECKBOX
#define EXPAND_WITH_TAP
#define USE_GRID
using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewArmor : ContentPage, ISheetView
    {
#if EXPAND_SELECTED
        public class SelectedArmorGrid : ArmorGrid
        {
            public EventHandler<CheckedChangedEventArgs> activeHandler = null;
            public CheckBox active = null;

            public Label nameTitle = null;

            public Label armorBonusTitle = null;
            public Label armorBonus = null;

            public Label armorTypeTitle = null;
            public Label armorType = null;

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
#endif

        public class ArmorGrid
        {
            public View container = null;
#if EXPAND_SELECTED && EXPAND_CHECKBOX
            public EventHandler<CheckedChangedEventArgs> selectedHandler = null;
            public CheckBox selected = null;
#endif
            public Label name = null;
            public Frame nameFrame = null;
        }

        private Page pushedPage = null;
        private Button armorReorderButton = null;
#if EXPAND_SELECTED
        readonly List<SelectedArmorGrid> selectedArmorGrids = new List<SelectedArmorGrid>();
        readonly List<SelectedArmorGrid> selectedArmorGridsPool = new List<SelectedArmorGrid>();
#endif
        readonly List<ArmorGrid> armorGrids = new List<ArmorGrid>();
        readonly List<ArmorGrid> armorGridsPool = new List<ArmorGrid>();

        public ViewArmor()
        {
            InitializeComponent();
            CreateHeader();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            armorReorderButton.IsVisible = sheet.armorClassItems.Count > 1;
#if EXPAND_SELECTED
            var selectedArmorItems = new List<KeyValuePair<ArmorClassItem, int>>();
#endif
            var armorItems = new List<KeyValuePair<ArmorClassItem, int>>();
            var totalItemsCount = sheet.armorClassItems.Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var wpn = sheet.armorClassItems[i];
                if (wpn == null)
                    continue;
#if EXPAND_SELECTED
                if (wpn.selected)
                    selectedArmorItems.Add(new KeyValuePair<ArmorClassItem, int>(wpn, i));
                else
#endif
                    armorItems.Add(new KeyValuePair<ArmorClassItem, int>(wpn, i));
            }
#if EXPAND_SELECTED
            var selectedItemsCount = selectedArmorItems.Count;
            var selectedGridsCount = selectedArmorGrids.Count;
            var selectedUpdate = Math.Min(selectedGridsCount, selectedItemsCount);
            for (var i = 0; i < selectedUpdate; i++)
                UpdateArmorGrid(selectedArmorGrids[i], selectedArmorItems[i]);
            var selectedCreate = selectedGridsCount < selectedItemsCount;
            var selectedLeft = selectedCreate ? selectedItemsCount - selectedGridsCount : selectedGridsCount - selectedItemsCount;
            for (int i = 0; i < selectedLeft; i++)
            {
                if (selectedCreate)
                    CreateSelectedArmorGrid(selectedArmorItems[i + selectedUpdate]);
                else
                    RemoveArmorGrid(selectedArmorGrids[selectedUpdate]);
            }
#endif
            var itemsCount = armorItems.Count;
            var gridsCount = armorGrids.Count;
            var update = Math.Min(gridsCount, itemsCount);
            for (var i = 0; i < update; i++)
                UpdateArmorGrid(armorGrids[i], armorItems[i]);
            var create = gridsCount < itemsCount;
            var left = create ? itemsCount - gridsCount : gridsCount - itemsCount;
            for (int i = 0; i < left; i++)
            {
                if (create)
                    CreateArmorGrid(armorItems[i + update]);
                else
                    RemoveArmorGrid(armorGrids[update]);
            }
        }


        private void CreateHeader()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
#if USE_GRID
            var armor = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            armor.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Auto },
            };
            armor.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
#else
            var armor = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.LightGray,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
#endif
            armorReorderButton = new Button()
            {
                Text = "Reorder",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
            };
            armorReorderButton.Clicked += Reorder_Clicked;
            var armorTitle = UIHelpers.CreateLabel("Armor", TextAlignment.Center);
            var armorAddButton = new Button()
            {
                Text = "Add",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
            };
            armorAddButton.Clicked += (s, e) => Armor_DoubleTap();
#if USE_GRID
            armor.Children.Add(armorReorderButton, 0, 0);
            armor.Children.Add(armorTitle, 1, 0);
            armor.Children.Add(armorAddButton, 2, 0);
#else
            armor.Children.Add(armorReorderButton);
            armor.Children.Add(armorTitle);
            armor.Children.Add(armorAddButton);
#endif
            Header.Children.Add(armor);
        }

#if EXPAND_SELECTED
        private void RemoveArmorGrid(SelectedArmorGrid armorGrid)
        {
            if (armorGrid == null)
                return;
            Armor.Children.Remove(armorGrid.container);
            selectedArmorGrids.Remove(armorGrid);
            selectedArmorGridsPool.Add(armorGrid);
        }

        private void UpdateArmorGrid(SelectedArmorGrid armorGrid, KeyValuePair<ArmorClassItem, int> kvp)
        {
            UpdateArmorGrid(armorGrid, kvp.Key, kvp.Value);
        }

        private void UpdateArmorGrid(SelectedArmorGrid armorGrid, ArmorClassItem item, int itemIndex)
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;

#if EXPAND_CHECKBOX
            if (armorGrid.selectedHandler != null)
                armorGrid.selected.CheckedChanged -= armorGrid.selectedHandler;
            armorGrid.selectedHandler = (s, e) => ArmorSelected_CheckedChanged(item, e.Value);
            UIHelpers.UpdateValue(armorGrid.selected, item.selected);
            armorGrid.selected.IsChecked = item.selected;
            armorGrid.selected.CheckedChanged += armorGrid.selectedHandler;
#endif

            if (armorGrid.activeHandler != null)
                armorGrid.active.CheckedChanged -= armorGrid.activeHandler;
            armorGrid.activeHandler = (s, e) => ArmorActive_CheckedChanged(item, e.Value);
            UIHelpers.UpdateValue(armorGrid.active, item.active);
            armorGrid.active.IsChecked = item.active;
            armorGrid.active.CheckedChanged += armorGrid.activeHandler;

            UIHelpers.UpdateValue(armorGrid.name, item.name);
            UIHelpers.UpdateValue(armorGrid.armorBonus, item.ArmorBonus(sheet));
            UIHelpers.UpdateValue(armorGrid.armorType, item.armorType);
            UIHelpers.UpdateValue(armorGrid.maxDexBonus, item.MaxDexBonus(sheet));
            UIHelpers.UpdateValue(armorGrid.checkPenalty, item.CheckPenalty(sheet));
            UIHelpers.UpdateValue(armorGrid.spellFailure, item.SpellFailure(sheet));
            UIHelpers.UpdateValue(armorGrid.properties, item.properties);
            UIHelpers.UpdateValue(armorGrid.weight, item.weight.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(armorGrid.description, item.description);

            UIHelpers.SetTapHandler(armorGrid.container, (s, e) => Armor_DoubleTap(item), 2);
#if EXPAND_WITH_TAP
#if EXPAND_CHECKBOX
            UIHelpers.AddTapHandler(armorGrid.container, (s, e) => Armor_Tap(armorGrid.selected), 1);
#else
            UIHelpers.AddTapHandler(armorGrid.container, (s, e) => Armor_Tap(item), 1);
#endif
#endif
        }

        private void CreateSelectedArmorGrid(KeyValuePair<ArmorClassItem, int> kvp)
        {
            CreateSelectedArmorGrid(kvp.Key, kvp.Value);
        }

        private void CreateSelectedArmorGrid(ArmorClassItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (selectedArmorGridsPool.Count > 0)
            {
                var armorGrid = selectedArmorGridsPool[0];
                selectedArmorGridsPool.RemoveAt(0);
                UpdateArmorGrid(armorGrid, item, itemIndex);
                var pos = selectedArmorGrids.Count;
                selectedArmorGrids.Add(armorGrid);
                Armor.Children.Insert(pos, armorGrid.container);
                return;
            }
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
#if EXPAND_CHECKBOX
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => ArmorSelected_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
#endif
            var nameTitle = UIHelpers.CreateLabel("Name:");
            var nameStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
            };
#if EXPAND_CHECKBOX
            nameStack.Children.Add(selectedcb);
#endif
            nameStack.Children.Add(nameTitle);

            var row = 0;
            var nameValue = UIHelpers.CreateFrame(item.name);
            grid.Children.Add(nameStack, 0, row);
            grid.Children.Add(nameValue, 1, row);
            row += 1;

            var activeTitle = UIHelpers.CreateLabel("Active:");
            var activecb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.active,
                IsEnabled = false,
            };
            void activeHandler(object s, CheckedChangedEventArgs e) => ArmorActive_CheckedChanged(item, e.Value);
            activecb.CheckedChanged += activeHandler;
            grid.Children.Add(activeTitle, 0, row);
            grid.Children.Add(activecb, 1, row);
            row += 1;

            var armorBonusTitle = UIHelpers.CreateLabel("Armor Bonus:");
            var armorBonusValue = UIHelpers.CreateFrame(item.ArmorBonus(sheet));
            grid.Children.Add(armorBonusTitle, 0, row);
            grid.Children.Add(armorBonusValue, 1, row);
            row += 1;

            var armorTypeTitle = UIHelpers.CreateLabel("Armor Type:");
            var armorTypeValue = UIHelpers.CreateFrame(item.ArmorType.ToString());
            grid.Children.Add(armorTypeTitle, 0, row);
            grid.Children.Add(armorTypeValue, 1, row);
            row += 1;

            var maxDexBonusTitle = UIHelpers.CreateLabel("Max Dex Bonus:");
            var maxDexBonusValue = UIHelpers.CreateFrame(item.MaxDexBonus(sheet));
            grid.Children.Add(maxDexBonusTitle, 0, row);
            grid.Children.Add(maxDexBonusValue, 1, row);
            row += 1;

            var checkPenaltyTitle = UIHelpers.CreateLabel("Check Penalty:");
            var checkPenaltyValue = UIHelpers.CreateFrame(item.CheckPenalty(sheet));
            grid.Children.Add(checkPenaltyTitle, 0, row);
            grid.Children.Add(checkPenaltyValue, 1, row);
            row += 1;

            var spellFailureTitle = UIHelpers.CreateLabel("Spell Failure:");
            var spellFailureValue = UIHelpers.CreateFrame(item.SpellFailure(sheet));
            grid.Children.Add(spellFailureTitle, 0, row);
            grid.Children.Add(spellFailureValue, 1, row);
            row += 1;

            var propertiesTitle = UIHelpers.CreateLabel("Properties:");
            var propertiesValue = UIHelpers.CreateFrame(item.properties);
            grid.Children.Add(propertiesTitle, 0, row);
            grid.Children.Add(propertiesValue, 1, row);
            row += 1;

            var weightTitle = UIHelpers.CreateLabel("Weight:");
            var weightValue = UIHelpers.CreateFrame(item.weight.GetValue(sheet).ToString());
            grid.Children.Add(weightTitle, 0, row);
            grid.Children.Add(weightValue, 1, row);
            row += 1;

            var descriptionTitle = UIHelpers.CreateLabel("Description:");
            grid.Children.Add(descriptionTitle, 0, 2, row, row + 1);
            row += 1;

            var descriptionValue = UIHelpers.CreateFrame(item.description);
            grid.Children.Add(descriptionValue, 0, 2, row, row + 1);
            row += 1;

            UIHelpers.AddTapHandler(grid, (s, e) => Armor_DoubleTap(item), 2);
#if EXPAND_WITH_TAP
#if EXPAND_CHECKBOX
            UIHelpers.AddTapHandler(grid, (s, e) => Armor_Tap(selectedcb), 1);
#else
            UIHelpers.AddTapHandler(grid, (s, e) => Armor_Tap(item), 1);
#endif
#endif

            var newArmorGrid = new SelectedArmorGrid()
            {
                container = grid,
#if EXPAND_CHECKBOX
                selectedHandler = selectedHandler,
                selected = selectedcb,
#endif
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
            };

            var newpos = selectedArmorGrids.Count;
            selectedArmorGrids.Add(newArmorGrid);
            Armor.Children.Insert(newpos, newArmorGrid.container);
        }
#endif

        private void RemoveArmorGrid(ArmorGrid armorGrid)
        {
            if (armorGrid == null)
                return;
            Armor.Children.Remove(armorGrid.container);
            armorGrids.Remove(armorGrid);
            armorGridsPool.Add(armorGrid);
        }

        private void UpdateArmorGrid(ArmorGrid armorGrid, KeyValuePair<ArmorClassItem, int> kvp)
        {
            UpdateArmorGrid(armorGrid, kvp.Key, kvp.Value);
        }

        private void UpdateArmorGrid(ArmorGrid armorGrid, ArmorClassItem item, int itemIndex)
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            UIHelpers.SetTapHandler(armorGrid.container, (s, e) => Armor_DoubleTap(item), 2);
#if EXPAND_SELECTED
#if EXPAND_CHECKBOX
            if (armorGrid.selectedHandler != null)
                armorGrid.selected.CheckedChanged -= armorGrid.selectedHandler;
            armorGrid.selectedHandler = (s, e) => ArmorSelected_CheckedChanged(item, e.Value);
            UIHelpers.UpdateValue(armorGrid.selected, item.selected);
            armorGrid.selected.CheckedChanged += armorGrid.selectedHandler;
#if EXPAND_WITH_TAP
UIHelpers.AddTapHandler(armorGrid.container, (s, e) => Armor_Tap(armorGrid.selected), 1);
#endif
#else
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(armorGrid.container, (s, e) => Armor_Tap(item), 1);
#endif
#endif
#endif
            UIHelpers.UpdateValue(armorGrid.name, item.AsString(sheet));
            armorGrid.name.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
        }

        private void CreateArmorGrid(KeyValuePair<ArmorClassItem, int> kvp)
        {
            CreateArmorGrid(kvp.Key, kvp.Value);
        }

        private void CreateArmorGrid(ArmorClassItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (armorGridsPool.Count > 0)
            {
                var armorGrid = armorGridsPool[0];
                armorGridsPool.RemoveAt(0);
                UpdateArmorGrid(armorGrid, item, itemIndex);
                var pos =
#if EXPAND_SELECTED
                    selectedArmorGrids.Count +
#endif
                    armorGrids.Count;
                armorGrids.Add(armorGrid);
                Armor.Children.Insert(pos, armorGrid.container);
                return;
            }
#if USE_GRID
            var container = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            container.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
            };
            container.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
#else
            var container = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.LightGray,
            };
#endif
            var armorNameFrame = UIHelpers.CreateFrame(item.AsString(sheet));
            armorNameFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
            var armorName = armorNameFrame.Content as Label;
            armorName.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
            UIHelpers.AddTapHandler(container, (s, e) => Armor_DoubleTap(item), 2);
#if EXPAND_SELECTED
#if EXPAND_CHECKBOX
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => ArmorSelected_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(container, (s, e) => Armor_Tap(selectedcb), 1);
#endif
#if USE_GRID
            container.Children.Add(selectedcb, 0, 0);
#else
            container.Children.Add(selectedcb);
#endif
#else
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(container, (s, e) => Armor_Tap(item), 1);
#endif
#endif
#endif
#if USE_GRID
            container.Children.Add(armorNameFrame, 1, 0);
#else
            container.Children.Add(armorNameFrame);
#endif

            var newArmorGrid = new ArmorGrid()
            {
                container = container,
#if EXPAND_SELECTED && EXPAND_CHECKBOX
                selectedHandler = selectedHandler,
                selected = selectedcb,
#endif
                name = armorName,
                nameFrame = armorNameFrame,
            };

            var newpos =
#if EXPAND_SELECTED
                selectedArmorGrids.Count +
#endif
                armorGrids.Count;
            armorGrids.Add(newArmorGrid);
            Armor.Children.Insert(newpos, newArmorGrid.container);
        }

#if EXPAND_SELECTED
        public void ArmorSelected_CheckedChanged(ArmorClassItem armor, bool value)
        {
            if (armor == null)
                return;
            if (armor.selected == value)
                return;
            armor.selected = value;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }
#if EXPAND_CHECKBOX
        public void Armor_Tap(CheckBox selectedcb)
        {
            selectedcb.IsChecked = !selectedcb.IsChecked;
        }
#else
        public void Armor_Tap(ArmorClassItem armor)
        {
            if (armor == null)
                return;
            armor.selected = !armor.selected;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }
#endif
#endif

        public void ArmorActive_CheckedChanged(ArmorClassItem armor, bool value)
        {
            if (armor == null)
                return;
            if (armor.active == value)
                return;
            armor.active = value;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }

        public void Armor_DoubleTap(ArmorClassItem item = null)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ew = new EditArmor();
            ew.InitEditor(item);
            pushedPage = ew;
            Navigation.PushAsync(pushedPage);
        }

        private void Reorder_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ri = new ReorderItemsWithDescription();
            pushedPage = ri;
            var items = new List<ItemWithDescription>();
            foreach (var item in sheet.armorClassItems)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                sheet.armorClassItems.Clear();
                foreach (var item in reordered)
                    sheet.armorClassItems.Add(item as ArmorClassItem);
                UIMediator.OnCharacterSheetChanged?.Invoke();
            });
            Navigation.PushAsync(pushedPage);
        }
    }
}