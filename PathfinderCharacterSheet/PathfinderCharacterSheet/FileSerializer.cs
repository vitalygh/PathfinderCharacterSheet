#define SAVE_BACKUPS
#define LOAD_FROM_BACKUPS
//#define VALIDATE_XML
using System;
using System.Collections.Generic;
using System.IO;

namespace PathfinderCharacterSheet
{
    public abstract class FileSerializer<T>: ISerializer<T> where T: class
    {
        private readonly string rootPath = null;

        public static int MaxBackupsCount { get { return 5; } }
        public static int MaxGeneratePathIndex { get { return 1000; } }

        private string RootPath
        {
            get
            {
                string persistentDataPath = null;
                if (App.PlatformProxy != null)
                    persistentDataPath = App.PlatformProxy.PersistentDataPath;
                if (persistentDataPath == null)
                    persistentDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(persistentDataPath, rootPath);
            }
        }

        public FileSerializer(string path)
        {
            rootPath = path;
        }

        protected abstract string Extension { get; }
        protected abstract T Deserialize(StreamReader stream);
        protected abstract void Serialize(T data, MemoryStream stream);

        public bool SaveBackups { get; set; } = false;
        public bool LoadFromBackups { get; set; } = false;
        public bool CheckPathNotExist { get; set; } = false;

        public Action<T, string> OnLoadingSuccess { get; set; }
        public Action<string, Exception> OnLoadingFailed { get; set; }
        public Action<T, string, string> OnLoadingFromBackup { get; set; }
        public Action<T, string> OnSavingSuccess { get; set; }
        public Action<string, string, Exception> OnSavingFailed { get; set; }
        public Action<string, string, Exception> OnBackupSavingFailed { get; set; }
        public Action<string, Exception> OnBackupRemovingFailed { get; set; }

        public Dictionary<T, string> Load()
        {
            var loaded = new Dictionary<T, string>();
            var path = RootPath;
            if (!Directory.Exists(path))
                return null;
            var files = Directory.GetFiles(path, "*." + Extension, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (file.EndsWith("_backup." + Extension))
                    continue;
                var data = LoadFromPath(file);
                if (data != null)
                {
                    //CharacterSheets.V1.ValueWithIntModifiers.Optimize();
#if VALIDATE_XML
                    using (var memoryStream = new MemoryStream())
                    {
                        Serialize(data, memoryStream);
                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            var fileText = File.ReadAllText(file);
                            var streamText = streamReader.ReadToEnd();
                            if (fileText != streamText)
                                File.WriteAllText(file + ".validate", streamText);
                        }
                    }
#endif
                    loaded.Add(data, file);
                    continue;
                }
#if LOAD_FROM_BACKUPS
                if (!LoadFromBackups)
                    continue;
                var backups = GetBackupsList(file);
                if (backups == null)
                    continue;
                var sorted = new List<string>(backups);
                sorted.Sort();
                var index = sorted.Count;
                while (--index >= 0)
                {
                    var backup = sorted[index];
                    var loadedFromBackup = LoadFromPath(backup);
                    if (loadedFromBackup != null)
                    {
                        loaded.Add(loadedFromBackup, file);
                        OnLoadingFromBackup?.Invoke(loadedFromBackup, file, backup);
                        break;
                    }
                }
#endif
            }
            return loaded;
        }


        private T LoadFromPath(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    OnLoadingFailed?.Invoke(path, new FileNotFoundException("File \"" + path + "\" doesn't exist!"));
                    return null;
                }
                using (var stream = new StreamReader(path))
                {
                    var data = Deserialize(stream);
                    OnLoadingSuccess?.Invoke(data, path);
                    return data;
                }
            }
            catch (Exception ex)
            {
                OnLoadingFailed?.Invoke(path, ex);
            }
            return null;
        }

        public T Load(string name)
        {
            var index = 0;
            var path = GenerateFreePath(name, ref index);
            return LoadFromPath(path);
        }

        private string GenerateFreePath(string name, ref int index)
        {
            if (name is null)
                name = string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c, '_');
            while (index < MaxGeneratePathIndex)
            {
                var path = name;
                //path += "_" + Guid.NewGuid().ToString();
                if (index > 0)
                    path += "_" + index;
#if SAVE_BACKUPS
                if (SaveBackups)
                    path += "_pcs";
#endif
                path = Path.Combine(RootPath, path + "." + Extension);
                if (!CheckPathNotExist || !File.Exists(path))
                    return path;
                index += 1;
            }
            return null;
        }

#if SAVE_BACKUPS || LOAD_FROM_BACKUPS
        private static string[] GetBackupsList(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var dir = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var ext = Path.GetExtension(filename);
            if (!string.IsNullOrWhiteSpace(ext))
                filename = filename.Substring(0, filename.Length - ext.Length);
            var pattern = filename + "*_backup" + ext;
            return Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
        }
#endif

#if SAVE_BACKUPS
        private static string GetBackupFilename(string path, int index)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var ext = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(ext))
                path = path.Substring(0, path.Length - ext.Length);
            var dt = DateTime.Now.ToString("yyyyMMddHHmmss");
            return path + "_" + dt + index.ToString("000") + "_backup" + ext;
        }

        private static string GetBackupFilename(string path)
        {
            var index = 0;
            var filename = GetBackupFilename(path, index);
            while (index < MaxGeneratePathIndex)
            {
                if (!File.Exists(filename))
                    return filename;
                index += 1;
                filename = GetBackupFilename(path, index);
            }
            return filename;
        }

        private void SaveBackup(string path)
        {
            if (!SaveBackups)
                return;
            if (string.IsNullOrWhiteSpace(path))
                return;
            if (!File.Exists(path))
                return;
            var backup = GetBackupFilename(path);
            try
            {
                File.Copy(path, backup, true);
            }
            catch (Exception ex)
            {
                OnBackupSavingFailed?.Invoke(path, backup, ex);
            }
        }

        private void RemoveOldBackups(string path)
        {
            var files = GetBackupsList(path);
            if (files == null)
                return;
            if (files.Length <= MaxBackupsCount)
                return;
            var sorted = new List<string>(files);
            sorted.Sort();
            while (sorted.Count > MaxBackupsCount)
            {
                var file = sorted[0];
                sorted.RemoveAt(0);
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    OnBackupRemovingFailed?.Invoke(file, ex);
                }
            }
        }
#endif

        public string Save(string name, T data, string path = null)
        {
            if (data == null)
                return null;
            var dir = RootPath;
            //var characterName = string.IsNullOrWhiteSpace(name) ? "unnamed character" : "character \"" + name + "\"";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            try
            {
                //sheet.ModificationTime = DateTime.Now;
                using (var memoryStream = new MemoryStream())
                {
                    Serialize(data, memoryStream);
#if SAVE_BACKUPS
                    SaveBackup(path);
#endif
                    int index = 0;
                    if (path == null)
                        path = GenerateFreePath(name, ref index);
                    while (true)
                    {
                        try
                        {
                            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                            {
                                memoryStream.WriteTo(fileStream);
                                fileStream.Flush();
                                break;
                            }
                        }
                        catch
                        {
                            index += 1;
                            path = CheckPathNotExist ? GenerateFreePath(name, ref index) : null;
                            if (path == null)
                                throw;
                        }
                    }
#if SAVE_BACKUPS
                    RemoveOldBackups(path);
#endif
                }
                OnSavingSuccess?.Invoke(data, path);
                return path;
            }
            catch (Exception ex)
            {
                OnSavingFailed?.Invoke(name, path, ex);
            }
            return null;
        }

        public bool Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if (!File.Exists(path))
                return false;
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
