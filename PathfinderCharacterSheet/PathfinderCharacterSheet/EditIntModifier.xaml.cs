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
	public partial class EditIntModifier : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private IntModifiersList modifiersList = null;
        private IntModifier source = null;
        private IntModifier modifier = null;
        private static readonly IntMultiplier emptyMultiplier = new IntMultiplier();

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
        private Picker AbilityPicker = null;

        private Label AbilityMultiplierTitle = null;
        private Frame AbilityMultiplierFrame = null;
        private Label AbilityMultiplier = null;

        /*
        private Label MultiplierTitle = null;
        private Frame MultiplierFrame = null;
        private Entry Multiplier = null;

        private Label DividerTitle = null;
        private Frame DividerFrame = null;
        private Entry Divider = null;

        private Label RoundingTitle = null;
        private Frame RoundingFrame = null;
        private Picker Rounding = null;
        */
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

        private Label LevelMultiplierTitle = null;
        private Frame LevelMultiplierFrame = null;
        private Label LevelMultiplier = null;

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

            NameTitle = CreateLabel("Name:");
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
                AbilityTitle = CreateLabel("Ability:");
                AbilityPicker = new Picker()
                {
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)),
                    ItemDisplayBinding = new Binding("Name"),
                };
                var abilityFrame = new Frame()
                {
                    Content = AbilityPicker,
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var abilities = new List<AbilityPickerItem>();
                var values = Enum.GetValues(typeof(Ability));
                var index = -1;
                var selectedIndex = -1;
                var selectedValue = modifier != null ? modifier.SourceAbility : IntModifier.DefaultSourceAbility;
                foreach (var v in values)
                {
                    var value = (Ability)v;
                    if (value == Ability.Total)
                        continue;
                    index += 1;
                    if (selectedValue == value)
                        selectedIndex = index;
                    abilities.Add(new AbilityPickerItem()
                    {
                        Name = v.ToString(),
                        Value = value,
                    });
                }
                AbilityPicker.ItemsSource = abilities;
                AbilityPicker.SelectedIndex = selectedIndex;
                AbilityPicker.SelectedIndexChanged += (s, e) =>
                {
                    UpdateValue();
                };

                grid.Children.Add(AbilityTitle, 0, row);
                grid.Children.Add(abilityFrame, 1, row);
                row += 1;

                AbilityMultiplierTitle = CreateLabel("Ability Multiplier:");
                AbilityMultiplierFrame = CreateFrame(string.Empty);
                AbilityMultiplier = AbilityMultiplierFrame.Content as Label;
                MainPage.SetTapHandler(AbilityMultiplierFrame, (s, e) =>
                {
                    if (modifier.abilityMultiplier == null)
                        modifier.abilityMultiplier = emptyMultiplier.Clone;
                    EditMultiplier(modifier.abilityMultiplier);
                });

                grid.Children.Add(AbilityMultiplierTitle, 0, row);
                grid.Children.Add(AbilityMultiplierFrame, 1, row);
                row += 1;
            }

            LinkedItemTitle = CreateLabel("Linked To Item:");
            LinkedItemFrame = CreateFrame(string.Empty);
            LinkedItem = LinkedItemFrame.Content as Label;

            grid.Children.Add(LinkedItemTitle, 0, row);
            grid.Children.Add(LinkedItemFrame, 1, row);
            row += 1;

            ItemMustBeActiveTitle = CreateLabel("Item Must Be Active:");
            ItemMustBeActive = new CheckBox()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            ItemMustBeActive.CheckedChanged += (s, e) =>
            {
                modifier.MustBeActive = e.Value;
                UpdateView();
            };

            grid.Children.Add(ItemMustBeActiveTitle, 0, row);
            grid.Children.Add(ItemMustBeActive, 1, row);
            row += 1;

            ClassNameTitle = CreateLabel("Level Of Class:");
            ClassNameFrame = CreateFrame(string.Empty);
            ClassName = ClassNameFrame.Content as Label;

            grid.Children.Add(ClassNameTitle, 0, row);
            grid.Children.Add(ClassNameFrame, 1, row);
            row += 1;

            LevelMultiplierTitle = CreateLabel("Level Multiplier:");
            LevelMultiplierFrame = CreateFrame(string.Empty);
            LevelMultiplier = LevelMultiplierFrame.Content as Label;
            MainPage.SetTapHandler(LevelMultiplierFrame, (s, e) =>
            {
                if (modifier.levelMultiplier == null)
                    modifier.levelMultiplier = emptyMultiplier.Clone;
                EditMultiplier(modifier.levelMultiplier);
            });

            grid.Children.Add(LevelMultiplierTitle, 0, row);
            grid.Children.Add(LevelMultiplierFrame, 1, row);
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

        public void Init(CharacterSheet sheet, IntModifiersList modifiersList, IntModifier modifier, bool allowUseAbilities = true)
        {
            this.sheet = sheet;
            this.modifiersList = modifiersList;
            source = modifier;
            if (modifier != null)
                this.modifier = modifier.Clone as IntModifier;
            else
                this.modifier = new IntModifier();
            InitControls(allowUseAbilities);
            IsActive.IsChecked = this.modifier.active;
            ItemMustBeActive.IsChecked = this.modifier.MustBeActive;
            ModifierValue.Text = this.modifier.value.ToString();
            ModifierName.Text = this.modifier.name;
            Delete.IsEnabled = source != null;
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

            GearItem item = null;
            if (modifier.SourceItemUID == CharacterSheet.InvalidUID)
                LinkedItem.Text = string.Empty;
            else
            {
                item = sheet.GetItemByUID(modifier.SourceItemUID);
                if (item != null)
                {
                    LinkedItem.Text = item.AsString(sheet);
                    LinkedItem.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
                    LinkedItem.TextColor = (item.active || !modifier.MustBeActive) ? Color.Green : Color.Red;
                }
                else
                {
                    LinkedItem.Text = "Item not found";
                    LinkedItem.TextColor = Color.Red;
                }
            }
            MainPage.SetTapHandler(LinkedItemFrame, (s, e) => SelectItem(item));
            var loc = sheet.GetLevelOfClass(modifier.className);
            if (!modifier.MultiplyToLevel)
                ClassName.Text = string.Empty;
            else
            {
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

            var lms = loc == null ? "Total Level" : "Level Of " + modifier.className;
            var lm = modifier.levelMultiplier?.AsString(lms) ?? string.Empty;
            if (lm == lms)
                lm = string.Empty;
            LevelMultiplier.Text = !modifier.MultiplyToLevel ? string.Empty : lm;
            LevelMultiplierFrame.BackgroundColor = modifier.MultiplyToLevel ? Color.White : Color.LightGray;
            LevelMultiplierFrame.InputTransparent = !modifier.MultiplyToLevel;

            MainPage.SetTapHandler(ClassNameFrame, (s, e) => SelectClass(modifier.className));
            AutoNaming.IsChecked = modifier.AutoNaming;
        }

        private void EditMultiplier(IntMultiplier multiplier)
        {
            if (pushedPage != null)
                return;
            var eim = new EditIntMultiplier();
            eim.Init(multiplier);
            pushedPage = eim;
            Navigation.PushAsync(pushedPage);
        }

        private void SelectItem(GearItem item)
        {
            if (pushedPage != null)
                return;
            var sgi = new SelectGearItem();
            sgi.InitSelection((selected) =>
            {
                modifier.SourceItemUID = selected == null ? CharacterSheet.InvalidUID : selected.uid;
            }, item);
            pushedPage = sgi;
            Navigation.PushAsync(pushedPage);
        }

        private void SelectClass(string className)
        {
            if (pushedPage != null)
                return;
            var sc = new SelectClass();
            var level = modifier.MultiplyToLevel ? new LevelOfClass() { className = className }: null;
            sc.InitSelection((selected) =>
            {
                if (selected == null)
                    modifier.MultiplyToLevel = false;
                else
                {
                    modifier.MultiplyToLevel = true;
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
            if (AbilityPicker != null)
            {
                var currentAbility = Ability.None;
                var item = (AbilityPicker.SelectedItem as AbilityPickerItem);
                if (item != null)
                    currentAbility = item.Value;
                modifier.SourceAbility = currentAbility;

                var sab = modifier.SourceAbility != IntModifier.DefaultSourceAbility;
                var ams = modifier.sourceAbility;
                var am = modifier.abilityMultiplier?.AsString(ams) ?? string.Empty;
                if (am == ams)
                    am = string.Empty;
                AbilityMultiplier.Text = !sab ? string.Empty : am;
                AbilityMultiplierFrame.BackgroundColor = sab ? Color.White : Color.LightGray;
                AbilityMultiplierFrame.InputTransparent = !sab;
            }

            MainPage.StrToInt(ModifierValue.Text, ref modifier.value);
            TotalValue.Text = modifier.GetValue(sheet).ToString();
        }

        private void EditToView()
        {
            if (source == null)
                modifiersList.Add(modifier);
            modifier.active = IsActive.IsChecked;
            MainPage.StrToInt(ModifierValue.Text, ref modifier.value);
            modifier.name = ModifierName.Text;
            if (AbilityPicker != null)
            {
                if (AbilityPicker.SelectedItem is AbilityPickerItem selectedItem)
                    modifier.SourceAbility = selectedItem.Value;
            }
            /*
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
            */
            modifier.AutoNaming = AutoNaming.IsChecked;

            if (modifier.abilityMultiplier == emptyMultiplier)
                modifier.abilityMultiplier = null;
            if (modifier.levelMultiplier == emptyMultiplier)
                modifier.levelMultiplier = null;

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
            if ((modifier != null) && !string.IsNullOrWhiteSpace(modifier.name))
                modifierName = " \"" + modifier.name + "\"";
            bool allow = await DisplayAlert("Remove modifier" + modifierName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                modifiersList.Remove(source);
                await Navigation.PopAsync();
            }
        }
    }
}