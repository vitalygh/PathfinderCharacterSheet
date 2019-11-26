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
	public partial class EditIntModifier : ContentPage, ISheetView
	{
        public class RoundingTypesPickerItem
        {
            public string Name { set; get; }
            public CharacterSheet.IntModifier.RoundingTypes  Value { set; get; }
        }

        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiersList = null;
        private CharacterSheet.IntModifier source = null;
        private CharacterSheet.IntModifier modifier = null;

        private Label TotalTitle = null;
        private Label TotalValue = null;

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

        private Label RoundingTitle = null;
        private Frame RoundingFrame = null;
        private Picker Rounding = null;

        private Label LinkedItemTitle = null;
        private Frame LinkedItemFrame = null;
        private Label LinkedItem = null;

        private Label ItemMustBeActiveTitle = null;
        private CheckBox ItemMustBeActive = null;

        //private Label MultiplyToLevelTitle = null;
        //private CheckBox MultiplyToLevel = null;

        private Label ClassNameTitle = null;
        private Frame ClassNameFrame = null;
        private Label ClassName = null;

        private Label AutoNamingTitle = null;
        private CheckBox AutoNaming = null;

        private Button Cancel = null;
        private Button Save = null;
        private Button Delete = null;

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

            TotalTitle = CreateLabel("Total:");
            var totalFrame = CreateFrame(string.Empty);
            totalFrame.BackgroundColor = Color.LightGray;
            TotalValue = totalFrame.Content as Label;

            var row = 0;
            grid.Children.Add(TotalTitle, 0, row);
            grid.Children.Add(totalFrame, 1, row);
            row += 1;

            IsActiveTitle = CreateLabel("Active:");
            IsActive = new CheckBox()
            {
                IsChecked = true,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };

            grid.Children.Add(IsActiveTitle, 0, row);
            grid.Children.Add(IsActive, 1, row);
            row += 1;

            ValueTitle = CreateLabel("Value:");
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

            grid.Children.Add(ValueTitle, 0, row);
            grid.Children.Add(ModifierValueFrame, 1, row);
            row += 1;

            NameTitle = CreateLabel("Name: ");
            ModifierName = new Entry()
            {
                HorizontalTextAlignment = TextAlignment.Center,
            };
            /*
            ModifierName.TextChanged += (s, e) =>
            {
                UpdateValue();
            };
            */
            ModifierNameFrame = new Frame()
            {
                Content = ModifierName,
                BorderColor = Color.Black,
                Padding = 5,
            };

            grid.Children.Add(NameTitle, 0, row);
            grid.Children.Add(ModifierNameFrame, 1, row);
            row += 1;

            if (allowUseAbilities)
            {
                AbilityTitle = CreateLabel("Ability: ");
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
                var abilities = new List<CharacterSheet.AbilityPickerItem>();
                var values = Enum.GetValues(typeof(CharacterSheet.Ability));
                var index = -1;
                var selectedIndex = -1;
                var selectedValue = modifier != null ? modifier.SourceAbility : CharacterSheet.Ability.None;
                foreach (var v in values)
                {
                    var value = (CharacterSheet.Ability)v;
                    if (value == CharacterSheet.Ability.Total)
                        continue;
                    index += 1;
                    if (selectedValue == value)
                        selectedIndex = index;
                    abilities.Add(new CharacterSheet.AbilityPickerItem()
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

                grid.Children.Add(AbilityTitle, 0, row);
                grid.Children.Add(abilityFrame, 1, row);
                row += 1;

                MultiplierTitle = CreateLabel("Multiplier: ");
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

                grid.Children.Add(MultiplierTitle, 0, row);
                grid.Children.Add(MultiplierFrame, 1, row);
                row += 1;

                DividerTitle = CreateLabel("Divider: ");
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
                grid.Children.Add(DividerTitle, 0, row);
                grid.Children.Add(DividerFrame, 1, row);
                row += 1;

                RoundingTitle = CreateLabel("Rounding: ");
                Rounding = new Picker()
                {
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)),
                    ItemDisplayBinding = new Binding("Name"),
                };
                RoundingFrame = new Frame()
                {
                    Content = Rounding,
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var roundingTypes = new List<RoundingTypesPickerItem>();
                var roundingValues = Enum.GetValues(typeof(CharacterSheet.IntModifier.RoundingTypes));
                var roundingIndex = -1;
                var roundingSelectedIndex = -1;
                var roundingSelectedValue = modifier != null ? modifier.RoundingType : CharacterSheet.IntModifier.DefaultRounding;
                foreach (var v in roundingValues)
                {
                    var value = (CharacterSheet.IntModifier.RoundingTypes)v;
                    roundingIndex += 1;
                    if (roundingSelectedValue == value)
                        roundingSelectedIndex = roundingIndex;
                    roundingTypes.Add(new RoundingTypesPickerItem()
                    {
                        Name = v.ToString(),
                        Value = value,
                    });
                }
                Rounding.ItemsSource = roundingTypes;
                Rounding.SelectedIndex = roundingSelectedIndex;
                Rounding.SelectedIndexChanged += (s, e) =>
                {
                    UpdateValue();
                };

                grid.Children.Add(RoundingTitle, 0, row);
                grid.Children.Add(RoundingFrame, 1, row);
                row += 1;
            }

            LinkedItemTitle = CreateLabel("Linked To Item: ");
            LinkedItemFrame = CreateFrame(string.Empty);
            LinkedItem = LinkedItemFrame.Content as Label;

            grid.Children.Add(LinkedItemTitle, 0, row);
            grid.Children.Add(LinkedItemFrame, 1, row);
            row += 1;

            ItemMustBeActiveTitle = CreateLabel("Item Must Be Active: ");
            ItemMustBeActive = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            ItemMustBeActive.CheckedChanged += (s, e) =>
            {
                modifier.mustBeActive = e.Value;
                UpdateView();
            };

            grid.Children.Add(ItemMustBeActiveTitle, 0, row);
            grid.Children.Add(ItemMustBeActive, 1, row);
            row += 1;

            /*
            MultiplyToLevelTitle = CreateLabel("Multiply To Level:");
            MultiplyToLevel = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            MultiplyToLevel.CheckedChanged += (s, e) =>
            {
                modifier.multiplyToLevel = e.Value;
                UpdateView();
            };

            grid.Children.Add(MultiplyToLevelTitle, 0, row);
            grid.Children.Add(MultiplyToLevel, 1, row);
            row += 1;
            */

            ClassNameTitle = CreateLabel("Level Of Class: ");
            ClassNameFrame = CreateFrame(string.Empty);
            ClassName = ClassNameFrame.Content as Label;

            grid.Children.Add(ClassNameTitle, 0, row);
            grid.Children.Add(ClassNameFrame, 1, row);
            row += 1;

            AutoNamingTitle = CreateLabel("Auto Naming:");
            AutoNaming = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };

            grid.Children.Add(AutoNamingTitle, 0, row);
            grid.Children.Add(AutoNaming, 1, row);
            row += 1;

            grid.RowDefinitions = new RowDefinitionCollection();
            for (var i = 0; i < row; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

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

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        public void Init(CharacterSheet sheet, CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiersList, CharacterSheet.IntModifier modifier, bool allowUseAbilities = true)
        {
            this.sheet = sheet;
            this.modifiersList = modifiersList;
            source = modifier;
            if (modifier != null)
                this.modifier = modifier.Clone as CharacterSheet.IntModifier;
            else
                this.modifier = new CharacterSheet.IntModifier();
            InitControls(allowUseAbilities);
            IsActive.IsChecked = this.modifier.IsActive;
            ItemMustBeActive.IsChecked = this.modifier.mustBeActive;
            ModifierValue.Text = this.modifier.value.ToString();
            ModifierName.Text = this.modifier.Name;
            Delete.IsEnabled = source != null;
            if (Multiplier != null)
                Multiplier.Text = this.modifier.multiplier.ToString();
            if (Divider != null)
                Divider.Text = this.modifier.divider.ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (sheet == null)
                return;
            UpdateValue();
            if (modifier == null)
                return;
            CharacterSheet.GearItem item = null;
            if (modifier.sourceItemUID == CharacterSheet.InvalidUID)
                LinkedItem.Text = string.Empty;
            else
            {
                item = sheet.GetItemByUID(modifier.sourceItemUID);
                if (item != null)
                {
                    LinkedItem.Text = item.AsString(sheet);
                    LinkedItem.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
                    LinkedItem.TextColor = (item.active || !modifier.mustBeActive) ? Color.Green : Color.Red;
                }
                else
                {
                    LinkedItem.Text = "Item not found";
                    LinkedItem.TextColor = Color.Red;
                }
            }
            MainPage.SetTapHandler(LinkedItemFrame, (s, e) => SelectItem(item));
            if (!modifier.multiplyToLevel)
                ClassName.Text = string.Empty;
            else
            {
                var loc = sheet.GetLevelOfClass(modifier.className);
                if (string.IsNullOrWhiteSpace(modifier.className) || (loc != null))
                {
                    ClassName.Text = loc == null ? "Total Level" : modifier.className;
                    ClassName.TextColor = Color.Green;
                }
                else
                {
                    ClassName.Text = "Class \"" + modifier.className + "\" not found";
                    ClassName.TextColor = Color.Red;
                }
            }
            MainPage.SetTapHandler(ClassNameFrame, (s, e) => SelectClass(modifier.className));
            AutoNaming.IsChecked = modifier.autoNaming;
        }

        private void SelectItem(CharacterSheet.GearItem item)
        {
            if (pushedPage != null)
                return;
            var sgi = new SelectGearItem();
            sgi.InitSelection((selected) =>
            {
                modifier.sourceItemUID = selected == null ? CharacterSheet.InvalidUID : selected.uid;
            }, item);
            pushedPage = sgi;
            Navigation.PushAsync(pushedPage);
        }

        private void SelectClass(string className)
        {
            if (pushedPage != null)
                return;
            var sc = new SelectClass();
            var level = modifier.multiplyToLevel ? new CharacterSheet.LevelOfClass() { className = className }: null;
            sc.InitSelection((selected) =>
            {
                if (selected == null)
                    modifier.multiplyToLevel = false;
                else
                {
                    modifier.multiplyToLevel = true;
                    modifier.className = selected.ClassName;
                }
            }, level);
            pushedPage = sc;
            Navigation.PushAsync(pushedPage);
        }

        private void UpdateValue()
        {
            if (sheet == null)
                return;
            var ab = false;
            var currentAbility = CharacterSheet.Ability.None;
            if (Ability != null)
            {
                var item = (Ability.SelectedItem as CharacterSheet.AbilityPickerItem);
                if (item != null)
                {
                    currentAbility = item.Value;
                    ab = currentAbility != CharacterSheet.Ability.None;
                }
            }
            modifier.SourceAbility = currentAbility;
            MainPage.StrToInt(ModifierValue.Text, ref modifier.value);
            if (Multiplier != null)
                MainPage.StrToInt(Multiplier.Text, ref modifier.multiplier);
            if (Divider != null)
                MainPage.StrToInt(Divider.Text, ref modifier.divider);
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
            if (Rounding != null)
            {
                Rounding.InputTransparent = !ab;
                RoundingFrame.BackgroundColor = !ab ? Color.LightGray : Color.White;
            }
            var currentRoundingType = CharacterSheet.IntModifier.DefaultRounding;
            if (Rounding != null)
            {
                var item = (Rounding.SelectedItem as RoundingTypesPickerItem);
                if (item != null)
                    currentRoundingType = item.Value;
            }
            modifier.RoundingType = currentRoundingType;
            TotalValue.Text = modifier.GetValue(sheet).ToString();
        }

        private void EditToView()
        {
            if (source == null)
                modifiersList.Add(modifier);
            modifier.active = IsActive.IsChecked;
            MainPage.StrToInt(ModifierValue.Text, ref modifier.value);
            modifier.name = ModifierName.Text;
            if (Ability != null)
            {
                var selectedItem = Ability.SelectedItem as CharacterSheet.AbilityPickerItem;
                if (selectedItem != null)
                    modifier.SourceAbility = selectedItem.Value;
            }
            if (Multiplier != null)
                MainPage.StrToInt(Multiplier.Text, ref modifier.multiplier);
            if (Divider != null)
                MainPage.StrToInt(Divider.Text, ref modifier.divider);
            if (Rounding != null)
            {
                var selectedItem = Rounding.SelectedItem as RoundingTypesPickerItem;
                if (selectedItem != null)
                    modifier.RoundingType = selectedItem.Value;
            }
            modifier.autoNaming = AutoNaming.IsChecked;
            if ((source != null) && !source.Equals(modifier))
                source.Fill(modifier);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
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
                modifiersList.Remove(source);
                await Navigation.PopAsync();
            }
        }
    }
}