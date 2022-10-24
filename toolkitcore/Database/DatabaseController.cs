using System;
using System.IO;
using UnityEngine;
using Verse;

namespace ToolkitCore.Database
{
    [StaticConstructorOnStartup]
    public static class DatabaseController
    {
        private static readonly string modFolder = "ToolkitCore";
        public static readonly string dataPath = Application.persistentDataPath + "/" + DatabaseController.modFolder + "/";

        static DatabaseController() => DatabaseController.Main();

        private static void Main()
        {
            if (Directory.Exists(DatabaseController.dataPath))
                return;
            Directory.CreateDirectory(DatabaseController.dataPath);
        }

        public static void SaveToolkit()
        {
        }

        public static void LoadToolkit()
        {
        }

        public static void SaveObject(object obj, string fileName, Mod mod)
        {
            if (mod.Content.Name == null)
            {
                Log.Error("Mod has no name");
            }
            else
            {
                fileName = mod.Content.Name.Replace(" ", "") + "_" + fileName + ".json";
                DatabaseController.SaveFile(JsonUtility.ToJson(obj), fileName);
            }
        }

        public static bool LoadObject<T>(string fileName, Mod mod, out T obj)
        {
            obj = default(T);
            if (mod.Content.Name == null)
            {
                Log.Error("Mod has no name");
                return false;
            }
            fileName = mod.Content.Name.Replace(" ", "") + "_" + fileName + ".json";
            string json;
            if (!DatabaseController.LoadFile(fileName, out json))
            {
                Log.Warning("Tried to load " + fileName + " but could not find file");
                return false;
            }
            obj = JsonUtility.FromJson<T>(json);
            return true;
        }

        public static bool SaveFile(string json, string fileName)
        {
            Log.Message(json);
            try
            {
                using (StreamWriter text = File.CreateText(Path.Combine(DatabaseController.dataPath, fileName)))
                    text.Write(json.ToString());
            }
            catch (IOException ex)
            {
                Log.Error(ex.Message);
                return false;
            }
            return true;
        }

        public static bool LoadFile(string fileName, out string json)
        {
            json = (string)null;
            string path = Path.Combine(DatabaseController.dataPath, fileName);
            if (!File.Exists(path))
                return false;
            try
            {
                using (StreamReader streamReader = File.OpenText(path))
                    json = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                return false;
            }
            return true;
        }
    }
}
