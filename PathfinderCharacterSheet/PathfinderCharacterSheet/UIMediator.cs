#define SHOW_ERROR_MESSAGES
#define SHOW_ERROR_DETAILS
using System;
using System.Collections.Generic;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    public static class UIMediator
    {
        public static void Init(Action<string, string, string> alert)
        {
            WriteToLog("Pathfinder Character Sheet build " + UIHelpers.GetBuildVersion() + " started");
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
                alert("Error", message, "Hmmmm");
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
                WriteToLog("Character \"" + name + " saving to file \"" + path + "\" failed: " + ex.ToString());
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
                alert("Error", message, "Noooooooo!!!");
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
                alert("Error", message, "So sad...");
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
                alert("Error", message, "Interesting...");
#endif
            };
            SettingsStorage.Instance.Load();
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
                alert("Error", message, "Unthinkable!");
            }
#endif
        }

        public static void WriteToLog(string message)
        {
            Console.WriteLine(message);
        }

        public static readonly Func<Settings> GetSettings = () => SettingsStorage.Instance.Settings;
        public static readonly Action<Settings> SetSettings = (updatedSettings) => SettingsStorage.Instance.Settings = updatedSettings;
        public static readonly Action SaveChangedsSettings = SettingsStorage.Instance.SaveChangedSettings;

        public static readonly Func<int> GetUID = () => CharacterSheetStorage.Instance.GetUID();
        public static readonly Func<CharacterSheet> GetSelectedCharacter = () => CharacterSheetStorage.Instance.SelectedCharacter;
        public static readonly Action<CharacterSheet> SetSelectedCharacter = (sheet) => CharacterSheetStorage.Instance.SelectedCharacter = sheet;
        public static readonly Action OnCharacterSheetChanged = () =>
        {
            if (!GetSettings().SaveChangesImmediately)
                CharacterSheetStorage.Instance.MarkSelectedCharacterAsChanged();
            else
                CharacterSheetStorage.Instance.SaveCharacter();
        };
        public static readonly Action SaveChangedCharacters = () => CharacterSheetStorage.Instance.SaveChangedCharacters();
        public static readonly Action<CharacterSheet> SaveCharacter = (sheet) => CharacterSheetStorage.Instance.SaveCharacter(sheet);
        public static readonly Action<CharacterSheet> DeleteCharacter = (sheet) => CharacterSheetStorage.Instance.DeleteCharacter(sheet);

        public static readonly Action OnAppLostFocus = () =>
        {
            SaveChangedCharacters?.Invoke();
            SaveChangedsSettings?.Invoke();
        };

    }
}
