//#define EXPAND_SELECTED
#define EXPAND_WITH_TAP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewInventory : ContentPage, ISheetView
    {
#if EXPAND_SELECTED
        public class SelectedGearItemGrid: GearItemGrid
        {
            public Label nameTitle = null;
            public Label amountTitle = null;
            public Label amount = null;
            public Label weightTitle = null;
            public Label weight = null;
            public Label descriptionTitle = null;
            public Label description = null;
        }
#endif

        public class GearItemGrid
        {
            public Grid grid = null;
#if EXPAND_SELECTED
            public EventHandler<CheckedChangedEventArgs> selectedHandler = null;
            public CheckBox selected = null;
#endif
            public Label name = null;
            public EventHandler viewButtonHandler = null;
            public Button viewButton = null;
        }

        private Page pushedPage = null;

#if  EXPAND_SELECTED
        List<SelectedGearItemGrid> selectedGearItemGridsPool = new List<SelectedGearItemGrid>();
#endif
        readonly List<GearItemGrid> gearItemGridsPool = new List<GearItemGrid>();
        readonly List<GearItemGrid> gearItemGrids = new List<GearItemGrid>();

        public ViewInventory()
        {
            InitializeComponent();
            UIHelpers.AddTapHandler(PPTitle, PP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(PPFrame, PP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(GPTitle, GP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(GPFrame, GP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(SPTitle, SP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(SPFrame, SP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(CPTitle, CP_DoubleTapped, 2);
            UIHelpers.AddTapHandler(CPFrame, CP_DoubleTapped, 2); 
            UIHelpers.AddTapHandler(EncumbranceGrid, Encumbrance_DoubleTapped, 2);

            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            UpdateValue(PP, sheet.money.platinumPoints.GetValue(sheet).ToString());
            UpdateValue(GP, sheet.money.goldenPoints.GetValue(sheet).ToString());
            UpdateValue(SP, sheet.money.silverPoints.GetValue(sheet).ToString());
            UpdateValue(CP, sheet.money.cuprumPoints.GetValue(sheet).ToString());
            UpdateValue(LightLoad, sheet.encumbrance.LightLoad(sheet));
            UpdateValue(MediumLoad, sheet.encumbrance.MediumLoad(sheet));
            UpdateValue(HeavyLoad, sheet.encumbrance.HeavyLoad(sheet));
            UpdateValue(LiftOverHead, sheet.encumbrance.LiftOverHead(sheet) + " lbs");
            UpdateValue(LiftOffGround, sheet.encumbrance.LiftOffGround(sheet) + " lbs");
            UpdateValue(DragOrPush, sheet.encumbrance.DragOrPush(sheet) + " lbs");
            var items = sheet.GetAllGearItems();
            var totalWeight = 0;
            foreach (var item in items)
                if (item != null)
                    totalWeight += item.TotalWeight(sheet);
            var heavy = sheet.encumbrance.heavyLoad.GetValue(sheet);
            var medium = sheet.encumbrance.mediumLoad.GetValue(sheet);
            var light = sheet.encumbrance.lightLoad.GetValue(sheet);
            TotalWeight.TextColor = totalWeight >= heavy ? Color.Red : (totalWeight > medium ? Color.Orange : (totalWeight > light ? Color.Yellow : Color.Green));
            Reorder.IsVisible = sheet.gear.Count > 1;
            UpdateValue(TotalWeight, totalWeight + " lbs");
            UpdateGearView();
        }

        private void UpdateGearView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var totalItemsCount = sheet.gear.Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var gearItem = sheet.gear[i];
                if (gearItemGrids.Count <= i)
                {
                    var newGearItemGrid =
#if EXPAND_SELECTED
                        gearItem.selected ? CreateSelectedGearItemGrid(gearItem, i) :
#endif
                        CreateGearItemGrid(gearItem, i);
                    gearItemGrids.Add(newGearItemGrid);
                    
                    Gear.Children.Add(newGearItemGrid.grid);
                    continue;
                }
                var gearItemGrid = gearItemGrids[i];
#if EXPAND_SELECTED
                var selectedGearItemGrid = gearItemGrid as SelectedGearItemGrid;
                if (gearItem.selected)
                {
                    if (selectedGearItemGrid != null)
                    {
                        UpdateGearItemGrid(selectedGearItemGrid, gearItem, i);
                        continue;
                    }
                    RemoveGearItemGrid(gearItemGrid);
                    var newGearItemGrid = CreateSelectedGearItemGrid(gearItem, i);
                    gearItemGrids.Insert(i, newGearItemGrid);
                    Gear.Children.Insert(i, newGearItemGrid.grid);
                    continue;
                }
                if (selectedGearItemGrid == null)
                {
#endif
                    UpdateGearItemGrid(gearItemGrid, gearItem, i);
                    continue;
#if EXPAND_SELECTED
                }
                RemoveGearItemGrid(gearItemGrid);
                gearItemGrid = CreateGearItemGrid(gearItem, i);
                gearItemGrids.Insert(i, gearItemGrid);
                Gear.Children.Insert(i, gearItemGrid.grid);
#endif
            }
            while (gearItemGrids.Count > sheet.gear.Count)
                RemoveGearItemGrid(gearItemGrids[gearItemGrids.Count - 1]);
        }

        private void UpdateValue(Label label, string text)
        {
            if (label == null)
                return;
            if (label.Text != text)
                label.Text = text;
        }

        private Frame CreateFrame(string text)
        {
            return UIHelpers.CreateFrame(text);
        }

#if EXPAND_SELECTED
        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return UIHelpers.CreateLabel(text, horz);
        }

        private void RemoveGearItemGrid(SelectedGearItemGrid gearItemGrid)
        {
            if (gearItemGrid == null)
                return;
            Gear.Children.Remove(gearItemGrid.grid);
            gearItemGrids.Remove(gearItemGrid);
            selectedGearItemGridsPool.Add(gearItemGrid);
        }

        private void UpdateGearItemGrid(SelectedGearItemGrid gearItemGrid, KeyValuePair<CharacterSheet.GearItem, int> kvp)
        {
            UpdateGearItemGrid(gearItemGrid, kvp.Key, kvp.Value);
        }

        private void UpdateGearItemGrid(SelectedGearItemGrid gearItemGrid, CharacterSheet.GearItem item, int itemIndex)
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;

            if (gearItemGrid.selectedHandler != null)
                gearItemGrid.selected.CheckedChanged -= gearItemGrid.selectedHandler;
            gearItemGrid.selectedHandler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            gearItemGrid.selected.IsChecked = item.selected;
            gearItemGrid.selected.CheckedChanged += gearItemGrid.selectedHandler;

            gearItemGrid.name.Text = item.name;
            gearItemGrid.amount.Text = item.amount.GetTotal(sheet).ToString();
            gearItemGrid.weight.Text = item.weight.GetTotal(sheet).ToString();
            gearItemGrid.description.Text = item.description;

            if (gearItemGrid.viewButtonHandler != null)
                gearItemGrid.viewButton.Clicked -= gearItemGrid.viewButtonHandler;
            gearItemGrid.viewButtonHandler = (s, e) => GearItemViewButton_Tap(item, itemIndex);
            gearItemGrid.viewButton.Clicked += gearItemGrid.viewButtonHandler;

            UIHelpers.SetTapHandler(gearItemGrid.grid, (s, e) => GearItem_DoubleTap(item, itemIndex), 2);
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(gearItemGrid.grid, (s, e) => GearItem_Tap(gearItemGrid.selected), 1);
#endif
        }

        private void CreateSelectedGearItemGrid(KeyValuePair<CharacterSheet.GearItem, int> kvp)
        {
            CreateSelectedGearItemGrid(kvp.Key, kvp.Value);
        }

        private SelectedGearItemGrid CreateSelectedGearItemGrid(CharacterSheet.GearItem item, int itemIndex)
        {
            if (item == null)
                return null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return null;
            if (selectedGearItemGridsPool.Count > 0)
            {
                var gearItemGrid = selectedGearItemGridsPool[0];
                selectedGearItemGridsPool.RemoveAt(0);
                UpdateGearItemGrid(gearItemGrid, item, itemIndex);
                return gearItemGrid;
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
            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
            var nameTitle = CreateLabel("Name: ");
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
            EventHandler viewButtonHandler = (s, e) => GearItemViewButton_Tap(item, itemIndex);
            viewButton.Clicked += viewButtonHandler;
            nameValueStack.Children.Add(nameValue);
            nameValueStack.Children.Add(viewButton);

            var row = 0;
            grid.Children.Add(nameTitleStack, 0, row);
            grid.Children.Add(nameValueStack, 1, row);
            row += 1;

            var amountTitle = CreateLabel("Amount: ");
            var amountValue = CreateFrame(item.amount.GetTotal(sheet).ToString());
            grid.Children.Add(amountTitle, 0, row);
            grid.Children.Add(amountValue, 1, row);
            row += 1;

            var weightTitle = CreateLabel("Weight: ");
            var weightValue = CreateFrame(item.weight.GetTotal(sheet).ToString());
            grid.Children.Add(weightTitle, 0, row);
            grid.Children.Add(weightValue, 1, row);
            row += 1;

            var descriptionTitle = CreateLabel("Description: ");
            grid.Children.Add(descriptionTitle, 0, 2, row, row + 1);
            row += 1;

            var descriptionValue = CreateFrame(item.description);
            grid.Children.Add(descriptionValue, 0, 2, row, row + 1);
            row += 1;

            UIHelpers.AddTapHandler(grid, (s, e) => GearItem_DoubleTap(item, itemIndex), 2);
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(grid, (s, e) => GearItem_Tap(selectedcb), 1);
#endif
            var newGearItemGrid = new SelectedGearItemGrid()
            {
                grid = grid,
                selectedHandler = selectedHandler,
                selected = selectedcb,
                viewButtonHandler = viewButtonHandler,
                viewButton = viewButton,
                nameTitle = nameTitle,
                name = nameValue.Content as Label,
                amountTitle = amountTitle,
                amount = amountValue.Content as Label,
                weightTitle = weightTitle,
                weight = weightValue.Content as Label,
                descriptionTitle = descriptionTitle,
                description = descriptionValue.Content as Label,
            };
            return newGearItemGrid;
        }
#endif

        private void RemoveGearItemGrid(GearItemGrid gearItemGrid)
        {
            if (gearItemGrid == null)
                return;
#if EXPAND_SELECTED
            var sgig = gearItemGrid as SelectedGearItemGrid;
            if (sgig != null)
            {
                RemoveGearItemGrid(sgig);
                return;
            }
#endif
            Gear.Children.Remove(gearItemGrid.grid);
            gearItemGrids.Remove(gearItemGrid);
            gearItemGridsPool.Add(gearItemGrid);
        }

        /*private void UpdateGearItemGrid(GearItemGrid gearItemGrid, KeyValuePair<GearItem, int> kvp)
        {
            UpdateGearItemGrid(gearItemGrid, kvp.Key, kvp.Value);
        }*/

        private void UpdateGearItemGrid(GearItemGrid gearItemGrid, GearItem item, int itemIndex)
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            UIHelpers.SetTapHandler(gearItemGrid.grid, (s, e) => GearItem_DoubleTap(item), 2);
#if EXPAND_SELECTED
            if (gearItemGrid.selectedHandler != null)
                gearItemGrid.selected.CheckedChanged -= gearItemGrid.selectedHandler;
            gearItemGrid.selectedHandler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            gearItemGrid.selected.IsChecked = item.selected;
            gearItemGrid.selected.CheckedChanged += gearItemGrid.selectedHandler;
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(gearItemGrid.grid, (s, e) => GearItem_Tap(gearItemGrid.selected), 1);
#endif
#endif
            gearItemGrid.name.Text = item.AsString(sheet);
            if (gearItemGrid.viewButtonHandler != null)
                gearItemGrid.viewButton.Clicked -= gearItemGrid.viewButtonHandler;
            gearItemGrid.viewButtonHandler = (s, e) => GearItemViewButton_Tap(item);
            gearItemGrid.viewButton.Clicked += gearItemGrid.viewButtonHandler;
            gearItemGrid.name.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
        }

        /*private void CreateGearItemGrid(KeyValuePair<GearItem, int> kvp)
        {
            CreateGearItemGrid(kvp.Key, kvp.Value);
        }*/

        private GearItemGrid CreateGearItemGrid(GearItem item, int itemIndex)
        {
            if (item == null)
                return null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return null;
            if (gearItemGridsPool.Count > 0)
            {
                var gearItemGrid = gearItemGridsPool[0];
                gearItemGridsPool.RemoveAt(0);
                UpdateGearItemGrid(gearItemGrid, item, itemIndex);
                return gearItemGrid;
            }
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
#if EXPAND_SELECTED
                new ColumnDefinition() { Width = GridLength.Auto },
#endif
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Auto },
            };
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
            UIHelpers.AddTapHandler(grid, (s, e) => GearItem_DoubleTap(item), 2);
#if EXPAND_SELECTED
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> selectedHandler = (s, e) => GearItem_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += selectedHandler;
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(grid, (s, e) => GearItem_Tap(selectedcb), 1);
#endif
#endif
            var gearItemNameFrame = CreateFrame(item.AsString(sheet));
            var gearItemName = gearItemNameFrame.Content as Label;
            gearItemName.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
            var viewButton = new Button()
            {
                Text = "View",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            void viewButtonHandler(object s, EventArgs e) => GearItemViewButton_Tap(item);
            viewButton.Clicked += viewButtonHandler;
            var column = 0;
#if EXPAND_SELECTED
            grid.Children.Add(selectedcb, column++, 0);
#endif
            grid.Children.Add(gearItemNameFrame, column++, 0);
            grid.Children.Add(viewButton, column++, 0);
            var newGearItemGrid = new GearItemGrid()
            {
                grid = grid,
#if EXPAND_SELECTED
                selectedHandler = selectedHandler,
                selected = selectedcb,
#endif
                name = gearItemName,
                viewButton = viewButton,
                viewButtonHandler = viewButtonHandler,
            };

            return newGearItemGrid;
        }

        public void GearItem_CheckedChanged(GearItem gearItem, bool value)
        {
            if (gearItem == null)
                return;
            if (gearItem.selected == value)
                return;
            gearItem.selected = value;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }

        public void GearItem_Tap(CheckBox selectedcb)
        {
            selectedcb.IsChecked = !selectedcb.IsChecked;
        }

        public void GearItemViewButton_Tap(GearItem gearItem = null)
        {
            if (pushedPage != null)
                return;
            var vgi = new ViewGearItem();
            vgi.InitView(gearItem);
            pushedPage = vgi;
            Navigation.PushAsync(pushedPage);
        }

        public void GearItem_DoubleTap(GearItem gearItem = null)
        {
            if (pushedPage != null)
                return;
            var egi = new EditGearItem();
            egi.InitEditor(gearItem);
            pushedPage = egi;
            Navigation.PushAsync(pushedPage);
        }

        private void AddGear_Clicked(object sender, EventArgs e)
        {
            GearItem_DoubleTap();
        }

        private void PP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.money.platinumPoints, "Edit Platinum Points", "Platinum Points", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void GP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.money.goldenPoints, "Edit Gold Points", "Gold Points", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.money.silverPoints, "Edit Silver Points", "Silver Points", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void CP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.money.cuprumPoints, "Edit Cuprum Points", "Cuprum Points", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Encumbrance_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ee = new EditEncumbrance();
            pushedPage = ee;
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
            foreach (var item in sheet.gear)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                sheet.gear.Clear();
                foreach (var item in reordered)
                    sheet.gear.Add(item as GearItem);
                UIMediator.OnCharacterSheetChanged?.Invoke();
            });
            Navigation.PushAsync(pushedPage);
        }
    }
}