using System;
using System.Collections.Generic;

namespace PathfinderCharacterSheet
{
    public interface ISerializer<T>
    {
        Dictionary<T, string> Load();
        T Load(string name);
        string Save(string name, T data, string path = null, string loadedFrom = null);
        bool Delete(string path);

        Action<T, string> OnLoadingSuccess { get; set; }
        Action<string, Exception> OnLoadingFailed { get; set; }

        Action<T, string> OnSavingSuccess { get; set; }
        Action<string, string, Exception> OnSavingFailed { get; set; }
    }
}
