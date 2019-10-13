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
                        var name = character.characterName;
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

        private string GeneratePath(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            var name = string.IsNullOrWhiteSpace(sheet.characterName) ? string.Empty : sheet.characterName;
            name += "_" + Guid.NewGuid().ToString();
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c, '_');
            return Path.Combine(CharactersPath, name + ".xml");
        }

        private void SaveCharacter(CharacterSheet sheet, string path)
        {
            if (sheet == null)
                return;
            var dir = CharactersPath;
            var characterName = string.IsNullOrWhiteSpace(sheet.characterName) ? "unnamed character" : "character \"" + sheet.characterName + "\"";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    var serializer = new XmlSerializer(typeof(CharacterSheet));
                    serializer.Serialize(writer, sheet);
                    writer.Flush();
                }
                Console.WriteLine("Serialization " + characterName + " to \"" + path + "\" complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialization " + characterName + " to \"" + path + "\" failed: " + ex.ToString());
            }
        }

        public void SaveCharacter(CharacterSheet sheet)
        {
            var path = GetPath(sheet);
            if (path != null)
            {
                SaveCharacter(sheet, path);
                return;
            }
            path = GeneratePath(sheet);
            if (path != null)
            {
                SaveCharacter(sheet, path);
                characters.Add(sheet, path);
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
