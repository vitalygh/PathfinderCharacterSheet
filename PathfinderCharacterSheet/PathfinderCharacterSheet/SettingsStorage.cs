namespace PathfinderCharacterSheet
{
    sealed class SettingsStorage
    {
        private static SettingsStorage instance = null;

        private readonly Settings settings = new Settings();

        public Settings Settings
        {
            get => settings;
            set
            {
                if (settings == value)
                    return;
                settings.Fill(value);
                if (!settings.SaveChangesImmediately)
                    return;
                settings.Save();
                CharacterSheetStorage.Instance.SaveChangedCharacters();
            }
        }

        private SettingsStorage()
        {

        }

        static internal SettingsStorage Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsStorage();
                return instance;
            }
        }

        public void Load()
        {
            Settings.Fill(Settings.Load());
        }

        public void SaveChangedSettings()
        {
            if (Settings.IsChanged)
                Settings.Save();
        }
    }
}
