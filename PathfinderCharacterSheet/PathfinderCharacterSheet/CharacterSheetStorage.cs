using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    sealed class CharacterSheetStorage
    {
        private static CharacterSheetStorage instance = null;

        //private static string CharactersPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "characters"); } }
        private static string CharactersPath { get { return Path.Combine(App.PlatformProxy.PersistentDataPath, "characters"); } }

        public Dictionary<CharacterSheet, string> characters = null;
        public CharacterSheet selectedCharacter = null;
               
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
                try
                {
                    using (var stream = new StreamReader(file))
                    {
                        var serializer = new XmlSerializer(typeof(CharacterSheet));
                        var character = serializer.Deserialize(stream) as CharacterSheet;
                        characters.Add(character, file);
                        var name = character.Name;
                        name = string.IsNullOrWhiteSpace(name) ? "unnamed character" : "character \"" + name + "\"";
                        Console.WriteLine("Loaded " + name + " from file \"" + file + "\"");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("File \"" + file + "\" loading failed: " + ex.ToString());
                }
            }
        }

        private string GetPath(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            string path = null;
            if ((characters != null) && characters.TryGetValue(sheet, out path))
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
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c, '_');
            return Path.Combine(CharactersPath, name + ".xml");
        }

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
                using (var writer = new StreamWriter(path))
                {
                    var serializer = new XmlSerializer(typeof(CharacterSheet));
                    serializer.Serialize(writer, sheet);
                    writer.Flush();
                }
                Console.WriteLine("Serialization " + characterName + " to \"" + path + "\" complete");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialization " + characterName + " to \"" + path + "\" failed: " + ex.ToString());
            }
            return false;
        }

        public static int GetUID()
        {
            if (Instance == null)
                return CharacterSheet.InvalidUID;
            if (Instance.selectedCharacter == null)
                return CharacterSheet.InvalidUID;
            return Instance.selectedCharacter.GetUID();
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
                while (File.Exists(path))
                {
                    index += 1;
                    path = GeneratePath(sheet, index);
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
            characters.Remove(sheet);
            if (selectedCharacter == sheet)
                selectedCharacter = null;
        }
    }
}
