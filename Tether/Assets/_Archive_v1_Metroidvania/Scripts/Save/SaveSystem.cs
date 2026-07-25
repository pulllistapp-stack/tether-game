using System.IO;
using UnityEngine;

namespace Tether.Save
{
    public static class SaveSystem
    {
        private const string FileName = "save.json";
        private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        private static SaveData _cache;

        public static SaveData Current
        {
            get
            {
                if (_cache == null) _cache = Load();
                return _cache;
            }
        }

        public static SaveData Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    _cache = new SaveData();
                    return _cache;
                }
                string json = File.ReadAllText(FilePath);
                _cache = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
                return _cache;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Load failed: {e.Message}");
                _cache = new SaveData();
                return _cache;
            }
        }

        public static void Save()
        {
            try
            {
                if (_cache == null) _cache = new SaveData();
                string json = JsonUtility.ToJson(_cache, true);
                File.WriteAllText(FilePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
            }
        }

        public static void Reset()
        {
            _cache = new SaveData();
            Save();
        }

        public static string PathInfo() => FilePath;
    }
}
