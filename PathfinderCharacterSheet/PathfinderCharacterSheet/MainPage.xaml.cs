using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathfinderCharacterSheet
{
    public partial class MainPage : ContentPage, ISheetView
    {
        NewCharacter newCharacter = new NewCharacter();
        CharacterSheetTabs tabs = new CharacterSheetTabs();

        public MainPage()
        {
            InitializeComponent();
            CharacterSheetStorage.Instance.LoadCharacters();
            UpdateView();
        }

        public void UpdateView()
        {
            Characters.IsVisible = true;
            Characters.ItemsSource = null;
            Characters.ItemsSource = CharacterSheetStorage.Instance.characters.Keys;
        }

        private void Add_Clicked(object sender, EventArgs args)
        {
            Characters.IsVisible = false;
            newCharacter.UpdateView();
            Navigation.PushAsync(newCharacter);
        }

        private void Characters_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Characters.IsVisible = false;
            CharacterSheetStorage.Instance.selectedCharacter = e.Item as CharacterSheet;
            tabs.Title = CharacterSheetStorage.Instance.selectedCharacter.Name;
            tabs.UpdateView();
            Navigation.PushAsync(tabs);
        }

        public static bool StrToInt(string from, ref int to)
        {
            var i = 0;
            if (int.TryParse(from, out i))
            {
                var changed = to != i;
                to = i;
                return changed;
            }
            return false;
        }


        public static void InitIntModifierGrid(Grid grid, List<CharacterSheet.IntModifier> modifiers, string title, Action<List<CharacterSheet.IntModifier>> addModifier, Action<List<CharacterSheet.IntModifier>, CharacterSheet.IntModifier> editModifier)
        {
            grid.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            var tempHPTitle = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = title,
            };
            stack.Children.Add(tempHPTitle);
            if (addModifier != null)
            {
                var addModifierButton = new Button()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.End,
                    Text = "Add",
                };
                addModifierButton.Clicked += (s, e) => addModifier(modifiers);
                stack.Children.Add(addModifierButton);
            }
            grid.Children.Add(stack, 0, 3, 0, 1);
            var count = modifiers.Count;
            if (count <= 0)
                return;
            grid.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Active",
            }, 0, 1);
            grid.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Value",
            }, 1, 1);
            grid.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Name",
            }, 2, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var t = modifiers[i];
                var active = new CheckBox()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    IsChecked = t.IsActive,
                };
                active.CheckedChanged += (s, e) => t.active = active.IsChecked;
                grid.Children.Add(active, 0, index);
                var value = new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = t.Value.ToString(),
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                if (editModifier != null)
                {
                    var valueTapped = new TapGestureRecognizer();
                    valueTapped.Tapped += (s, e) => editModifier(modifiers, t);
                    value.GestureRecognizers.Add(valueTapped);
                }
                grid.Children.Add(value, 1, index);
                var name = new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = t.Name.ToString(),
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                if (editModifier != null)
                {
                    var nameTapped = new TapGestureRecognizer();
                    nameTapped.Tapped += (s, e) => editModifier(modifiers, t);
                    name.GestureRecognizers.Add(nameTapped);
                }
                grid.Children.Add(name, 2, index);
            }
        }
    }
}
