#define SHOW_ERROR_MESSAGES
#define SHOW_ERROR_DETAILS
//#define DEBUG_DISABLE_UPDATE_WHEN_PAGE_CHANGED
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    public partial class MainPage : ContentPage, ISheetView
    {
        private readonly NewCharacter newCharacter = new NewCharacter();
        private readonly EditSettings editSettings = new EditSettings();
        private readonly CharacterSheetTabs tabs = new CharacterSheetTabs();
        public static readonly double DefaultLabelTextSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        public static readonly double DefaultButtonTextSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button));

        private static Settings settings = new Settings();
        public static readonly Func<Settings> GetSettings = () => settings;
        public static readonly Action<Settings> SetSettings = (updatedSettings) =>
        {
            if (settings == updatedSettings)
                return;
            settings.Fill(updatedSettings);
            if (!settings.SaveChangesImmediately)
                return;
            settings.Save();
            CharacterSheetStorage.Instance.SaveChangedCharacters();
        };

        public static readonly Func<int> GetUID = () => CharacterSheetStorage.Instance.GetUID();
        public static readonly Func<CharacterSheet> GetSelectedCharacter = () => CharacterSheetStorage.Instance.SelectedCharacter;
        public static readonly Action<CharacterSheet> SetSelectedCharacter = (sheet) => CharacterSheetStorage.Instance.SelectedCharacter = sheet;
        public static readonly Action OnCharacterSheetChanged = () =>
        {
            if (!settings.SaveChangesImmediately)
                CharacterSheetStorage.Instance.MarkSelectedCharacterAsChanged();
            else
                CharacterSheetStorage.Instance.SaveCharacter();
        };
        public static readonly Action<CharacterSheet> SaveCharacter = (sheet) => CharacterSheetStorage.Instance.SaveCharacter(sheet);
        public static readonly Action<CharacterSheet> DeleteCharacter = (sheet) => CharacterSheetStorage.Instance.DeleteCharacter(sheet);

        public static readonly Action OnAppLostFocus = () =>
        {
            CharacterSheetStorage.Instance.SaveChangedCharacters();
            if (settings.IsChanged)
                settings.Save();
        };

        public static DateTime? GetBuildDateTime()
        {
            return GetBuildDateTime(Assembly.GetExecutingAssembly());
        }

        private static DateTime? GetBuildDateTime(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute?.DateTime;
        }

        public MainPage()
        {
            InitializeComponent();
            var failedToLoad = new List<KeyValuePair<string, Exception>>();
            var loadedFromBackup = new List<KeyValuePair<string, string>>();
            Settings.Serializer.OnLoadingFailed += (path, exception) =>
            {
                WriteToLog("Settings loading failed from path \"" + path + "\": " + exception.ToString());
            };
            Settings.Serializer.OnLoadingSuccess += (settings, path) =>
            {
                WriteToLog("Settings successful loaded from path \"" + path + "\"");
            };
            Settings.Serializer.OnSavingFailed += (name, path, exception) =>
            {
                WriteToLog("Settings saving failed to path \"" + path + "\": " + exception.ToString());
#if SHOW_ERROR_MESSAGES
                var message = "Settings saving failed!";
#if SHOW_ERROR_DETAILS
                if (path != null)
                    message += "\n\nPath: " + path;
                if (exception != null)
                    message += "\n\nException: " + exception.ToString();
#endif
                DisplayAlert("Error", message, "Hmmmm");
#endif
            };
            Settings.Serializer.OnSavingSuccess += (settings, path) =>
            {
                WriteToLog("Settings successful saved to path \"" + path + "\"");
            };
            CharacterSheetStorage.Instance.onCharacterLoadingSuccess += (name, path) =>
            {
                WriteToLog("Character \"" + name + "\" successful loaded from path \"" + path + "\"");
            };
            CharacterSheetStorage.Instance.onCharacterSavingSuccess += (name, path) =>
            {
                WriteToLog("Character \"" + name + "\" successful saved to path \"" + path + "\"");
            };
            CharacterSheetStorage.Instance.onCharacterSavingFailed += (name, path, ex) =>
            {
                WriteToLog("Character \"" + name +" saving to file \"" + path + "\" failed: " + ex.ToString());
#if SHOW_ERROR_MESSAGES
                var message = "Character saving failed!";
#if SHOW_ERROR_DETAILS
                if (name != null)
                    message += "\n\nCharacter: " + name;
                if (path != null)
                    message += "\n\nPath: " + path;
                if (ex != null)
                    message += "\n\nException: " + ex.ToString();
#endif
                DisplayAlert("Error", message, "Noooooooo!!!");
#endif
            };
            CharacterSheetStorage.Instance.onCharacterLoadingFailed += (path, ex) =>
            {
                WriteToLog("Character file \"" + path + "\" loading failed: " + ex.ToString());
#if SHOW_ERROR_MESSAGES
                failedToLoad.Add(new KeyValuePair<string, Exception>(path, ex));
#endif
            };
            CharacterSheetStorage.Instance.onCharacterLoadedFromBackup += (name, path, backup) =>
            {
                WriteToLog("Character \"" + name + "\" from path \"" + path + "\" loaded from backup \"" + backup + "\"");
#if SHOW_ERROR_MESSAGES
                loadedFromBackup.Add(new KeyValuePair<string, string>(path, backup));
#endif
            };
            CharacterSheetStorage.Instance.onBackupSavingFailed += (from, to, ex) =>
            {
                WriteToLog("Backup \"" + to + "\" of \"" + from + "\" saving failed: " + ex.ToString());
#if SHOW_ERROR_MESSAGES
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
#endif
            };
            CharacterSheetStorage.Instance.onBackupRemovingFailed += (file, ex) =>
            {
                WriteToLog("Backup \"" + file + "\" removing failed: " + ex.ToString());
#if SHOW_ERROR_MESSAGES
                var message = "Old backup removing failed!";
#if SHOW_ERROR_DETAILS
                if (file != null)
                    message += "\n\nFile: " + file;
                if (ex != null)
                    message += "\n\nException: " + ex.ToString();
#endif
                DisplayAlert("Error", message, "Interesting...");
#endif
            };
            settings.Fill(Settings.Load());
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
                    var view = tabs.CurrentPage as ISheetView;
                    if (view != null)
                        view.UpdateView();
                });
            };
#endif
        }

        private void WriteToLog(string message)
        {
            Console.WriteLine(message);
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
            SetSelectedCharacter?.Invoke(sheet);
            tabs.InitTabs();
            var title = sheet.Name;
            if (tabs.CurrentPage != null) 
                title += ": " + tabs.CurrentPage.Title;
            tabs.Title = title;
            Navigation.PushAsync(tabs);
        }
    }
}
