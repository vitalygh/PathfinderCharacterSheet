using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace PathfinderCharacterSheet.iOS
{
    public class PlatformProxy: IPlatformProxy
    {
        public string PersistentDataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); } }
    }
}