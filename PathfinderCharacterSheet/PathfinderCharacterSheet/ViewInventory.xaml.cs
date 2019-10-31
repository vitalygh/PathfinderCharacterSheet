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
    public partial class ViewInventory : ContentPage, ISheetView
    {
        public class SelectedGearItemGrid
        {
            public Grid grid = null;
            public EventHandler<CheckedChangedEventArgs> handler = null;
            public CheckBox selected = null;
            public Label nameTitle = null;
            public Label name = null;
            public Label amountTitle = null;
            public Label amount = null;
            public Label weightTitle = null;
            public Label weight = null;
            public Label descriptionTitle = null;
            public Label description = null;
        }

        public class GearItemGrid
        {
            public Grid grid = null;
            public EventHandler<CheckedChangedEventArgs> handler = null;
            public CheckBox selected = null;
            public Label text = null;
        }

        private Page pushedPage = null;

        List<SelectedGearItemGrid> selectedGearItemGrids = new List<SelectedGearItemGrid>();
        List<SelectedGearItemGrid> selectedGearItemGridsPool = new List<SelectedGearItemGrid>();
        List<GearItemGrid> gearItemGrids = new List<GearItemGrid>();
        List<GearItemGrid> gearItemGridsPool = new List<GearItemGrid>();

        public ViewInventory()
        {
            InitializeComponent();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var selectedGearItems = new List<KeyValuePair<CharacterSheet.GearItem, int>>();
            var gearItems = new List<KeyValuePair<CharacterSheet.GearItem, int>>();
            var totalItemsCount = sheet.gear.Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var item = sheet.gear[i];
                if (item == null)
                    continue;
                if (item.selected)
                    selectedGearItems.Add(new KeyValuePair<CharacterSheet.GearItem, int>(item, i));
                else
                    gearItems.Add(new KeyValuePair<CharacterSheet.GearItem, int>(item, i));
            }
            var itemsCount = selectedGearItems.Count;
            var gridsCount = selectedGearItemGrids.Count;
            var update = Math.Min(gridsCount, itemsCount);
            for (var i = 0; i < update; i++)
                UpdateGearItemGrid(selectedGearItemGrids[i], selectedGearItems[i]);
            var create = gridsCount < itemsCount;
            var left = create ? itemsCount - gridsCount : gridsCount - itemsCount;
            for (int i = 0; i < left; i++)
            {
                if (create)
                    CreateSelectedGearItemGrid(selectedGearItems[i + update]);
                else
                    RemoveGearItemGrid(selectedGearItemGrids[update]);
            }

            itemsCount = gearItems.Count;
            gridsCount = gearItemGrids.Count;
            update = Math.Min(gridsCount, itemsCount);
            for (var i = 0; i < update; i++)
                UpdateGearItemGrid(gearItemGrids[i], gearItems[i]);
            create = gridsCount < itemsCount;
            left = create ? itemsCount - gridsCount : gridsCount - itemsCount;
            for (int i = 0; i < left; i++)
            {
                if (create)
                    CreateGearItemGrid(gearItems[i + update]);
                else
                    RemoveGearItemGrid(gearItemGrids[update]);
            }
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
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BorderColor = Color.Black,
                Padding = 5,
            };
        }

        private void RemoveGearItemGrid(SelectedGearItemGrid gearItemGrid)
        {
            if (gearItemGrid == null)
                return;
            Gear.Children.Remove(gearItemGrid.grid);
            selectedGearItemGrids.Remove(gearItemGrid);
            selectedGearItemGridsPool.Add(gearItemGrid);
        }

        private void UpdateGearItemGrid(SelectedGearItemGrid gearItemGrid, KeyValuePair<CharacterSheet.GearItem, int> kvp)
        {
            UpdateGearItemGrid(gearItemGrid, kvp.Key, kvp.Value);
        }

        private void UpdateGearItemGrid(SelectedGearItemGrid gearItemGrid, CharacterSheet.GearItem item, int itemIndex)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (gearItemGrid.handler != null)
                gearItemGrid.selected.CheckedChanged -= gearItemGrid.handler;
            gearItemGrid.handler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            gearItemGrid.selected.IsChecked = item.selected;
            gearItemGrid.selected.CheckedChanged += gearItemGrid.handler;
            gearItemGrid.name.Text = item.name;
            gearItemGrid.amount.Text = item.amount.GetTotal(sheet).ToString();
            gearItemGrid.weight.Text = item.weight.GetTotal(sheet).ToString();
            gearItemGrid.description.Text = item.description;

            gearItemGrid.grid.GestureRecognizers.Clear();
            MainPage.AddTapHandler(gearItemGrid.grid, (s, e) => GearItem_Tap(gearItemGrid.selected), 1);
            MainPage.AddTapHandler(gearItemGrid.grid, (s, e) => GearItem_DoubleTap(item, itemIndex), 2);
        }

        private void CreateSelectedGearItemGrid(KeyValuePair<CharacterSheet.GearItem, int> kvp)
        {
            CreateSelectedGearItemGrid(kvp.Key, kvp.Value);
        }

        private void CreateSelectedGearItemGrid(CharacterSheet.GearItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (selectedGearItemGridsPool.Count > 0)
            {
                var gearItemGrid = selectedGearItemGridsPool[0];
                selectedGearItemGridsPool.RemoveAt(0);
                UpdateGearItemGrid(gearItemGrid, item, itemIndex);
                var pos = 2 + selectedGearItemGrids.Count;
                selectedGearItemGrids.Add(gearItemGrid);
                Gear.Children.Insert(pos, gearItemGrid.grid);
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
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> handler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += handler;
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
            var nameValue = CreateFrame(item.name);
            grid.Children.Add(nameStack, 0, row);
            grid.Children.Add(nameValue, 1, row);
            row += 1;

            var amountTitle = CreateLabel("Amount: ", TextAlignment.Start);
            var amountValue = CreateFrame(item.amount.GetTotal(sheet).ToString());
            grid.Children.Add(amountTitle, 0, row);
            grid.Children.Add(amountValue, 1, row);
            row += 1;

            var weightTitle = CreateLabel("Weight: ", TextAlignment.Start);
            var weightValue = CreateFrame(item.weight.GetTotal(sheet).ToString());
            grid.Children.Add(weightTitle, 0, row);
            grid.Children.Add(weightValue, 1, row);
            row += 1;

            var descriptionTitle = CreateLabel("Description: ", TextAlignment.Start);
            grid.Children.Add(descriptionTitle, 0, 2, row, row + 1);
            row += 1;

            var descriptionValue = CreateFrame(item.description);
            grid.Children.Add(descriptionValue, 0, 2, row, row + 1);
            row += 1;

            MainPage.AddTapHandler(grid, (s, e) => GearItem_Tap(selectedcb), 1);
            MainPage.AddTapHandler(grid, (s, e) => GearItem_DoubleTap(item, itemIndex), 2);

            var newGearItemGrid = new SelectedGearItemGrid()
            {
                grid = grid,
                handler = handler,
                selected = selectedcb,
                nameTitle = nameTitle,
                name = nameValue.Content as Label,
                amountTitle = amountTitle,
                amount = amountValue.Content as Label,
                weightTitle = weightTitle,
                weight = weightValue.Content as Label,
                descriptionTitle = descriptionTitle,
                description = descriptionValue.Content as Label,
            };

            var newpos = 2 + selectedGearItemGrids.Count;
            selectedGearItemGrids.Add(newGearItemGrid);
            Gear.Children.Insert(newpos, newGearItemGrid.grid);
        }

        private void RemoveGearItemGrid(GearItemGrid gearItemGrid)
        {
            if (gearItemGrid == null)
                return;
            Gear.Children.Remove(gearItemGrid.grid);
            gearItemGrids.Remove(gearItemGrid);
            gearItemGridsPool.Add(gearItemGrid);
        }

        private void UpdateGearItemGrid(GearItemGrid gearItemGrid, KeyValuePair<CharacterSheet.GearItem, int> kvp)
        {
            UpdateGearItemGrid(gearItemGrid, kvp.Key, kvp.Value);
        }

        private void UpdateGearItemGrid(GearItemGrid gearItemGrid, CharacterSheet.GearItem item, int itemIndex)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (gearItemGrid.handler != null)
                gearItemGrid.selected.CheckedChanged -= gearItemGrid.handler;
            gearItemGrid.handler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            gearItemGrid.selected.IsChecked = item.selected;
            gearItemGrid.selected.CheckedChanged += gearItemGrid.handler;
            gearItemGrid.text.Text = item.AsString(sheet);

            gearItemGrid.grid.GestureRecognizers.Clear();
            MainPage.AddTapHandler(gearItemGrid.grid, (s, e) => GearItem_Tap(gearItemGrid.selected), 1);
            MainPage.AddTapHandler(gearItemGrid.grid, (s, e) => GearItem_DoubleTap(item, itemIndex), 2);
        }

        private void CreateGearItemGrid(KeyValuePair<CharacterSheet.GearItem, int> kvp)
        {
            CreateGearItemGrid(kvp.Key, kvp.Value);
        }

        private void CreateGearItemGrid(CharacterSheet.GearItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (gearItemGridsPool.Count > 0)
            {
                var gearItemGrid = gearItemGridsPool[0];
                gearItemGridsPool.RemoveAt(0);
                UpdateGearItemGrid(gearItemGrid, item, itemIndex);
                var pos = 2 + selectedGearItemGrids.Count + gearItemGrids.Count;
                gearItemGrids.Add(gearItemGrid);
                Gear.Children.Insert(pos, gearItemGrid.grid);
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
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };

            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> handler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += handler;
            var gearItemNameFrame = CreateFrame(item.AsString(sheet));
            MainPage.AddTapHandler(grid, (s, e) => GearItem_Tap(selectedcb), 1);
            MainPage.AddTapHandler(grid, (s, e) => GearItem_DoubleTap(item, itemIndex), 2);
            grid.Children.Add(selectedcb, 0, 0);
            grid.Children.Add(gearItemNameFrame, 1, 0);

            var newGearItemGrid = new GearItemGrid()
            {
                grid = grid,
                handler = handler,
                selected = selectedcb,
                text = gearItemNameFrame.Content as Label,
            };

            var newpos = 2 + selectedGearItemGrids.Count + gearItemGrids.Count;
            gearItemGrids.Add(newGearItemGrid);
            Gear.Children.Insert(newpos, newGearItemGrid.grid);
        }

        public void GearItem_CheckedChanged(CharacterSheet.GearItem gearItem, bool value)
        {
            if (gearItem == null)
                return;
            if (gearItem.selected == value)
                return;
            gearItem.selected = value;
            CharacterSheetStorage.Instance.SaveCharacter();
            UpdateView();
        }

        public void GearItem_Tap(CheckBox selectedcb)
        {
            selectedcb.IsChecked = !selectedcb.IsChecked;
        }

        public void GearItem_DoubleTap(CharacterSheet.GearItem gearItem = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            /*
            var egi = new EditGearItem();
            egi.InitEditor(gearItem, index);
            pushedPage = egi;
            Navigation.PushAsync(pushedPage);
            */
        }

        private void AddGear_Clicked(object sender, EventArgs e)
        {
            GearItem_DoubleTap();
        }

        private void PP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.money.platinumPoints, "Edit Platinum Points", "Platinum Points: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void GP_DoubleTapped(object sender, EventArgs e)
        {

        }

        private void SP_DoubleTapped(object sender, EventArgs e)
        {

        }

        private void CP_DoubleTapped(object sender, EventArgs e)
        {

        }

        private void Encumbrance_DoubleTapped(object sender, EventArgs e)
        {

        }
    }
}