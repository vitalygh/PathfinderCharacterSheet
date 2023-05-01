//#define DEBUG_DISABLE_UPDATE_WHEN_PAGE_CHANGED
using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PathfinderCharacterSheet
{
    public partial class MainPage : ContentPage, ISheetView
    {
        private readonly NewCharacter newCharacter = new NewCharacter();
        private readonly EditSettings editSettings = new EditSettings();
        private readonly CharacterSheetTabs tabs = new CharacterSheetTabs();
        public static readonly double DefaultLabelTextSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        public static readonly double DefaultButtonTextSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button));

        public MainPage()
        {
            InitializeComponent();
            UIMediator.Init((title, message, button) => DisplayAlert(title, message, button));
            UpdateView();
#if !DEBUG_DISABLE_UPDATE_WHEN_PAGE_CHANGED
            tabs.CurrentPageChanged += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    tabs.MoveTabs();
                    var sheet = UIMediator.GetSelectedCharacter?.Invoke();
                    if (sheet != null)
                    {
                        var title = sheet.Name;
                        if (tabs.CurrentPage != null)
                            title += ": " + tabs.CurrentPage.Title;
                        tabs.Title = title;
                    }
                    if (tabs.CurrentPage is ISheetView view)
                        view.UpdateView();
                });
            };
#endif
        }

        public void UpdateView()
        {
            Characters.IsVisible = true;
            var grid = Characters;
            grid.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            var settingsModifierButton = new Button()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                Text = "Settings",
            };
            settingsModifierButton.Clicked += (s, e) => ShowSettings();
            stack.Children.Add(settingsModifierButton);
            var gridTitle = UIHelpers.CreateLabel("Characters", TextAlignment.Center);
            stack.Children.Add(gridTitle);
            var addModifierButton = new Button()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.End,
                Text = "Add",
            };
            addModifierButton.Clicked += (s, e) => AddCharacter();
            stack.Children.Add(addModifierButton);
            grid.Children.Add(stack, 0, 3, 0, 1);
            var characters = new List<CharacterSheet>(CharacterSheetStorage.Instance.Characters);
            characters.Sort((a, b) => a.Name.CompareTo(b.Name));
             var count = characters.Count;
            if (count <= 0)
                return;
            grid.Children.Add(UIHelpers.CreateLabel("Name", TextAlignment.Center), 0, 1);
            grid.Children.Add(UIHelpers.CreateLabel("Race", TextAlignment.Center), 1, 1);
            grid.Children.Add(UIHelpers.CreateLabel("Level", TextAlignment.Center), 2, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var sheet = characters[i];
                var name = UIHelpers.CreateFrame(sheet.Name.ToString());
                UIHelpers.AddTapHandler(name, (s, e) => SelectCharacter(sheet), 1);
                grid.Children.Add(name, 0, index);
                var race = UIHelpers.CreateFrame(sheet.Race.ToString());
                UIHelpers.AddTapHandler(race, (s, e) => SelectCharacter(sheet), 1);
                grid.Children.Add(race, 1, index);
                var level = UIHelpers.CreateFrame(sheet.TotalLevel.ToString());
                UIHelpers.AddTapHandler(level, (s, e) => SelectCharacter(sheet), 1);
                grid.Children.Add(level, 2, index);
            }
        }

        private void Add_Clicked(object sender, EventArgs args)
        {
            AddCharacter();
        }

        private void AddCharacter()
        {
            Characters.IsVisible = false;
            newCharacter.UpdateView();
            Navigation.PushAsync(newCharacter);
        }

        private void ShowSettings()
        {
            Characters.IsVisible = false;
            editSettings.UpdateView();
            Navigation.PushAsync(editSettings);
        }

        private void Characters_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SelectCharacter(e.Item as CharacterSheet);
        }

        private void SelectCharacter(CharacterSheet sheet)
        {
            Characters.IsVisible = false;
            UIMediator.SetSelectedCharacter?.Invoke(sheet);
            CharacterSheetTabs.InitTabs();
            var title = sheet.Name;
            if (tabs.CurrentPage != null) 
                title += ": " + tabs.CurrentPage.Title;
            tabs.Title = title;
            Navigation.PushAsync(tabs);
        }
    }
}
