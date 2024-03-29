﻿#define EXPAND_SELECTED
//#define EXPAND_WITH_TAP
#define EXPAND_CHECKBOX
//#define USE_GRID
#define USE_GRID_IN_HEADER
using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewWeapon : ContentPage, ISheetView
    {
#if EXPAND_SELECTED
        public class SelectedWeaponGrid: WeaponGrid
        {
            public EventHandler<CheckedChangedEventArgs> activeHandler = null;
            public CheckBox active = null;
            public Label nameTitle = null;
            public Label attackBonusTitle = null;
            public Label attackBonus = null;
            public Label criticalTitle = null;
            public Label critical = null;
            public Label damageTitle = null;
            public Label damage = null;
            public Label damageBonusTitle = null;
            public Label damageBonus = null;
            public Label typeTitle = null;
            public Label type = null;
            public Label rangeTitle = null;
            public Label range = null;
            public Label ammunitionTitle = null;
            public Label ammunition = null;
            public Label specialTitle = null;
            public Label special = null;
            public Label weightTitle = null;
            public Label weight = null;
            public Label descriptionTitle = null;
            public Label description = null;
        }
#endif

        public class WeaponGrid
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
        private Label attackBonus = null;
        private Label damageBonus = null;
        private Button weaponReorderButton = null;
#if EXPAND_SELECTED
        readonly List<SelectedWeaponGrid> selectedWeaponGrids = new List<SelectedWeaponGrid>();
        readonly List<SelectedWeaponGrid> selectedWeaponGridsPool = new List<SelectedWeaponGrid>();
#endif
        readonly List<WeaponGrid> weaponGrids = new List<WeaponGrid>();
        readonly List<WeaponGrid> weaponGridsPool = new List<WeaponGrid>();

        public ViewWeapon()
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
            if (attackBonus != null)
                attackBonus.Text = sheet.GetAttackBonus();
            if (damageBonus != null)
            {
                var db = sheet.DamageBonus;
                damageBonus.Text = db >= 0 ? "+" + db : db.ToString();
            }
            weaponReorderButton.IsVisible = sheet.weaponItems.Count > 1;
#if EXPAND_SELECTED
            var selectedWeaponItems = new List<KeyValuePair<WeaponItem, int>>();
#endif
            var weaponItems = new List<KeyValuePair<WeaponItem, int>>();
            var totalItemsCount = sheet.weaponItems.Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var wpn = sheet.weaponItems[i];
                if (wpn == null)
                    continue;
#if EXPAND_SELECTED
                if (wpn.selected)
                    selectedWeaponItems.Add(new KeyValuePair<WeaponItem, int>(wpn, i));
                else
#endif
                    weaponItems.Add(new KeyValuePair<WeaponItem, int>(wpn, i));
            }
#if EXPAND_SELECTED
            var selectedItemsCount = selectedWeaponItems.Count;
            var selectedGridsCount = selectedWeaponGrids.Count;
            var selectedUpdate = Math.Min(selectedGridsCount, selectedItemsCount);
            for (var i = 0; i < selectedUpdate; i++)
                UpdateWeaponGrid(selectedWeaponGrids[i], selectedWeaponItems[i]);
            var selectedCreate = selectedGridsCount < selectedItemsCount;
            var selectedLeft = selectedCreate ? selectedItemsCount - selectedGridsCount : selectedGridsCount - selectedItemsCount;
            for (int i = 0; i < selectedLeft; i++)
            {
                if (selectedCreate)
                    CreateSelectedWeaponGrid(selectedWeaponItems[i + selectedUpdate]);
                else
                    RemoveWeaponGrid(selectedWeaponGrids[selectedUpdate]);
            }
#endif
            var itemsCount = weaponItems.Count;
            var gridsCount = weaponGrids.Count;
            var update = Math.Min(gridsCount, itemsCount);
            for (var i = 0; i < update; i++)
                UpdateWeaponGrid(weaponGrids[i], weaponItems[i]);
            var create = gridsCount < itemsCount;
            var left = create ? itemsCount - gridsCount : gridsCount - itemsCount;
            for (int i = 0; i < left; i++)
            {
                if (create)
                    CreateWeaponGrid(weaponItems[i + update]);
                else
                    RemoveWeaponGrid(weaponGrids[update]);
            }
        }


        private void CreateHeader()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
#if USE_GRID || USE_GRID_IN_HEADER
            var container = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            container.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Star },
            };
            container.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
                new RowDefinition() { Height = GridLength.Auto },
            };
#else
            var container = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
#endif
            var attackBonusTitle = UIHelpers.CreateLabel("Attack Bonus:");
            UIHelpers.AddTapHandler(attackBonusTitle, AttackBonus_DoubleTap, 2);
            var ab = (sheet != null) ? sheet.GetAttackBonus() : "+0";
            var attackBonusFrame = UIHelpers.CreateFrame(ab);
            UIHelpers.AddTapHandler(attackBonusFrame, AttackBonus_DoubleTap, 2);
            attackBonus = attackBonusFrame.Content as Label;

            var damageBonusTitle = UIHelpers.CreateLabel("Damage Bonus:");
            UIHelpers.AddTapHandler(damageBonusTitle, DamageBonus_DoubleTap, 2);
            var db = (sheet != null) ? sheet.DamageBonus : 0;
            var damageBonusFrame = UIHelpers.CreateFrame(db >= 0 ? "+" + db : db.ToString());
            UIHelpers.AddTapHandler(damageBonusFrame, DamageBonus_DoubleTap, 2);
            damageBonus = damageBonusFrame.Content as Label;

#if USE_GRID || USE_GRID_IN_HEADER
            container.Children.Add(attackBonusTitle, 0, 0);
            container.Children.Add(attackBonusFrame, 1, 0);
            container.Children.Add(damageBonusTitle, 0, 1);
            container.Children.Add(damageBonusFrame, 1, 1);
#else
            var attack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray,
            };
            attack.Children.Add(attackBonusTitle);
            attack.Children.Add(attackBonusFrame);
            container.Children.Add(attack);
            var damage = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray,
            };
            damage.Children.Add(damageBonusTitle);
            damage.Children.Add(damageBonusFrame);
            container.Children.Add(damage);
#endif
            Header.Children.Add(container);
#if USE_GRID
            var weapon = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            weapon.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Auto },
            };
            weapon.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
#else
            var weapon = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.LightGray,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
#endif
            weaponReorderButton = new Button()
            {
                Text = "Reorder",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
            };
            weaponReorderButton.Clicked += Reorder_Clicked;
            var weaponTitle = UIHelpers.CreateLabel("Weapon", TextAlignment.Center);
            var weaponAddButton = new Button()
            {
                Text = "Add",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
            };
            weaponAddButton.Clicked += (s, e) => Weapon_DoubleTap();
#if USE_GRID
            weapon.Children.Add(weaponReorderButton, 0, 0);
            weapon.Children.Add(weaponTitle, 1, 0);
            weapon.Children.Add(weaponAddButton, 2, 0);
#else
            weapon.Children.Add(weaponReorderButton);
            weapon.Children.Add(weaponTitle);
            weapon.Children.Add(weaponAddButton);
#endif
            Header.Children.Add(weapon);
        }

#if EXPAND_SELECTED
        private void RemoveWeaponGrid(SelectedWeaponGrid weaponGrid)
        {
            if (weaponGrid == null)
                return;
            Weapon.Children.Remove(weaponGrid.container);
            selectedWeaponGrids.Remove(weaponGrid);
            selectedWeaponGridsPool.Add(weaponGrid);
        }

        private void UpdateWeaponGrid(SelectedWeaponGrid weaponGrid, KeyValuePair<WeaponItem, int> kvp)
        {
            UpdateWeaponGrid(weaponGrid, kvp.Key, kvp.Value);
        }

        private void UpdateWeaponGrid(SelectedWeaponGrid weaponGrid, WeaponItem item, int itemIndex)
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
#if EXPAND_CHECKBOX
            if (weaponGrid.selectedHandler != null)
                weaponGrid.selected.CheckedChanged -= weaponGrid.selectedHandler;
            weaponGrid.selectedHandler = (s, e) => Weapon_CheckedChanged(item, e.Value);
            UIHelpers.UpdateValue(weaponGrid.selected, item.selected);
            weaponGrid.selected.IsChecked = item.selected;
            weaponGrid.selected.CheckedChanged += weaponGrid.selectedHandler;
#endif
            if (weaponGrid.activeHandler != null)
                weaponGrid.active.CheckedChanged -= weaponGrid.activeHandler;
            weaponGrid.activeHandler = (s, e) => WeaponActive_CheckedChanged(item, e.Value);
            UIHelpers.UpdateValue(weaponGrid.active, item.active);
            weaponGrid.active.IsChecked = item.active;
            weaponGrid.active.CheckedChanged += weaponGrid.activeHandler;

            UIHelpers.UpdateValue(weaponGrid.name, item.name);
            UIHelpers.UpdateValue(weaponGrid.attackBonus, item.AttackBonus(sheet));
            UIHelpers.UpdateValue(weaponGrid.critical, item.critical.AsString(sheet));
            UIHelpers.UpdateValue(weaponGrid.damage, item.Damage(sheet));
            UIHelpers.UpdateValue(weaponGrid.damageBonus, item.DamageBonus(sheet));
            UIHelpers.UpdateValue(weaponGrid.type, item.type);
            UIHelpers.UpdateValue(weaponGrid.range, item.Range(sheet));
            UIHelpers.UpdateValue(weaponGrid.ammunition, item.ammunition.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(weaponGrid.special, item.special);
            UIHelpers.UpdateValue(weaponGrid.weight, item.weight.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(weaponGrid.description, item.description);

            UIHelpers.SetTapHandler(weaponGrid.container, (s, e) => Weapon_DoubleTap(item), 2);
#if EXPAND_WITH_TAP
#if EXPAND_CHECKBOX
            UIHelpers.AddTapHandler(weaponGrid.container, (s, e) => Weapon_Tap(weaponGrid.selected), 1);
#else
            UIHelpers.AddTapHandler(weaponGrid.container, (s, e) => Weapon_Tap(item), 1);
#endif
#endif
        }

        private void CreateSelectedWeaponGrid(KeyValuePair<WeaponItem, int> kvp)
        {
            CreateSelectedWeaponGrid(kvp.Key, kvp.Value);
        }

        private void CreateSelectedWeaponGrid(WeaponItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (selectedWeaponGridsPool.Count > 0)
            {
                var weaponGrid = selectedWeaponGridsPool[0];
                selectedWeaponGridsPool.RemoveAt(0);
                UpdateWeaponGrid(weaponGrid, item, itemIndex);
                var pos = selectedWeaponGrids.Count;
                selectedWeaponGrids.Add(weaponGrid);
                Weapon.Children.Insert(pos, weaponGrid.container);
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
            void handler(object s, CheckedChangedEventArgs e) => Weapon_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += handler;
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
            void activeHandler(object s, CheckedChangedEventArgs e) => WeaponActive_CheckedChanged(item, e.Value);
            activecb.CheckedChanged += activeHandler;
            grid.Children.Add(activeTitle, 0, row);
            grid.Children.Add(activecb, 1, row);
            row += 1;

            var attackBonusTitle = UIHelpers.CreateLabel("Attack Bonus:");
            var attackBonusValue = UIHelpers.CreateFrame(item.AttackBonus(sheet));
            grid.Children.Add(attackBonusTitle, 0, row);
            grid.Children.Add(attackBonusValue, 1, row);
            row += 1;

            var criticalTitle = UIHelpers.CreateLabel("Critical:");
            var criticalValue = UIHelpers.CreateFrame(item.critical.AsString(sheet));
            grid.Children.Add(criticalTitle, 0, row);
            grid.Children.Add(criticalValue, 1, row);
            row += 1;

            var damageTitle = UIHelpers.CreateLabel("Damage:");
            var damageValue = UIHelpers.CreateFrame(item.Damage(sheet));
            grid.Children.Add(damageTitle, 0, row);
            grid.Children.Add(damageValue, 1, row);
            row += 1;

            var damageBonusTitle = UIHelpers.CreateLabel("Damage Bonus:");
            var damageBonusValue = UIHelpers.CreateFrame(item.DamageBonus(sheet));
            grid.Children.Add(damageBonusTitle, 0, row);
            grid.Children.Add(damageBonusValue, 1, row);
            row += 1;

            var typeTitle = UIHelpers.CreateLabel("Type:");
            var typeValue = UIHelpers.CreateFrame(item.type);
            grid.Children.Add(typeTitle, 0, row);
            grid.Children.Add(typeValue, 1, row);
            row += 1;

            var rangeTitle = UIHelpers.CreateLabel("Range:");
            var rangeValue = UIHelpers.CreateFrame(item.Range(sheet));
            grid.Children.Add(rangeTitle, 0, row);
            grid.Children.Add(rangeValue, 1, row);
            row += 1;

            var ammunitionTitle = UIHelpers.CreateLabel("Ammunition:");
            var ammunitionValue = UIHelpers.CreateFrame(item.ammunition.GetValue(sheet).ToString());
            grid.Children.Add(ammunitionTitle, 0, row);
            grid.Children.Add(ammunitionValue, 1, row);
            row += 1;

            var specialTitle = UIHelpers.CreateLabel("Special:");
            var specialValue = UIHelpers.CreateFrame(item.special);
            grid.Children.Add(specialTitle, 0, row);
            grid.Children.Add(specialValue, 1, row);
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

            UIHelpers.AddTapHandler(grid, (s, e) => Weapon_DoubleTap(item), 2);
#if EXPAND_CHECKBOX
            UIHelpers.AddTapHandler(grid, (s, e) => Weapon_Tap(selectedcb), 1);
#else
            UIHelpers.AddTapHandler(grid, (s, e) => Weapon_Tap(item), 1);
#endif

            var newWeaponGrid = new SelectedWeaponGrid()
            {
                container = grid,
#if EXPAND_CHECKBOX
                selectedHandler = handler,
                selected = selectedcb,
#endif
                activeHandler = activeHandler,
                active = activecb,
                nameTitle = nameTitle,
                nameFrame = nameValue,
                name = nameValue.Content as Label,
                attackBonusTitle = attackBonusTitle,
                attackBonus = attackBonusValue.Content as Label,
                criticalTitle = criticalTitle,
                critical = criticalValue.Content as Label,
                damageTitle = damageTitle,
                damage = damageValue.Content as Label,
                damageBonusTitle = damageBonusTitle,
                damageBonus = damageBonusValue.Content as Label,
                typeTitle = typeTitle,
                type = typeValue.Content as Label,
                rangeTitle = rangeTitle,
                range = rangeValue.Content as Label,
                ammunitionTitle = ammunitionTitle,
                ammunition = ammunitionValue.Content as Label,
                specialTitle = specialTitle,
                special = specialValue.Content as Label,
                weightTitle = weightTitle,
                weight = weightValue.Content as Label,
                descriptionTitle = descriptionTitle,
                description = descriptionValue.Content as Label,
            };

            var newpos = selectedWeaponGrids.Count;
            selectedWeaponGrids.Add(newWeaponGrid);
            Weapon.Children.Insert(newpos, newWeaponGrid.container);
        }
#endif

            private void RemoveWeaponGrid(WeaponGrid weaponGrid)
        {
            if (weaponGrid == null)
                return;
            Weapon.Children.Remove(weaponGrid.container);
            weaponGrids.Remove(weaponGrid);
            weaponGridsPool.Add(weaponGrid);
        }

        private void UpdateWeaponGrid(WeaponGrid weaponGrid, KeyValuePair<WeaponItem, int> kvp)
        {
            UpdateWeaponGrid(weaponGrid, kvp.Key, kvp.Value);
        }

        private void UpdateWeaponGrid(WeaponGrid weaponGrid, WeaponItem item, int itemIndex)
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            UIHelpers.SetTapHandler(weaponGrid.container, (s, e) => Weapon_DoubleTap(item), 2);
#if EXPAND_SELECTED
#if EXPAND_CHECKBOX
            if (weaponGrid.selectedHandler != null)
                weaponGrid.selected.CheckedChanged -= weaponGrid.selectedHandler;
            weaponGrid.selectedHandler = (s, e) => Weapon_CheckedChanged(item, e.Value);
            UIHelpers.UpdateValue(weaponGrid.selected, item.selected);
            weaponGrid.selected.CheckedChanged += weaponGrid.selectedHandler;
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(weaponGrid.container, (s, e) => Weapon_Tap(weaponGrid.selected), 1);
#endif
#else
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(weaponGrid.container, (s, e) => Weapon_Tap(item), 1);
#endif
#endif
#endif
            weaponGrid.name.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
            UIHelpers.UpdateValue(weaponGrid.name, item.AsString(sheet));
        }

        private void CreateWeaponGrid(KeyValuePair<WeaponItem, int> kvp)
        {
            CreateWeaponGrid(kvp.Key, kvp.Value);
        }

        private void CreateWeaponGrid(WeaponItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (weaponGridsPool.Count > 0)
            {
                var weaponGrid = weaponGridsPool[0];
                weaponGridsPool.RemoveAt(0);
                UpdateWeaponGrid(weaponGrid, item, itemIndex);
                var pos =
#if EXPAND_SELECTED
                    selectedWeaponGrids.Count + 
#endif
                    weaponGrids.Count;
                weaponGrids.Add(weaponGrid);
                Weapon.Children.Insert(pos, weaponGrid.container);
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
            var weaponNameFrame = UIHelpers.CreateFrame(item.AsString(sheet));
            weaponNameFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
            var weaponName = weaponNameFrame.Content as Label;
            weaponName.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
            UIHelpers.AddTapHandler(container, (s, e) => Weapon_DoubleTap(item), 2);
#if EXPAND_SELECTED
#if EXPAND_CHECKBOX
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            void handler(object s, CheckedChangedEventArgs e) => Weapon_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += handler;
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(container, (s, e) => Weapon_Tap(selectedcb), 1);
#endif
#if USE_GRID
            container.Children.Add(selectedcb, 0, 0);
#else
            container.Children.Add(selectedcb);
#endif
#else
#if EXPAND_WITH_TAP
            UIHelpers.AddTapHandler(container, (s, e) => Weapon_Tap(item), 1);
#endif
#endif
#endif
#if USE_GRID
            container.Children.Add(weaponNameFrame, 1, 0);
#else
            container.Children.Add(weaponNameFrame);
#endif

            var newWeaponGrid = new WeaponGrid()
            {
                container = container,
#if EXPAND_SELECTED && EXPAND_CHECKBOX
                selectedHandler = handler,
                selected = selectedcb,
#endif
                name = weaponName,
                nameFrame = weaponNameFrame,
            };

            var newpos =
#if EXPAND_SELECTED
                selectedWeaponGrids.Count +
#endif
                weaponGrids.Count;
            weaponGrids.Add(newWeaponGrid);
            Weapon.Children.Insert(newpos, newWeaponGrid.container);
        }

        public void AttackBonus_DoubleTap(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var page = new EditAttackBonus();
            pushedPage = page;
            Navigation.PushAsync(page);
        }

        public void DamageBonus_DoubleTap(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.damageBonusModifiers, "Edit Damage Bonus", "Damage Bonus", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

#if EXPAND_SELECTED
        public void Weapon_CheckedChanged(WeaponItem weapon, bool value)
        {
            if (weapon == null)
                return;
            if (weapon.selected == value)
                return;
            weapon.selected = value;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }
#if EXPAND_CHECKBOX
        public static void Weapon_Tap(CheckBox cb)
        {
            cb.IsChecked = !cb.IsChecked;
        }
#else
        public void Weapon_Tap(CharacterSheet.WeaponItem weapon)
        {
            if (weapon == null)
                return;
            weapon.selected = !weapon.selected;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }
#endif
#endif

        public void WeaponActive_CheckedChanged(WeaponItem item, bool value)
        {
            if (item == null)
                return;
            if (item.active == value)
                return;
            item.active = value;
            UIMediator.OnCharacterSheetChanged?.Invoke();
            UpdateView();
        }

        public void Weapon_DoubleTap(WeaponItem item = null)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ew = new EditWeapon();
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
            foreach (var item in sheet.weaponItems)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                sheet.weaponItems.Clear();
                foreach (var item in reordered)
                    sheet.weaponItems.Add(item as WeaponItem);
                UIMediator.OnCharacterSheetChanged?.Invoke();
            });
            Navigation.PushAsync(pushedPage);
        }
    }
}