#define SAVE_BACKUPS
#define LOAD_FROM_BACKUPS
//#define VALIDATE_XML
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    sealed class CharacterSheetStorage
    {
        private static CharacterSheetStorage instance = null;

        private static string CharactersPath
        {
            get
            {
                string persistentDataPath = null;
                if (App.PlatformProxy != null)
                    persistentDataPath = App.PlatformProxy.PersistentDataPath;
                if (persistentDataPath == null)
                    persistentDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(persistentDataPath, "characters");
            }
        }

        public int MaxBackupsCount { get { return 5; } }

        private Dictionary<CharacterSheet, string> characters = null;
        public IEnumerable<CharacterSheet> Characters => characters?.Keys;
        private readonly HashSet<CharacterSheet> markedAsChanged = new HashSet<CharacterSheet>();

        private CharacterSheet selectedCharacter = null;
        public CharacterSheet SelectedCharacter
        {
            get => selectedCharacter;
            set { selectedCharacter = value; }
        }

        public Action<string, string, Exception> onCharacterSavingFailed = null;
        public Action<string, Exception> onCharacterLoadingFailed = null;
        public Action<string, string, Exception> onBackupSavingFailed = null;
        public Action<string, Exception> onBackupRemovingFailed = null;
        public Action<string, string> onCharacterLoadedFromBackup = null;

        private CharacterSheetStorage()
        {
        }

        static internal CharacterSheetStorage Instance
        {
            get
            {
                if (instance == null)
                    instance = new CharacterSheetStorage();
                return instance;
            }
        }

        public CharacterSheet LoadCharacter(string path)
        {
            try
            {
                using (var stream = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(CharacterSheet));
                    var character = serializer.Deserialize(stream) as CharacterSheet;
                    var name = character.Name;
                    name = string.IsNullOrWhiteSpace(name) ? "unnamed character" : "character \"" + name + "\"";
                    Console.WriteLine("Loaded " + name + " from file \"" + path + "\"");
                    return character;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loading character from file \"" + path + "\" failed: " + ex.ToString());
                onCharacterLoadingFailed?.Invoke(path, ex);
            }
            return null;
        }

        public void LoadCharacters(bool reload = false)
        {
            if (!reload && (characters != null))
                return;
            characters = new Dictionary<CharacterSheet, string>();
            var path = CharactersPath;
            if (!Directory.Exists(path))
                return;
            var files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            Console.WriteLine("Files: " + files.Length);
            foreach (var file in files)
            {
                if (file.EndsWith("_backup.xml"))
                    continue;
                var character = LoadCharacter(file);
                if (character != null)
                {
                    //IntModifier.Optimize();
                    //ValueWithIntModifiers.Optimize();
#if VALIDATE_XML
                    using (var memoryStream = new MemoryStream())
                    {
                        var serializer = new XmlSerializer(typeof(CharacterSheet));
                        serializer.Serialize(memoryStream, character);
                        memoryStream.Position = 0;
                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            var fileText = File.ReadAllText(file);
                            var streamText = streamReader.ReadToEnd();
                            if (fileText != streamText)
                                File.WriteAllText(file + ".validate", streamText);
                        }
                    }
#endif
                    characters.Add(character, file);
                    continue;
                }
#if LOAD_FROM_BACKUPS
                var backups = GetBackupsList(file);
                if (backups == null)
                    continue;
                var sorted = new List<string>(backups);
                sorted.Sort();
                var index = sorted.Count;
                while (--index >= 0)
                {
                    var backup = sorted[index];
                    var backupCharacter = LoadCharacter(backup);
                    if (backupCharacter != null)
                    {
                        characters.Add(backupCharacter, file);
                        onCharacterLoadedFromBackup?.Invoke(file, backup);
                        break;
                    }
                }
#endif
            }
        }

        private string GetPath(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            if ((characters != null) && characters.TryGetValue(sheet, out string path))
                return path;
            return null;
        }

        private string GeneratePath(CharacterSheet sheet, int index = 0)
        {
            if (sheet == null)
                return null;
            var name = string.IsNullOrWhiteSpace(sheet.Name) ? string.Empty : sheet.Name;
            //name += "_" + Guid.NewGuid().ToString();
            if (index > 0)
                name = name + "_" + index;
#if SAVE_BACKUPS
            name += "_pcs";
#endif
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c, '_');
            return Path.Combine(CharactersPath, name + ".xml");
        }

#if SAVE_BACKUPS || LOAD_FROM_BACKUPS
        private string[] GetBackupsList(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var dir = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var ext = Path.GetExtension(filename);
            if (!string.IsNullOrWhiteSpace(ext))
                filename = filename.Substring(0, filename.Length - ext.Length);
            var pattern = filename + "*_backup" + ext;
            return Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
        }
#endif

#if SAVE_BACKUPS
        private string GetBackupFilename(string path, int index)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var ext = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(ext))
                path = path.Substring(0, path.Length - ext.Length);
            var dt = DateTime.Now.ToString("yyyyMMddHHmmss");
            return path + "_" + dt + index.ToString("000") + "_backup" + ext;
        }

        private string GetBackupFilename(string path)
        {
            var index = 0;
            var filename = GetBackupFilename(path, index);
            while (index < 10)
            {
                if (!File.Exists(filename))
                    return filename;
                index += 1;
                filename = GetBackupFilename(path, index);
            }
            return filename;
        }

        private void SaveBackup(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;
            if (!File.Exists(path))
                return;
            var backup = GetBackupFilename(path);
            try
            {
                File.Copy(path, backup, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving backup of " + path + " to " + backup + " failed: " + ex.ToString());
                onBackupSavingFailed?.Invoke(path, backup, ex);
            }
        }

        private void RemoveOldBackups(string path)
        {
            var files = GetBackupsList(path);
            if (files == null)
                return;
            if (files.Length <= MaxBackupsCount)
                return;
            var sorted = new List<string>(files);
            sorted.Sort();
            while (sorted.Count > MaxBackupsCount)
            {
                var file = sorted[0];
                sorted.RemoveAt(0);
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Removing backup " + file + " failed: " + ex.ToString());
                    onBackupRemovingFailed?.Invoke(file, ex);
                }
            }
        }
#endif

        private bool SaveCharacter(CharacterSheet sheet, string path)
        {
            if (sheet == null)
                return false;
            var dir = CharactersPath;
            var characterName = string.IsNullOrWhiteSpace(sheet.Name) ? "unnamed character" : "character \"" + sheet.Name + "\"";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            try
            {
                sheet.ModificationTime = DateTime.Now;
                using (var memoryStream = new MemoryStream())
                {
                    var serializer = new XmlSerializer(typeof(CharacterSheet));
                    serializer.Serialize(memoryStream, sheet);
                    memoryStream.Position = 0;
#if SAVE_BACKUPS
                    SaveBackup(path);
#endif
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.WriteTo(fileStream);
                        fileStream.Flush();
                    }
#if SAVE_BACKUPS
                    RemoveOldBackups(path);
#endif
                }
                Console.WriteLine("Serialization " + characterName + " to \"" + path + "\" complete");
                markedAsChanged.Remove(sheet);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialization " + characterName + " to \"" + path + "\" failed: " + ex.ToString());
                onCharacterSavingFailed?.Invoke(characterName, path, ex);
            }
            return false;
        }

        public int GetUID()
        {
            if (selectedCharacter == null)
                return CharacterSheet.InvalidUID;
            return selectedCharacter.GetUID();
        }

        public void MarkSelectedCharacterAsChanged()
        {
            if (selectedCharacter is null)
                return;
            markedAsChanged.Add(selectedCharacter);
        }

        public void SaveChangedCharacters()
        {
            var changedCharacterSheets = new List<CharacterSheet>(markedAsChanged);
            foreach (var sheet in changedCharacterSheets)
                SaveCharacter(sheet);
        }

        public void SaveCharacter()
        {
            SaveCharacter(selectedCharacter);
        }

        public void SaveCharacter(CharacterSheet sheet)
        {
            var path = GetPath(sheet);
            if (path != null)
            {
                SaveCharacter(sheet, path);
                return;
            }
            var index = 0;
            path = GeneratePath(sheet, index);
            if (path == null)
                return;
            var safeCounter = 1000;
            while (safeCounter > 0)
            {
                safeCounter -= 1;
                if (File.Exists(path))
                {
                    index += 1;
                    path = GeneratePath(sheet, index);
                    continue;
                }
                if (SaveCharacter(sheet, path))
                {
                    characters.Add(sheet, path);
                    break;
                }
            }
        }

        public void DeleteCharacter(CharacterSheet sheet)
        {
            var path = GetPath(sheet);
            if (path == null)
                return;
            File.Delete(path);
            markedAsChanged.Remove(sheet);
            characters.Remove(sheet);
            if (selectedCharacter == sheet)
                selectedCharacter = null;
        }
    }
}
