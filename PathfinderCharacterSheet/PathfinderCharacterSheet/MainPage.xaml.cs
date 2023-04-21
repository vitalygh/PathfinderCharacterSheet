#define SHOW_ERROR_MESSAGES
#define SHOW_ERROR_DETAILS
//#define DEBUG_DISABLE_UPDATE_WHEN_PAGE_CHANGED
//#define LONG_TAP_INSTEAD_OF_DOUBLE_TAP
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    public partial class MainPage : ContentPage, ISheetView
    {
        readonly NewCharacter newCharacter = new NewCharacter();
        readonly CharacterSheetTabs tabs = new CharacterSheetTabs();
        public static readonly double DefaultLabelTextSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        public static readonly double DefaultButtonTextSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button));

        public static readonly Func<int> GetUID = () => CharacterSheetStorage.Instance.GetUID();
        public static readonly Func<CharacterSheet> GetSelectedCharacter = () => CharacterSheetStorage.Instance.selectedCharacter;
        public static readonly Action<CharacterSheet> SetSelectedCharacter = (sheet) => CharacterSheetStorage.Instance.selectedCharacter = sheet;
        public static readonly Action SaveSelectedCharacter = CharacterSheetStorage.Instance.SaveCharacter;
        public static readonly Action<CharacterSheet> SaveCharacter = (sheet) => CharacterSheetStorage.Instance.SaveCharacter(sheet);
        public static readonly Action<CharacterSheet> DeleteCharacter = (sheet) => CharacterSheetStorage.Instance.DeleteCharacter(sheet);

        public MainPage()
        {
            InitializeComponent();
#if SHOW_ERROR_MESSAGES
            var failedToLoad = new List<KeyValuePair<string, Exception>>();
            var loadedFromBackup = new List<KeyValuePair<string, string>>();
            CharacterSheetStorage.Instance.onCharacterSavingFailed += (character, path, ex) =>
            {
                var message = "Character saving failed!";
#if SHOW_ERROR_DETAILS
                if (character != null)
                    message += "\n\nCharacter: " + character;
                if (path != null)
                    message += "\n\nPath: " + path;
                if (ex != null)
                    message += "\n\nException: " + ex.ToString();
#endif
                DisplayAlert("Error", message, "Noooooooo!!!");
            };
            CharacterSheetStorage.Instance.onCharacterLoadingFailed += (path, ex) =>
            {
                failedToLoad.Add(new KeyValuePair<string, Exception>(path, ex));
            };
            CharacterSheetStorage.Instance.onCharacterLoadedFromBackup += (path, backup) =>
            {
                loadedFromBackup.Add(new KeyValuePair<string, string>(path, backup));
            };
            CharacterSheetStorage.Instance.onBackupSavingFailed += (from, to, ex) =>
            {
                var message = "Backup saving failed!";
#if SHOW_ERROR_DETAILS
                if (from != null)
                    message += "\n\nFrom: " + from;
                if (to != null)
                    message += "\n\nTo: " + to;
                if (ex != null)
                    message += "\n\nException: " + ex.ToString();
#endif
                DisplayAlert("Error", message, "So sad...");
            };
            CharacterSheetStorage.Instance.onBackupRemovingFailed += (file, ex) =>
            {
                var message = "Old backup removing failed!";
#if SHOW_ERROR_DETAILS
                if (file != null)
                    message += "\n\nFile: " + file;
                if (ex != null)
                    message += "\n\nException: " + ex.ToString();
#endif
                DisplayAlert("Error", message, "Interesting...");
            };
#endif
            CharacterSheetStorage.Instance.LoadCharacters();
#if SHOW_ERROR_MESSAGES
            if ((failedToLoad.Count > 0) || (loadedFromBackup.Count > 0))
            {
                var message = "Some characters loading failed!";
#if SHOW_ERROR_DETAILS
                if (loadedFromBackup.Count > 0)
                    message += "\n";
                foreach (var kvp in loadedFromBackup)
                    message += "\nCharacter " + kvp.Key + " loaded from backup " + kvp.Value + "\n";
                foreach (var kvp in failedToLoad)
                {
                    if (kvp.Key != null)
                        message += "\n\nFailed to load: " + kvp.Key;
                    if (kvp.Value != null)
                        message += "\n\nException: " + kvp.Value.ToString();
                }
#endif
                DisplayAlert("Error", message, "Unthinkable!");
            }
#endif
            UpdateView();
#if !DEBUG_DISABLE_UPDATE_WHEN_PAGE_CHANGED
            tabs.CurrentPageChanged += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    tabs.MoveTabs();
                    var sheet = GetSelectedCharacter?.Invoke();
                    if (sheet != null)
                    {
                        var title = sheet.Name;
                        if (tabs.CurrentPage != null)
                            title += ": " + tabs.CurrentPage.Title;
                        tabs.Title = title;
                    }
                    var view = (tabs.CurrentPage as ISheetView);
                    if (view != null)
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
            var gridTitle = CreateLabel("Characters", TextAlignment.Center);
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
            var characters = new List<CharacterSheet>(CharacterSheetStorage.Instance.characters.Keys);
            characters.Sort((a, b) => a.Name.CompareTo(b.Name));
             var count = characters.Count;
            if (count <= 0)
                return;
            grid.Children.Add(CreateLabel("Name", TextAlignment.Center), 0, 1);
            grid.Children.Add(CreateLabel("Race", TextAlignment.Center), 1, 1);
            grid.Children.Add(CreateLabel("Level", TextAlignment.Center), 2, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var sheet = characters[i];
                var name = CreateFrame(sheet.Name.ToString());
                AddTapHandler(name, (s, e) => SelectCharacter(sheet), 1);
                grid.Children.Add(name, 0, index);
                var race = CreateFrame(sheet.Race.ToString());
                AddTapHandler(race, (s, e) => SelectCharacter(sheet), 1);
                grid.Children.Add(race, 1, index);
                var level = CreateFrame(sheet.TotalLevel.ToString());
                AddTapHandler(level, (s, e) => SelectCharacter(sheet), 1);
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

        private void Characters_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SelectCharacter(e.Item as CharacterSheet);
        }

        private void SelectCharacter(CharacterSheet sheet)
        {
            Characters.IsVisible = false;
            CharacterSheetStorage.Instance.selectedCharacter = sheet;
            tabs.InitTabs();
            var title = sheet.Name;
            if (tabs.CurrentPage != null) 
                title += ": " + tabs.CurrentPage.Title;
            tabs.Title = title;
            Navigation.PushAsync(tabs);
        }

        public static bool StrToInt(string from, ref int to)
        {
            if (int.TryParse(from, out int i))
            {
                var changed = to != i;
                to = i;
                return changed;
            }
            return false;
        }

        public static Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return new Label()
            {
                Text = text,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = horz,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(5, 0, 5, 0),
            };
        }

        public static Frame CreateFrame(string text)
        {
            return new Frame()
            {
                Content = CreateLabel(text, TextAlignment.Center),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BorderColor = Color.Black,
                Padding = 5,
            };
        }

        public static void SetTapHandler(View view, Action handler, int tapCount = 1)
        {
            SetTapHandler(view, (s, e) => handler?.Invoke(), tapCount);
        }

        public static void SetTapHandler(View view, EventHandler handler, int tapCount = 1)
        {
            view.GestureRecognizers.Clear();
#if LONG_TAP_INSTEAD_OF_DOUBLE_TAP
            view.Effects.Clear();
#endif
            AddTapHandler(view, handler, tapCount);
        }

        public static void AddTapHandler(View view, Action handler, int tapCount = 1)
        {
            AddTapHandler(view, (s, e) => handler?.Invoke(), tapCount);
        }

        public static void AddTapHandler(View view, EventHandler handler, int tapCount = 1)
        {
            if (view == null)
                return;
#if LONG_TAP_INSTEAD_OF_DOUBLE_TAP
            if (tapCount > 1)
            {
                AddLongTapHandler(view, () => handler?.Invoke(view, new EventArgs()));
                return;
            }
#endif
            var tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = tapCount,
            };
            tgr.Tapped += handler;
            view.GestureRecognizers.Add(tgr);
        }

        public static void SetLongTapHandler(View view, Action handler)
        {
            view.Effects.Clear();
            AddLongTapHandler(view, handler);
        }

        public static void AddLongTapHandler(View view, Action handler)
        {
            var lte = new LongPressedEffect();
            view.Effects.Add(lte);
            LongPressedEffect.SetAction(view, handler);
        }

        public static void FillIntMLGrid(Grid grid, CharacterSheet sheet, IntModifiersList modifiers, string title,
                                                Action<IntModifiersList> addModifier,
                                                Action<IntModifiersList, IntModifier> editModifier,
                                                Action<IntModifiersList> reorderModifiers,
                                                Action<IntModifiersList, IntModifier> activateModifier)
        {
            grid.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            if ((reorderModifiers != null) && modifiers.Count > 1)
            {
                var reorderModifiersButton = new Button()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.Start,
                    Text = "Reorder",
                };
                reorderModifiersButton.Clicked += (s, e) => reorderModifiers(modifiers);
                stack.Children.Add(reorderModifiersButton);
            }
            var stackTitle = CreateLabel(title, TextAlignment.Center);
            stack.Children.Add(stackTitle);
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
            grid.Children.Add(CreateLabel("Active", TextAlignment.Center), 0, 1);
            grid.Children.Add(CreateLabel("Value", TextAlignment.Center), 1, 1);
            grid.Children.Add(CreateLabel("Name", TextAlignment.Center), 2, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var t = modifiers[i];
                var active = new CheckBox()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    IsChecked = t.active,
                };
                active.CheckedChanged += (s, e) =>
                {
                    t.active = active.IsChecked;
                    if (activateModifier != null)
                        activateModifier.Invoke(modifiers, t);
                };
                grid.Children.Add(active, 0, index);
                var value = CreateFrame(t.GetValue(sheet).ToString());
                if (editModifier != null)
                {
                    var valueTapped = new TapGestureRecognizer();
                    valueTapped.Tapped += (s, e) => editModifier.Invoke(modifiers, t);
                    value.GestureRecognizers.Add(valueTapped);
                }
                grid.Children.Add(value, 1, index);
                var name = CreateFrame(t.AsString(sheet));
                if (editModifier != null)
                {
                    var nameTapped = new TapGestureRecognizer();
                    nameTapped.Tapped += (s, e) => editModifier.Invoke(modifiers, t);
                    name.GestureRecognizers.Add(nameTapped);
                }
                grid.Children.Add(name, 2, index);
            }
        }

        public static void UpdateParentGrid(View view)
        {
            if (view == null)
                return;
            var grid = (view.Parent as Grid);
            if (grid != null)
            {
                var r = Grid.GetRow(view);
                var c = Grid.GetColumn(view);
                var rs = Grid.GetRowSpan(view);
                var cs = Grid.GetColumnSpan(view);
                grid.Children.Remove(view);
                grid.Children.Add(view, c, c + cs, r, r + rs);
            }
            else
            {
                if (view.Parent is StackLayout sl)
                {
                    var index = sl.Children.IndexOf(view);
                    sl.Children.Remove(view);
                    sl.Children.Insert(index, view);
                }
            }
            UpdateParentGrid(view.Parent as View);
        }
    }
}
