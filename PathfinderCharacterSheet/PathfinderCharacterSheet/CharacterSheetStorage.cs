using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;

namespace PathfinderCharacterSheet
{
    sealed class CharacterSheetStorage
    {
        private static CharacterSheetStorage instance = null;

        private Dictionary<CharacterSheet, Tuple<string, ISerializer<CharacterSheet>>> characters = null;
        public IEnumerable<CharacterSheet> Characters => characters?.Keys;
        private readonly HashSet<CharacterSheet> markedAsChanged = new HashSet<CharacterSheet>();

        private CharacterSheet selectedCharacter = null;
        public CharacterSheet SelectedCharacter
        {
            get => selectedCharacter;
            set { selectedCharacter = value; }
        }

        private readonly ISerializer<CharacterSheet> serializer = null;
        private readonly ISerializer<CharacterSheet>[] deserializers = null;

        public Action<string, string> onCharacterLoadingSuccess = null;
        public Action<string, Exception> onCharacterLoadingFailed = null;
        public Action<string, string, string> onCharacterLoadedFromBackup = null;

        public Action<string, string> onCharacterSavingSuccess = null;
        public Action<string, string, Exception> onCharacterSavingFailed = null;
        public Action<string, string, Exception> onBackupSavingFailed = null;
        public Action<string, Exception> onBackupRemovingFailed = null;

        private CharacterSheetStorage()
        {
            serializer = new XmlFileSerializer<CharacterSheet>("characters")
            {
                SaveBackups = true,
                LoadFromBackups = true,
                CheckPathNotExist = true,

                OnLoadingSuccess = (data, file) => onCharacterLoadingSuccess?.Invoke(data?.Name, file),
                OnLoadingFailed = (path, exception) => onCharacterLoadingFailed?.Invoke(path, exception),
                OnLoadingFromBackup = (data, path, backup) => onCharacterLoadedFromBackup?.Invoke(data?.Name, path, backup),
                OnSavingSuccess = (data, path) => onCharacterSavingSuccess?.Invoke(data?.Name, path),
                OnSavingFailed = (name, path, exception) => onCharacterSavingFailed?.Invoke(name, path, exception),
                OnBackupSavingFailed = (path, backup, exception) => onBackupSavingFailed?.Invoke(path, backup, exception),
                OnBackupRemovingFailed = (path, exception) => onBackupRemovingFailed?.Invoke(path, exception),
            };

            deserializers = new ISerializer<CharacterSheet>[]
            {
                serializer,
            };
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

        public void LoadCharacters(bool reload = false)
        {
            if (!reload && characters != null)
                return;
            markedAsChanged.Clear();
            characters = new Dictionary<CharacterSheet, Tuple<string, ISerializer<CharacterSheet>>>();
            foreach (var deserializer in deserializers)
            {
                var loaded = deserializer.Load();
                foreach (var kvp in loaded)
                    characters.Add(kvp.Key, new Tuple<string, ISerializer<CharacterSheet>>(kvp.Value, deserializer));
            }
        }

        public void SaveCharacter(CharacterSheet sheet)
        {
            if (sheet == null)
                return;
            characters.TryGetValue(sheet, out Tuple<string, ISerializer<CharacterSheet>> loadedAs);
            var reserialized = loadedAs?.Item2 != serializer;
            sheet.ModificationTime = DateTime.Now;
            var path = serializer.Save(sheet.Name, sheet, loadedAs?.Item1);
            if (path != null)
            {
                markedAsChanged.Remove(sheet);
                if (reserialized)
                {
                    loadedAs?.Item2.Delete(loadedAs?.Item1);
                    if (loadedAs != null)
                        characters.Remove(sheet);
                    characters.Add(sheet, new Tuple<string, ISerializer<CharacterSheet>>(path, serializer));
                }
            }
        }

        public void DeleteCharacter(CharacterSheet sheet)
        {
            if (characters.TryGetValue(sheet, out Tuple<string, ISerializer<CharacterSheet>> loadedAs))
            {
                if (selectedCharacter == sheet)
                    selectedCharacter = null;
                loadedAs?.Item2?.Delete(loadedAs?.Item1);
                markedAsChanged.Remove(sheet);
                characters.Remove(sheet);
            }
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
    }
}
