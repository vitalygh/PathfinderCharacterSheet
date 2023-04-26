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
using Android.Content.PM;

namespace PathfinderCharacterSheet.Droid
{
    public class PlatformProxy: IPlatformProxy
    {
        readonly Context activity = null;
        readonly PackageInfo packageInfo = null;
        public PlatformProxy(Context activity)
        {
            this.activity = activity;
            packageInfo = activity.PackageManager.GetPackageInfo(activity.PackageName, 0);
        }

        public string GetVersionNumber { get => packageInfo?.VersionName; }
        public string GetBuildNumber
        {
            get
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                    return packageInfo?.LongVersionCode.ToString();
                else
#pragma warning disable CS0618 // Type or member is obsolete
                    return packageInfo?.VersionCode.ToString();
#pragma warning restore CS0618 // Type or member is obsolete
            }
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