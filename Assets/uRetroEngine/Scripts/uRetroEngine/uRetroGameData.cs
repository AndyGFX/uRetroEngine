using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace uRetroEngine
{
    public enum DataType { TYPE_INT, TYPE_FLOAT, TYPE_STRING }

    public struct GameData
    {
        public string name;
        public string value;
        public DataType type;
    }

    public static class uRetroGameData
    {
        public static List<GameData> data = new List<GameData>();

        private static bool Exist(string name)
        {
            return data.Exists(d => (d.name == name));
        }

        public static void Set(string name, object value)
        {
            GameData newData;

            if (value.GetType() == typeof(float))
            {
                if (!Exist(name))
                {
                    newData = new GameData();
                    newData.name = name;
                    data.Add(newData);
                }

                newData = data.Find(d => (d.name == name));
                newData.value = ((float)value).ToString();
                newData.type = DataType.TYPE_FLOAT;
                data[data.FindIndex(d => (d.name == name))] = newData;
            }

            if (value.GetType() == typeof(int))
            {
                if (!Exist(name))
                {
                    newData = new GameData();
                    newData.name = name;
                    data.Add(newData);
                }

                newData = data.Find(d => (d.name == name));
                newData.value = ((int)value).ToString();
                newData.type = DataType.TYPE_INT;
                data[data.FindIndex(d => (d.name == name))] = newData;
            }

            if (value.GetType() == typeof(string))
            {
                if (!Exist(name))
                {
                    newData = new GameData();
                    newData.name = name;
                    data.Add(newData);
                }

                newData = data.Find(d => (d.name == name));
                newData.value = (string)value;
                newData.type = DataType.TYPE_STRING;
                data[data.FindIndex(d => (d.name == name))] = newData;
            }
        }

        public static void SetFloat(string name, float value)
        {
            GameData newData;

            if (!Exist(name))
            {
                newData = new GameData();
                newData.name = name;
                data.Add(newData);
            }

            newData = data.Find(d => (d.name == name));
            newData.value = value.ToString();
            newData.type = DataType.TYPE_FLOAT;
            data[data.FindIndex(d => (d.name == name))] = newData;
        }

        public static void SetInt(string name, int value)
        {
            GameData newData;

            if (!Exist(name))
            {
                newData = new GameData();
                newData.name = name;
                data.Add(newData);
            }

            newData = data.Find(d => (d.name == name));
            newData.value = value.ToString();
            newData.type = DataType.TYPE_INT;
            data[data.FindIndex(d => (d.name == name))] = newData;
        }

        public static void SetString(string name, string value)
        {
            GameData newData;

            if (!Exist(name))
            {
                newData = new GameData();
                newData.name = name;
                data.Add(newData);
            }

            newData = data.Find(d => (d.name == name));
            newData.value = value;
            newData.type = DataType.TYPE_STRING;
            data[data.FindIndex(d => (d.name == name))] = newData;
        }

        public static void Delete(string name)
        {
            data.Remove(data.Find(d => (d.name == name)));
        }

        public static int GetAsInt(string name)
        {
            int res = 0;

            if (!data.Exists(d => (d.name == name)))
            {
                uRetroConsole.PrintError("Variable '" + name + "' doesn't esist!");
                return 0;
            }

            GameData resData = data.Find(d => (d.name == name));
            if (resData.type == DataType.TYPE_INT)
            {
                res = int.Parse(resData.value);
            }
            else
            {
                Debug.Log("uRetro GameData ERROR: value is not integer");
            }
            return res;
        }

        public static float GetAsFloat(string name)
        {
            float res = 0f;

            if (!data.Exists(d => (d.name == name)))
            {
                uRetroConsole.PrintError("Variable '" + name + "' doesn't esist!");
                return 0;
            }

            GameData resData = data.Find(d => (d.name == name));

            if (resData.type == DataType.TYPE_FLOAT)
            {
                res = float.Parse(resData.value);
            }
            else
            {
                Debug.Log("uRetro GameData ERROR: value is not float");
            }
            return res;
        }

        public static string GetAsString(string name)
        {
            string res = "";

            if (!data.Exists(d => (d.name == name)))
            {
                uRetroConsole.PrintError("Variable '" + name + "' doesn't esist!");
                return "";
            }

            GameData resData = data.Find(d => (d.name == name));
            if (resData.type == DataType.TYPE_STRING)
            {
                res = resData.value;
            }
            else
            {
                Debug.Log("uRetro GameData ERROR: value is not string");
            }
            return res;
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string path = Application.persistentDataPath + "/" + uRetroConfig.cartridgeName + ".gamedata";
            File.WriteAllText(path, json);
        }

        public static void Load()
        {
            string path = Application.persistentDataPath + "/" + uRetroConfig.cartridgeName + ".gamedata";

            if (!File.Exists(path))
            {
                Debug.Log("uRetro GameData ERROR: GameData file doesn't exist");
                return;
            }
            string json = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<List<GameData>>(json);
        }
    }
}