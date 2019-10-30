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
	public partial class EditIntModifier : ContentPage
	{
        private CharacterSheet sheet = null;
        private CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiersList = null;
        private CharacterSheet.IntModifier modifier = null;

        private Label IsActiveTitle = null;
        private CheckBox IsActive = null;

        private Label ValueTitle = null;
        private Frame ModifierValueFrame = null;
        private Entry ModifierValue = null;

        private Label NameTitle = null;
        private Frame ModifierNameFrame = null;
        private Entry ModifierName = null;

        private Label AbilityTitle = null;
        private Picker Ability = null;

        private Label MultiplierTitle = null;
        private Frame MultiplierFrame = null;
        private Entry Multiplier = null;

        private Label DividerTitle = null;
        private Frame DividerFrame = null;
        private Entry Divider = null;

        private Button Cancel = null;
        private Button Save = null;
        private Button Delete = null;

        public class AbilityPickerItem
        {
            public string Name { set; get; }
            public CharacterSheet.Ability Value { set; get; }
        }

        public EditIntModifier()
		{
			InitializeComponent ();
		}

        private void InitControls(bool allowUseAbilities = true)
        {
            Root.Children.Clear();
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto, },
                new ColumnDefinition() { Width = GridLength.Star, },
            };
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
            };
            IsActiveTitle = new Label()
            {
                Text = "Active: ",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            grid.Children.Add(IsActiveTitle, 0, 0);
            IsActive = new CheckBox()
            {
                IsChecked = true,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            grid.Children.Add(IsActive, 1, 0);
            ValueTitle = new Label()
            {
                Text = "Value: ",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            grid.Children.Add(ValueTitle, 0, 1);
            ModifierValue = new Entry()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Keyboard = Keyboard.Numeric,
            };
            ModifierValue.TextChanged += (s, e) =>
            {
                UpdateValue();
            };
            ModifierValueFrame = new Frame()
            {
                Content = ModifierValue,
                BorderColor = Color.Black,
                Padding = 5,
            };
            grid.Children.Add(ModifierValueFrame, 1, 1);
            NameTitle = new Label()
            {
                Text = "Name: ",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            grid.Children.Add(NameTitle, 0, 2);
            ModifierName = new Entry()
            {
                HorizontalTextAlignment = TextAlignment.Center,
            };
            ModifierName.TextChanged += (s, e) =>
            {
                UpdateValue();
            };
            ModifierNameFrame = new Frame()
            {
                Content = ModifierName,
                BorderColor = Color.Black,
                Padding = 5,
            };
            grid.Children.Add(ModifierNameFrame, 1, 2);
            if (allowUseAbilities)
            {
                AbilityTitle = new Label()
                {
                    Text = "Ability: ",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                grid.Children.Add(AbilityTitle, 0, 3);
                Ability = new Picker()
                {
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)),
                    ItemDisplayBinding = new Binding("Name"),
                };
                var abilityFrame = new Frame()
                {
                    Content = Ability,
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var abilities = new List<AbilityPickerItem>();
                var values = Enum.GetValues(typeof(CharacterSheet.Ability));
                var index = -1;
                var selectedIndex = -1;
                foreach (var v in values)
                {
                    var value = (CharacterSheet.Ability)v;
                    if (value == CharacterSheet.Ability.Total)
                        continue;
                    index += 1;
                    if ((modifier != null) && (modifier.SourceAbility == value))
                        selectedIndex = index;
                    abilities.Add(new AbilityPickerItem()
                    {
                        Name = v.ToString(),
                        Value = value,
                    });
                }
                Ability.ItemsSource = abilities;
                Ability.SelectedIndex = selectedIndex;
                Ability.SelectedIndexChanged += (s, e) =>
                {
                    UpdateValue();
                };
                grid.Children.Add(abilityFrame, 1, 3);
                MultiplierTitle = new Label()
                {
                    Text = "Multiplier: ",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                grid.Children.Add(MultiplierTitle, 0, 4);
                Multiplier = new Entry()
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    Keyboard = Keyboard.Numeric,
                };
                Multiplier.TextChanged += (s, e) =>
                {
                    UpdateValue();
                };
                MultiplierFrame = new Frame()
                {
                    Content = Multiplier,
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                grid.Children.Add(MultiplierFrame, 1, 4);
                DividerTitle = new Label()
                {
                    Text = "Divider: ",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                grid.Children.Add(DividerTitle, 0, 5);
                Divider = new Entry()
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    Keyboard = Keyboard.Numeric,
                };
                Divider.TextChanged += (s, e) =>
                {
                    UpdateValue();
                };
                DividerFrame = new Frame()
                {
                    Content = Divider,
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                grid.Children.Add(DividerFrame, 1, 5);
            }
            Root.Children.Add(grid);

            var buttons = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            Cancel = new Button()
            {
                Text = "Cancel",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttons.Children.Add(Cancel);
            Cancel.Clicked += Cancel_Clicked;
            Save = new Button()
            {
                Text = "Save",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttons.Children.Add(Save);
            Save.Clicked += Save_Clicked;
            Delete = new Button()
            {
                Text = "Delete",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttons.Children.Add(Delete);
            Delete.Clicked += Delete_Clicked;
            Root.Children.Add(buttons);
        }

        public void Init(CharacterSheet sheet, CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiersList, CharacterSheet.IntModifier modifier, bool allowUseAbilities = true)
        {
            this.sheet = sheet;
            this.modifiersList = modifiersList;
            this.modifier = modifier;
            InitControls(allowUseAbilities);
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            if (sheet == null)
                return;
            Delete.IsEnabled = modifier != null;
            UpdateValue();
            if (modifier == null)
                return;
            IsActive.IsChecked = modifier.IsActive;
            ModifierName.Text = modifier.Name;
            ModifierValue.Text = modifier.GetValue(sheet).ToString();
            if (Multiplier != null)
                Multiplier.Text = modifier.multiplier.ToString();
            if (Divider != null)
                Divider.Text = modifier.divider.ToString();
        }

        void UpdateValue()
        {
            if (sheet == null)
                return;
            var ab = false;
            var currentAbility = CharacterSheet.Ability.None;
            if (Ability != null)
            {
                var item = (Ability.SelectedItem as AbilityPickerItem);
                if (item != null)
                {
                    currentAbility = item.Value;
                    ab = currentAbility != CharacterSheet.Ability.None;
                }
            }
            ModifierValue.IsReadOnly = ab;
            ModifierValueFrame.BackgroundColor = ab ? Color.LightGray : Color.White;
            var currentModifier = new CharacterSheet.IntModifier();
            currentModifier.SourceAbility = currentAbility;
            MainPage.StrToInt(ModifierValue.Text, ref currentModifier.value);
            if (Multiplier != null)
                MainPage.StrToInt(Multiplier.Text, ref currentModifier.multiplier);
            if (Divider != null)
                MainPage.StrToInt(Divider.Text, ref currentModifier.divider);
            ModifierValue.Text = currentModifier.GetValue(sheet).ToString();
            if (Multiplier != null)
            {
                Multiplier.IsReadOnly = !ab;
                MultiplierFrame.BackgroundColor = !ab ? Color.LightGray : Color.White;
            }
            if (Divider != null)
            {
                Divider.IsReadOnly = !ab;
                DividerFrame.BackgroundColor = !ab ? Color.LightGray : Color.White;
            }
        }

        private void EditToView()
        {
            var anyChanged = false;
            if (modifier ==  null)
            {
                modifier = new CharacterSheet.IntModifier();
                modifiersList.Add(modifier);
                anyChanged = true;
            }
            anyChanged |= modifier.IsActive != IsActive.IsChecked;
            modifier.active = IsActive.IsChecked;
            anyChanged |= MainPage.StrToInt(ModifierValue.Text, ref modifier.value);
            anyChanged |= ModifierName.Text != modifier.name;
            modifier.name = ModifierName.Text;
            if (Ability != null)
            {
                var selectedItem = Ability.SelectedItem as AbilityPickerItem;
                if (selectedItem != null)
                {
                    anyChanged |= modifier.SourceAbility != selectedItem.Value;
                    modifier.SourceAbility = selectedItem.Value;
                }
            }
            if (Multiplier != null)
                anyChanged |= MainPage.StrToInt(Multiplier.Text, ref modifier.multiplier);
            if (Divider != null)
                anyChanged |= MainPage.StrToInt(Divider.Text, ref modifier.divider);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            EditToView();
            Navigation.PopAsync();
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var modifierName = string.Empty;
            if ((modifier != null) && !string.IsNullOrWhiteSpace(modifier.Name))
                modifierName = " \"" + modifier.Name + "\"";
            bool allow = await DisplayAlert("Remove modifier" + modifierName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                modifiersList.Remove(modifier);
                await Navigation.PopAsync();
            }
        }
    }
}