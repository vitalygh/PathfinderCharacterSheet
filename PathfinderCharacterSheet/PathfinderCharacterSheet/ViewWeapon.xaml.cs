#define EXPAND_SELECTED
//#define USE_GRID
#define USE_GRID_IN_HEADER
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
    public partial class ViewWeapon : ContentPage, ISheetView
    {
#if EXPAND_SELECTED
        public class SelectedWeaponGrid: WeaponGrid
        {
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
#if EXPAND_SELECTED
            public EventHandler<CheckedChangedEventArgs> handler = null;
            public CheckBox selected = null;
#endif
            public Label name = null;
            public Frame nameFrame = null;
        }

        private Page pushedPage = null;
        private Label attackBonus = null;
        private Label damageBonus = null;
#if EXPAND_SELECTED
        List<SelectedWeaponGrid> selectedWeaponGrids = new List<SelectedWeaponGrid>();
        List<SelectedWeaponGrid> selectedWeaponGridsPool = new List<SelectedWeaponGrid>();
#endif
        List<WeaponGrid> weaponGrids = new List<WeaponGrid>();
        List<WeaponGrid> weaponGridsPool = new List<WeaponGrid>();

        public ViewWeapon()
        {
            InitializeComponent();
            CreateHeader();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (attackBonus != null)
            {
                var ab = sheet.AttackBonus;
                attackBonus.Text = ab >= 0 ? "+" + ab : ab.ToString();
            }
            if (damageBonus != null)
            {
                var db = sheet.DamageBonus;
                damageBonus.Text = db >= 0 ? "+" + db : db.ToString();
            }
#if EXPAND_SELECTED
            var selectedWeaponItems = new List<KeyValuePair<CharacterSheet.WeaponItem, int>>();
#endif
            var weaponItems = new List<KeyValuePair<CharacterSheet.WeaponItem, int>>();
            var totalItemsCount = sheet.weaponItems.Count;
            for (var i = 0; i < totalItemsCount; i++)
            {
                var wpn = sheet.weaponItems[i];
                if (wpn == null)
                    continue;
#if EXPAND_SELECTED
                if (wpn.selected)
                    selectedWeaponItems.Add(new KeyValuePair<CharacterSheet.WeaponItem, int>(wpn, i));
                else
#endif
                    weaponItems.Add(new KeyValuePair<CharacterSheet.WeaponItem, int>(wpn, i));
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            var attackBonusTitle = CreateLabel("Attack Bonus: ", TextAlignment.Start);
            MainPage.AddTapHandler(attackBonusTitle, AttackBonus_DoubleTap, 2);
            var ab = (sheet != null) ? sheet.AttackBonus : 0;
            var attackBonusFrame = new Frame()
            {
                Content = new Label()
                {
                    Text = ab >= 0 ? "+" + ab : ab.ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                BorderColor = Color.Black,
                Padding = 5,
            };
            MainPage.AddTapHandler(attackBonusFrame, AttackBonus_DoubleTap, 2);
            attackBonus = attackBonusFrame.Content as Label;

            var damageBonusTitle = CreateLabel("Damage Bonus: ", TextAlignment.Start);
            MainPage.AddTapHandler(damageBonusTitle, DamageBonus_DoubleTap, 2);
            var db = (sheet != null) ? sheet.DamageBonus : 0;
            var damageBonusFrame = new Frame()
            {
                Content = new Label()
                {
                    Text = db >= 0 ? "+" + db : db.ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                BorderColor = Color.Black,
                Padding = 5,
            };
            MainPage.AddTapHandler(damageBonusFrame, DamageBonus_DoubleTap, 2);
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
            var weaponTitle = CreateLabel("Weapon:");
            var weaponAddButton = new Button()
            {
                Text = "Add",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
            };
            weaponAddButton.Clicked += (s, e) => Weapon_DoubleTap();
#if USE_GRID
            weapon.Children.Add(weaponTitle, 0, 0);
            weapon.Children.Add(weaponAddButton, 1, 0);
#else
            weapon.Children.Add(weaponTitle);
            weapon.Children.Add(weaponAddButton);
#endif
            Header.Children.Add(weapon);
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Center)
        {
            var label = MainPage.CreateLabel(text, horz);
            label.HorizontalOptions = LayoutOptions.FillAndExpand;
            return label;
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void UpdateValue(CheckBox checkbox, bool value)
        {
            if (checkbox == null)
                return;
            if (checkbox.IsChecked != value)
                checkbox.IsChecked = value;
        }

        private void UpdateValue(Label label, string text)
        {
            if (label == null)
                return;
            if (label.Text != text)
                label.Text = text;
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

        private void UpdateWeaponGrid(SelectedWeaponGrid weaponGrid, KeyValuePair<CharacterSheet.WeaponItem, int> kvp)
        {
            UpdateWeaponGrid(weaponGrid, kvp.Key, kvp.Value);
        }

        private void UpdateWeaponGrid(SelectedWeaponGrid weaponGrid, CharacterSheet.WeaponItem item, int itemIndex)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (weaponGrid.handler != null)
                weaponGrid.selected.CheckedChanged -= weaponGrid.handler;
            weaponGrid.handler = (s, e) => Weapon_CheckedChanged(item, e.Value);
            UpdateValue(weaponGrid.selected, item.selected);
            weaponGrid.selected.IsChecked = item.selected;
            weaponGrid.selected.CheckedChanged += weaponGrid.handler;
            UpdateValue(weaponGrid.name, item.name);
            UpdateValue(weaponGrid.attackBonus, item.AttackBonus(sheet));
            UpdateValue(weaponGrid.critical, item.critical.AsString(sheet));
            UpdateValue(weaponGrid.damage, item.damage.AsString(sheet));
            UpdateValue(weaponGrid.damageBonus, item.DamageBonus(sheet));
            UpdateValue(weaponGrid.type, item.type);
            UpdateValue(weaponGrid.range, item.Range(sheet));
            UpdateValue(weaponGrid.ammunition, item.ammunition.GetTotal(sheet).ToString());
            UpdateValue(weaponGrid.special, item.special);
            UpdateValue(weaponGrid.weight, item.weight.GetTotal(sheet).ToString());
            UpdateValue(weaponGrid.description, item.description);

            weaponGrid.container.GestureRecognizers.Clear();
            MainPage.AddTapHandler(weaponGrid.container, (s, e) => Weapon_Tap(weaponGrid.selected), 1);
            MainPage.AddTapHandler(weaponGrid.container, (s, e) => Weapon_DoubleTap(item, itemIndex), 2);
        }

        private void CreateSelectedWeaponGrid(KeyValuePair<CharacterSheet.WeaponItem, int> kvp)
        {
            CreateSelectedWeaponGrid(kvp.Key, kvp.Value);
        }

        private void CreateSelectedWeaponGrid(CharacterSheet.WeaponItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> handler = (s, e) => Weapon_CheckedChanged(item, e.Value);
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

            var attackBonusTitle = CreateLabel("Attack Bonus: ", TextAlignment.Start);
            var attackBonusValue = CreateFrame(item.AttackBonus(sheet));
            grid.Children.Add(attackBonusTitle, 0, row);
            grid.Children.Add(attackBonusValue, 1, row);
            row += 1;

            var criticalTitle = CreateLabel("Critical: ", TextAlignment.Start);
            var criticalValue = CreateFrame(item.critical.AsString(sheet));
            grid.Children.Add(criticalTitle, 0, row);
            grid.Children.Add(criticalValue, 1, row);
            row += 1;

            var damageTitle = CreateLabel("Damage: ", TextAlignment.Start);
            var damageValue = CreateFrame(item.damage.AsString(sheet));
            grid.Children.Add(damageTitle, 0, row);
            grid.Children.Add(damageValue, 1, row);
            row += 1;

            var damageBonusTitle = CreateLabel("Damage Bonus: ", TextAlignment.Start);
            var damageBonusValue = CreateFrame(item.DamageBonus(sheet));
            grid.Children.Add(damageBonusTitle, 0, row);
            grid.Children.Add(damageBonusValue, 1, row);
            row += 1;

            var typeTitle = CreateLabel("Type: ", TextAlignment.Start);
            var typeValue = CreateFrame(item.type);
            grid.Children.Add(typeTitle, 0, row);
            grid.Children.Add(typeValue, 1, row);
            row += 1;

            var rangeTitle = CreateLabel("Range: ", TextAlignment.Start);
            var rangeValue = CreateFrame(item.Range(sheet));
            grid.Children.Add(rangeTitle, 0, row);
            grid.Children.Add(rangeValue, 1, row);
            row += 1;

            var ammunitionTitle = CreateLabel("Ammunition: ", TextAlignment.Start);
            var ammunitionValue = CreateFrame(item.ammunition.GetTotal(sheet).ToString());
            grid.Children.Add(ammunitionTitle, 0, row);
            grid.Children.Add(ammunitionValue, 1, row);
            row += 1;

            var specialTitle = CreateLabel("Special: ", TextAlignment.Start);
            var specialValue = CreateFrame(item.special);
            grid.Children.Add(specialTitle, 0, row);
            grid.Children.Add(specialValue, 1, row);
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

            MainPage.AddTapHandler(grid, (s, e) => Weapon_Tap(selectedcb), 1);
            MainPage.AddTapHandler(grid, (s, e) => Weapon_DoubleTap(item, itemIndex), 2);

            var newWeaponGrid = new SelectedWeaponGrid()
            {
                container = grid,
                handler = handler,
                selected = selectedcb,
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

        private void UpdateWeaponGrid(WeaponGrid weaponGrid, KeyValuePair<CharacterSheet.WeaponItem, int> kvp)
        {
            UpdateWeaponGrid(weaponGrid, kvp.Key, kvp.Value);
        }

        private void UpdateWeaponGrid(WeaponGrid weaponGrid, CharacterSheet.WeaponItem item, int itemIndex)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            weaponGrid.container.GestureRecognizers.Clear();
#if EXPAND_SELECTED
            if (weaponGrid.handler != null)
                weaponGrid.selected.CheckedChanged -= weaponGrid.handler;
            weaponGrid.handler = (s, e) => Weapon_CheckedChanged(item, e.Value);
            UpdateValue(weaponGrid.selected, item.selected);
            weaponGrid.selected.CheckedChanged += weaponGrid.handler;
            MainPage.AddTapHandler(weaponGrid.container, (s, e) => Weapon_Tap(weaponGrid.selected), 1);
#endif
            UpdateValue(weaponGrid.name, item.AsString(sheet));
            MainPage.AddTapHandler(weaponGrid.container, (s, e) => Weapon_DoubleTap(item, itemIndex), 2);
        }

        private void CreateWeaponGrid(KeyValuePair<CharacterSheet.WeaponItem, int> kvp)
        {
            CreateWeaponGrid(kvp.Key, kvp.Value);
        }

        private void CreateWeaponGrid(CharacterSheet.WeaponItem item, int itemIndex)
        {
            if (item == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            var weaponNameFrame = CreateFrame(item.AsString(sheet));
            weaponNameFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
            MainPage.AddTapHandler(container, (s, e) => Weapon_DoubleTap(item, itemIndex), 2);
#if EXPAND_SELECTED
            var selectedcb = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsChecked = item.selected,
            };
            EventHandler<CheckedChangedEventArgs> handler = (s, e) => Weapon_CheckedChanged(item, e.Value);
            selectedcb.CheckedChanged += handler;
            MainPage.AddTapHandler(container, (s, e) => Weapon_Tap(selectedcb), 1);
#if USE_GRID
            container.Children.Add(selectedcb, 0, 0);
#else
            container.Children.Add(selectedcb);
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
#if EXPAND_SELECTED
                handler = handler,
                selected = selectedcb,
#endif
                name = weaponNameFrame.Content as Label,
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.damageBonusModifiers, "Edit Damage Bonus", "Damage Bonus: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

#if EXPAND_SELECTED
        public void Weapon_CheckedChanged(CharacterSheet.WeaponItem weapon, bool value)
        {
            if (weapon == null)
                return;
            if (weapon.selected == value)
                return;
            weapon.selected = value;
            CharacterSheetStorage.Instance.SaveCharacter();
            UpdateView();
        }

        public void Weapon_Tap(CheckBox selectedcb)
        {
            selectedcb.IsChecked = !selectedcb.IsChecked;
        }
#endif

        public void Weapon_DoubleTap(CharacterSheet.WeaponItem item = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ew = new EditWeapon();
            ew.InitEditor(item, index);
            pushedPage = ew;
            Navigation.PushAsync(pushedPage);
        }
    }
}