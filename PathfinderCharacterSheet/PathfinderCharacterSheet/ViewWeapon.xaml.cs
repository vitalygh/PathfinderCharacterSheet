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
        private bool rebuild = true;
        private Page pushedPage = null;
        private Label AttackBonus = null;
        private Label DamageBonus = null;
        private CharacterSheet sheet = null;

        public ViewWeapon()
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
            rebuild |= this.sheet != sheet;
            if (AttackBonus != null)
                AttackBonus.Text = sheet.AttackBonus.ToString();
            if (DamageBonus != null)
                DamageBonus.Text = sheet.DamageBonus.ToString();
            if (!rebuild)
                return;
            rebuild = false;
            Weapon.Children.Clear();
            CreateHeaderGrids();
            CreateWeaponGrids();
        }

        private void CreateHeaderGrids()
        {
            sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
            };
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto },
            };
            var attackBonusTitle = CreateLabel("Attack Bonus: ", TextAlignment.Start);
            MainPage.AddTapHandler(attackBonusTitle, AttackBonus_DoubleTap, 2);
            var attackBonusFrame = new Frame()
            {
                Content = new Label()
                {
                    Text = sheet.AttackBonus.ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                BorderColor = Color.Black,
                Padding = 5,
            };
            MainPage.AddTapHandler(attackBonusFrame, AttackBonus_DoubleTap, 2);
            AttackBonus = attackBonusFrame.Content as Label;

            var damageBonusTitle = CreateLabel("Damage Bonus: ", TextAlignment.Start);
            MainPage.AddTapHandler(damageBonusTitle, DamageBonus_DoubleTap, 2);
            var damageBonusFrame = new Frame()
            {
                Content = new Label()
                {
                    Text = sheet.DamageBonus.ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                BorderColor = Color.Black,
                Padding = 5,
            };
            MainPage.AddTapHandler(damageBonusFrame, DamageBonus_DoubleTap, 2);
            DamageBonus = damageBonusFrame.Content as Label;

            grid.Children.Add(attackBonusTitle, 0, 0);
            grid.Children.Add(attackBonusFrame, 1, 0);
            grid.Children.Add(damageBonusTitle, 2, 0);
            grid.Children.Add(damageBonusFrame, 3, 0);

            Weapon.Children.Add(grid);

            grid = new Grid()
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
            var weaponTitle = CreateLabel("Weapon:");
            var weaponAddButton = new Button()
            {
                Text = "Add",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            weaponAddButton.Clicked += (s, e) => Weapon_DoubleTap();
            grid.Children.Add(weaponTitle, 0, 0);
            grid.Children.Add(weaponAddButton, 1, 0);
            Weapon.Children.Add(grid);
        }

        private void CreateWeaponGrids()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            List<KeyValuePair<CharacterSheet.WeaponItem, int>> other = new List<KeyValuePair<CharacterSheet.WeaponItem, int>>();
            var count = sheet.weaponItems.Count;
            for (var i = 0; i < count; i++)
            {
                var weapon = sheet.weaponItems[i];
                if (weapon == null)
                    continue;
                if (weapon.selected)
                    CreateSelectedWeaponGrid(weapon, i);
                else
                    other.Add(new KeyValuePair<CharacterSheet.WeaponItem, int>(weapon, i));
            }
            CreateWeaponsGrid(other);
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

        private void CreateSelectedWeaponGrid(CharacterSheet.WeaponItem weapon, int index)
        {
            if (weapon == null)
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
                IsChecked = weapon.selected,
            };
            selectedcb.CheckedChanged += (s, e) => Weapon_CheckedChanged(weapon, e.Value);
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
            var nameValue = CreateFrame(weapon.name);
            grid.Children.Add(nameStack, 0, row);
            grid.Children.Add(nameValue, 1, row);
            row += 1;

            var attackBonusTitle = CreateLabel("Attack Bonus: ", TextAlignment.Start);
            var attackBonusValue = CreateFrame(weapon.AttackBonus(sheet));
            grid.Children.Add(attackBonusTitle, 0, row);
            grid.Children.Add(attackBonusValue, 1, row);
            row += 1;

            var criticalTitle = CreateLabel("Critical: ", TextAlignment.Start);
            var criticalValue = CreateFrame(weapon.critical.AsString(sheet));
            grid.Children.Add(criticalTitle, 0, row);
            grid.Children.Add(criticalValue, 1, row);
            row += 1;

            var damageTitle = CreateLabel("Damage: ", TextAlignment.Start);
            var damageValue = CreateFrame(weapon.damage.AsString(sheet));
            grid.Children.Add(damageTitle, 0, row);
            grid.Children.Add(damageValue, 1, row);
            row += 1;

            var damageBonusTitle = CreateLabel("Damage Bonus: ", TextAlignment.Start);
            var damageBonusValue = CreateFrame(weapon.DamageBonus(sheet));
            grid.Children.Add(damageBonusTitle, 0, row);
            grid.Children.Add(damageBonusValue, 1, row);
            row += 1;

            var typeTitle = CreateLabel("Type: ", TextAlignment.Start);
            var typeValue = CreateFrame(weapon.type);
            grid.Children.Add(typeTitle, 0, row);
            grid.Children.Add(typeValue, 1, row);
            row += 1;

            var rangeTitle = CreateLabel("Range: ", TextAlignment.Start);
            var rangeValue = CreateFrame(weapon.range.GetTotal(sheet).ToString());
            grid.Children.Add(rangeTitle, 0, row);
            grid.Children.Add(rangeValue, 1, row);
            row += 1;

            var ammunitionTitle = CreateLabel("Ammunition: ", TextAlignment.Start);
            var ammunitionValue = CreateFrame(weapon.ammunition.GetTotal(sheet).ToString());
            grid.Children.Add(ammunitionTitle, 0, row);
            grid.Children.Add(ammunitionValue, 1, row);
            row += 1;

            var specialTitle = CreateLabel("Special: ", TextAlignment.Start);
            var specialValue = CreateFrame(weapon.special);
            grid.Children.Add(specialTitle, 0, row);
            grid.Children.Add(specialValue, 1, row);
            row += 1;

            var weightTitle = CreateLabel("Weight: ", TextAlignment.Start);
            var weightValue = CreateFrame(weapon.weight.GetTotal(sheet).ToString());
            grid.Children.Add(weightTitle, 0, row);
            grid.Children.Add(weightValue, 1, row);
            row += 1;

            var descriptionTitle = CreateLabel("Description: ", TextAlignment.Start);
            grid.Children.Add(descriptionTitle, 0, 2, row, row + 1);
            row += 1;

            var descriptionValue = CreateFrame(weapon.description);
            grid.Children.Add(descriptionValue, 0, 2, row, row + 1);
            row += 1;

            MainPage.AddTapHandler(grid, (s, e) => Weapon_DoubleTap(weapon, index), 2);

            Weapon.Children.Add(grid);
        }

        private void CreateWeaponsGrid(List<KeyValuePair<CharacterSheet.WeaponItem, int>> weaponItems)
        {
            if (weaponItems == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var count = weaponItems.Count;
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
                if (weaponItems == null)
                    continue;
                var weapon = weaponItems[i].Key;
                var index = weaponItems[i].Value;
                if (weapon == null)
                    continue;
                var selectedcb = new CheckBox()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    IsChecked = weapon.selected,
                };
                selectedcb.CheckedChanged += (s, e) => Weapon_CheckedChanged(weapon, e.Value);
                var weaponNameFrame = new Frame()
                {
                    Content = new Label()
                    {
                        Text = weapon.AsString(sheet),
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                MainPage.AddTapHandler(weaponNameFrame, (s, e) => Weapon_DoubleTap(weapon, index), 2);
                grid.Children.Add(selectedcb, 0, i);
                grid.Children.Add(weaponNameFrame, 1, i);
            }
            Weapon.Children.Add(grid);
        }

        public void AttackBonus_DoubleTap(object sender, EventArgs e)
        {

        }

        public void DamageBonus_DoubleTap(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.damageBonusModifiers, "Edit Damage Bonus", "Damage Bonus: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        public void Weapon_CheckedChanged(CharacterSheet.WeaponItem weapon, bool value)
        {
            if (weapon == null)
                return;
            if (weapon.selected == value)
                return;
            rebuild = true;
            weapon.selected = value;
            CharacterSheetStorage.Instance.SaveCharacter();
            UpdateView();
        }

        public void Weapon_DoubleTap(CharacterSheet.WeaponItem item = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            rebuild = true;
            var ew = new EditWeapon();
            ew.InitEditor(item, index);
            pushedPage = ew;
            Navigation.PushAsync(pushedPage);
        }
    }
}