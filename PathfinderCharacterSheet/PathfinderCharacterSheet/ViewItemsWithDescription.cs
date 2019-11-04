﻿//#define EXPAND_SELECTED
//#define USE_GRID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PathfinderCharacterSheet
{
    public partial class ViewItemsWithDecription<ItemType> where ItemType: CharacterSheet.ItemWithDescription
    {
#if EXPAND_SELECTED
        public class SelectedItemGrid : ItemGrid
        {
            public Label nameTitle = null;
            public Label descriptionTitle = null;
            public Label description = null;
        }
#endif

        public class ItemGrid
        {
            public View container = null;
#if EXPAND_SELECTED
            public EventHandler<CheckedChangedEventArgs> selectedHandler = null;
            public CheckBox selected = null;
#endif
            public Label name = null;
            public EventHandler viewButtonHandler = null;
            public Button viewButton = null;
        }

        public Action<ItemType, int> actEditItem = null;
        public Action<ItemType, int> actViewItem = null;
        public StackLayout layout = null;
        public Func<List<ItemType>> items = null;

#if EXPAND_SELECTED
        private List<SelectedItemGrid> selectedItemGridsPool = new List<SelectedItemGrid>();
#endif
        private List<ItemGrid> itemGridsPool = new List<ItemGrid>();
        private List<ItemGrid> itemGrids = new List<ItemGrid>();

        public ViewItemsWithDecription()
        {

        }

        public void UpdateItemsView()
        {
            if ((items == null) || (items() == null))
                return;
            var totalItemsCount = items().Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var item = items()[i];
                if (itemGrids.Count <= i)
                {
                    var newItemGrid =
#if EXPAND_SELECTED
                        item.selected ? CreateSelectedItemGrid(item, i) :
#endif
                        CreateItemGrid(item, i);
                    itemGrids.Add(newItemGrid);
                    if (layout != null)
                        layout.Children.Add(newItemGrid.container);
                    continue;
                }
                var itemGrid = itemGrids[i];
#if EXPAND_SELECTED
                var selectedItemGrid = itemGrid as SelectedItemGrid;
                if (item.selected)
                {
                    if (selectedItemGrid != null)
                    {
                        UpdateItemGrid(selectedItemGrid, item, i);
                        continue;
                    }
                    RemoveItemGrid(itemGrid);
                    var newItemGrid = CreateSelectedItemGrid(item, i);
                    itemGrids.Insert(i, newItemGrid);
                    if (layout != null)
                        layout.Children.Insert(i, newItemGrid.container);
                    continue;
                }
                if (selectedItemGrid == null)
                {
#endif
                    UpdateItemGrid(itemGrid, item, i);
                    continue;
#if EXPAND_SELECTED
                }
                RemoveItemGrid(itemGrid);
                itemGrid = CreateItemGrid(item, i);
                itemGrids.Insert(i, itemGrid);
                if (layout != null)
                    layout.Children.Insert(i, itemGrid.container);
#endif
            }
            while (itemGrids.Count > items().Count)
                RemoveItemGrid(itemGrids[itemGrids.Count - 1]);
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Center)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

#if EXPAND_SELECTED
        private void RemoveItemGrid(SelectedItemGrid itemGrid)
        {
            if (itemGrid == null)
                return;
            if (layout != null)
                layout.Children.Remove(itemGrid.container);
            itemGrids.Remove(itemGrid);
            selectedItemGridsPool.Add(itemGrid);
        }

        private void UpdateItemGrid(SelectedItemGrid itemGrid, KeyValuePair<ItemType, int> kvp)
        {
            UpdateItemGrid(itemGrid, kvp.Key, kvp.Value);
        }

        private void UpdateItemGrid(SelectedItemGrid itemGrid, ItemType item, int itemIndex)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;

            if (itemGrid.selectedHandler != null)
                itemGrid.selected.CheckedChanged -= itemGrid.selectedHandler;
            itemGrid.selectedHandler = (s, e) => Item_CheckedChanged(item, e.Value);
            itemGrid.selected.IsChecked = item.selected;
            itemGrid.selected.CheckedChanged += itemGrid.selectedHandler;

            itemGrid.name.Text = item.name;
            itemGrid.description.Text = item.description;

            if (itemGrid.viewButtonHandler != null)
                itemGrid.viewButton.Clicked -= itemGrid.viewButtonHandler;
            itemGrid.viewButtonHandler = (s, e) => ItemViewButton_Tap(item, itemIndex);
            itemGrid.viewButton.Clicked += itemGrid.viewButtonHandler;

            itemGrid.container.GestureRecognizers.Clear();
            MainPage.AddTapHandler(itemGrid.container, (s, e) => Item_Tap(itemGrid.selected), 1);
            MainPage.AddTapHandler(itemGrid.container, (s, e) => Item_DoubleTap(item, itemIndex), 2);
        }

        private void CreateSelectedItemGrid(KeyValuePair<ItemType, int> kvp)
        {
            CreateSelectedItemGrid(kvp.Key, kvp.Value);
        }

        private SelectedItemGrid CreateSelectedItemGrid(ItemType item, int itemIndex)
        {
            if (item == null)
                return null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return null;
            if (selectedItemGridsPool.Count > 0)
            {
                var itemGrid = selectedItemGridsPool[0];
                selectedItemGridsPool.RemoveAt(0);
                UpdateItemGrid(itemGrid, item, itemIndex);
                return itemGrid;
            }
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };

            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => Item_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
            var nameTitle = CreateLabel("Name: ", TextAlignment.Start);
            var nameTitleStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            nameTitleStack.Children.Add(selectedcb);
            nameTitleStack.Children.Add(nameTitle);

            var nameValueStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            var nameValue = CreateFrame(item.name);
            var viewButton = new Button()
            {
                Text = "View",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            EventHandler viewButtonHandler = (s, e) => ItemViewButton_Tap(item, itemIndex);
            viewButton.Clicked += viewButtonHandler;
            nameValueStack.Children.Add(nameValue);
            nameValueStack.Children.Add(viewButton);

            var descriptionTitle = CreateLabel("Description: ", TextAlignment.Start);
            var descriptionValue = CreateFrame(item.description);

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
            const int count = 14;
            var rowDefinitions = new RowDefinitionCollection();
            for (var i = 0; i < count; i++)
                rowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            container.RowDefinitions = rowDefinitions;

            var row = 0;
            container.Children.Add(nameTitleStack, 0, row);
            container.Children.Add(nameValueStack, 1, row);
            row += 1;

            container.Children.Add(descriptionTitle, 0, 2, row, row + 1);
            row += 1;

            container.Children.Add(descriptionValue, 0, 2, row, row + 1);
            row += 1;
#else
            var container = new StackLayout()
            {
                BackgroundColor = Color.LightGray,
            };
            var nameStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            nameStack.Children.Add(nameTitleStack);
            nameStack.Children.Add(nameValueStack);
            container.Children.Add(nameStack);
            container.Children.Add(descriptionTitle);
            container.Children.Add(descriptionValue);
#endif

            MainPage.AddTapHandler(container, (s, e) => Item_Tap(selectedcb), 1);
            MainPage.AddTapHandler(container, (s, e) => Item_DoubleTap(item, itemIndex), 2);

            var newItemGrid = new SelectedItemGrid()
            {
                container = container,
                selectedHandler = selectedHandler,
                selected = selectedcb,
                viewButtonHandler = viewButtonHandler,
                viewButton = viewButton,
                nameTitle = nameTitle,
                name = nameValue.Content as Label,
                descriptionTitle = descriptionTitle,
                description = descriptionValue.Content as Label,
            };
            return newItemGrid;
        }
#endif

        private void RemoveItemGrid(ItemGrid itemGrid)
        {
            if (itemGrid == null)
                return;
#if EXPAND_SELECTED
            var sgig = itemGrid as SelectedItemGrid;
            if (sgig != null)
            {
                RemoveItemGrid(sgig);
                return;
            }
#endif
            if (layout != null)
                layout.Children.Remove(itemGrid.container);
            itemGrids.Remove(itemGrid);
            itemGridsPool.Add(itemGrid);
        }

        private void UpdateItemGrid(ItemGrid itemGrid, KeyValuePair<ItemType, int> kvp)
        {
            UpdateItemGrid(itemGrid, kvp.Key, kvp.Value);
        }

        private void UpdateItemGrid(ItemGrid itemGrid, ItemType item, int itemIndex)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            itemGrid.container.GestureRecognizers.Clear();
#if EXPAND_SELECTED
            if (itemGrid.selectedHandler != null)
                itemGrid.selected.CheckedChanged -= itemGrid.selectedHandler;
            itemGrid.selectedHandler = (s, e) => Item_CheckedChanged(item, e.Value);
            itemGrid.selected.IsChecked = item.selected;
            itemGrid.selected.CheckedChanged += itemGrid.selectedHandler;
            MainPage.AddTapHandler(itemGrid.container, (s, e) => Item_Tap(itemGrid.selected), 1);
#endif
            itemGrid.name.Text = item.AsString(sheet);
            if (itemGrid.viewButtonHandler != null)
                itemGrid.viewButton.Clicked -= itemGrid.viewButtonHandler;
            itemGrid.viewButtonHandler = (s, e) => ItemViewButton_Tap(item, itemIndex);
            itemGrid.viewButton.Clicked += itemGrid.viewButtonHandler;
            MainPage.AddTapHandler(itemGrid.container, (s, e) => Item_DoubleTap(item, itemIndex), 2);
        }

        private void CreateItemGrid(KeyValuePair<ItemType, int> kvp)
        {
            CreateItemGrid(kvp.Key, kvp.Value);
        }

        private ItemGrid CreateItemGrid(ItemType item, int itemIndex)
        {
            if (item == null)
                return null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return null;
            if (itemGridsPool.Count > 0)
            {
                var itemGrid = itemGridsPool[0];
                itemGridsPool.RemoveAt(0);
                UpdateItemGrid(itemGrid, item, itemIndex);
                return itemGrid;
            }
#if EXPAND_SELECTED
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => Item_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
#endif
            var itemNameFrame = CreateFrame(item.AsString(sheet));
            var viewButton = new Button()
            {
                Text = "View",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            EventHandler viewButtonHandler = (s, e) => ItemViewButton_Tap(item, itemIndex);
            viewButton.Clicked += viewButtonHandler;
#if USE_GRID
            var container = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            container.ColumnDefinitions = new ColumnDefinitionCollection()
            {
#if EXPAND_SELECTED
                new ColumnDefinition() { Width = GridLength.Auto },
#endif
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Auto },
            };
            container.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
            var column = 0;
#if EXPAND_SELECTED
            container.Children.Add(selectedcb, column++, 0);
#endif
            container.Children.Add(itemNameFrame, column++, 0);
            container.Children.Add(viewButton, column++, 0);
#else
            var container = new StackLayout()
            {
                BackgroundColor = Color.LightGray,
                Orientation = StackOrientation.Horizontal,
            };
#if EXPAND_SELECTED
            container.Children.Add(selectedcb);
#endif
            container.Children.Add(itemNameFrame);
            container.Children.Add(viewButton);
#endif
#if EXPAND_SELECTED
            MainPage.AddTapHandler(container, (s, e) => Item_Tap(selectedcb), 1);
#endif
            MainPage.AddTapHandler(container, (s, e) => Item_DoubleTap(item, itemIndex), 2);

            var newItemGrid = new ItemGrid()
            {
                container = container,
#if EXPAND_SELECTED
                selectedHandler = selectedHandler,
                selected = selectedcb,
#endif
                name = itemNameFrame.Content as Label,
                viewButton = viewButton,
                viewButtonHandler = viewButtonHandler,
            };

            return newItemGrid;
        }

        public void Item_CheckedChanged(ItemType item, bool value)
        {
            if (item == null)
                return;
            if (item.selected == value)
                return;
            item.selected = value;
            CharacterSheetStorage.Instance.SaveCharacter();
            UpdateItemsView();
        }

        public void Item_Tap(CheckBox selectedcb)
        {
            selectedcb.IsChecked = !selectedcb.IsChecked;
        }

        public void Item_DoubleTap(ItemType item = null, int index = -1)
        {
            if (actEditItem != null)
                actEditItem(item, index);
        }

        public void ItemViewButton_Tap(ItemType item = null, int index = -1)
        {
            if (actViewItem != null)
                actViewItem(item, index);
        }
    }
}