using DG.Tweening.Plugins.Core.PathCore;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Mine.CodeBase.App.Model
{
    [Serializable]
    public class JellyFarmJsonDBModel : JsonDBModel<JellyFarmJsonDBModel>
    {
        #region Properties
        
        public JArray JellyPresets => DB["JellyPreset"]["presets"] as JArray;
        public JToken Gelatin => DB["Currency"]["gelatin"];
        public JToken Gold => DB["Currency"]["gold"];
        public JArray Jellies => DB["Field"]["jellies"] as JArray;

        public JToken ApartmentLevel => DB["Plant"]["apartmentLevel"];
        public JToken ClickLevel => DB["Plant"]["clickLevel"];
        public JArray Apartment => DB["Upgrade"]["apartment"] as JArray;
        public JArray Click => DB["Upgrade"]["click"] as JArray;

        #endregion


        #region EditorOnly

#if UNITY_EDITOR

        [MenuItem("DB/Reset")]
        public static void ResetDBEditor()
        {

            string path = $"{Application.persistentDataPath}/Json";

            Debug.Log($"{path}");

            foreach (string directory in Directory.GetDirectories(path))
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (IOException)
                {
                    Directory.Delete(directory, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(directory, true);
                }
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

#endif

        #endregion
    }
}
