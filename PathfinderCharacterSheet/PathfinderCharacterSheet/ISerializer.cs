using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public interface ISerializer<T>
    {
        Dictionary<T, string> Load();
        string Save(string name, T data, string path = null);
        bool Delete(string path);

        Action<T, string> OnLoadingSuccess { get; set; }
        Action<string, Exception> OnLoadingFailed { get; set; }
        Action<T, string, string> OnLoadingFromBackup { get; set; }

        Action<T, string> OnSavingSuccess { get; set; }
        Action<string, string, Exception> OnSavingFailed { get; set; }
        Action<string, string, Exception> OnBackupSavingFailed { get; set; }
        Action<string, Exception> OnBackupRemovingFailed { get; set; }
    }
}
