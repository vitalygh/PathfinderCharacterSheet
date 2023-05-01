using Foundation;
using System;

namespace PathfinderCharacterSheet.iOS
{
    public class PlatformProxy: IPlatformProxy
    {
        public string PersistentDataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); } }

        public string GetVersionNumber { get => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString(); }
        public string GetBuildNumber { get => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString(); }
    }
}