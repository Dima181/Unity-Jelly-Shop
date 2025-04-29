using Mine.CodeBase.Framework.Extension;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mine.CodeBase.App.Model
{
    [Serializable]
    public class JsonDBModel<T>
        where T : JsonDBModel<T>
    {
        #region Properties

        public Dictionary<string, JObject> DB { get; private set; }

        #endregion


        #region Fields

        [SerializeField] protected string Path;
        [SerializeField] private TextAsset[] _jsons;

        #endregion


        #region Methods

        public void SaveDB()
        {
            DB.ForEach(kvp =>
            {
                File.WriteAllText($"{Application.persistentDataPath}/{Path}/{kvp.Key}.json", kvp.Value.ToString());
            });
        }

        public void LoadDB(params string[] keys)
        {
            if (!Directory.Exists($"{Application.persistentDataPath}/{Path}"))
                InitDB();

            DB = new();
            keys.ForEach(key =>
            {
                string json = File.ReadAllText($"{Application.persistentDataPath}/{Path}/{key}.json");
                if(!string.IsNullOrEmpty(json))
                    DB[key] = JObject.Parse(json);
            });
        }

        public void InitDB()
        {
            Debug.Log("InitDB");
            Directory.CreateDirectory($"{Application.persistentDataPath}/{Path}");
            _jsons.ForEach(json =>
            {
                string key = json.name;
                string path = $"{Application.persistentDataPath}/{Path}/{key}.json";

                File.WriteAllText(path, json.text);
            });
        }

        #endregion
    }
}
