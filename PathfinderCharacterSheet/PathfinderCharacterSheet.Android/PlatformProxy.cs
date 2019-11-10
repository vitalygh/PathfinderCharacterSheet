using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PathfinderCharacterSheet.Droid
{
    public class PlatformProxy: IPlatformProxy
    {
        Context activity = null;
        public PlatformProxy(Context activity)
        {
            this.activity = activity;
        }
        public string PersistentDataPath
        {
            get
            {
                if (activity != null)
                {
                    var path = activity.GetExternalFilesDir("");
                    if (path != null)
                        return path.AbsolutePath;
                }
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            }
        }
    }
}