using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace PrefsWrapperJson
{
    public class JSONData
    {
        public static JSONData Instance = new JSONData();

        Dictionary<string, object> _dic = new Dictionary<string, object>();

        string _jsonFileName = "default";

        public JSONData() { Load(""); }

        public bool HasKey(string key) { return _dic.ContainsKey(key); }
        public void DeleteKey(string key) { _dic.Remove(key); }

        public object Get(string key) { return _dic[key]; }

        public void Set(string key, object value) { _dic[key] = value; }

        //public void SetJsionFileName(string filename) { _jsonFileName = filename; }

        //string path { get { return Application.persistentDataPath + "/Prefs.json"; } }
        // StreamingAssetsに保存されるように変更
        string path { get { return Application.streamingAssetsPath + "/" + _jsonFileName + ".json"; } }
        string GetPath(string filename)
        {
            return Application.streamingAssetsPath + "/" + filename + ".json";
        }

        public void Save(string filename)
        {
            var str = MiniJSON.Json.Serialize(_dic);
            File.WriteAllText(GetPath(filename), str);
        }

        public void Load(string filename)
        {
            if (File.Exists(GetPath(filename)))
            {
                var str = File.ReadAllText(GetPath(filename));
                _dic = (Dictionary<string, object>)MiniJSON.Json.Deserialize(str);
            }
        }

        public void DeleteAll()
        {
            _dic.Clear();
        }
    }

    class PlayerPrefsGlobal
    {
        // add
        //public static void SetFileName(string filename) { JSONData.Instance.SetJsionFileName(filename); }
        public static void Save(string filename) { JSONData.Instance.Save(filename); }
        public static void Load(string filename) { JSONData.Instance.Load(filename); }
        public static void DeleteAll() { JSONData.Instance.DeleteAll(); }
    }


    class PlayerPrefsStrandard<T>
    {
        static Type type
        {
            get
            {
                return (typeof(T) == typeof(bool) || typeof(T).IsEnum)
                    ? typeof(int)
                    : typeof(T);
            }
        }

        public static bool HasKey(string key)
        {
            return JSONData.Instance.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            JSONData.Instance.DeleteKey(key);
        }

        public static object Get(string key, object defaultValue)
        {
            if (!HasKey(key)) Set(key, defaultValue);

            var ret = JSONData.Instance.Get(key);
            return (typeof(T).IsEnum ? (T)Enum.Parse(typeof(T), ret.ToString()) : Convert.ChangeType(ret, typeof(T)));
        }

        public static void Set(string key, object val)
        {
            JSONData.Instance.Set(key, Convert.ChangeType(val, type));
        }
    }

}
