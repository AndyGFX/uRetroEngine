using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace uRetroEngine
{
    public struct SrcInclude
    {
        public string name;
        public string code;

        public SrcInclude(string n, string c)
        {
            name = n;
            code = c;
        }
    }

    public static class uRetroLuaLibrary
    {
        public static List<SrcInclude> include = new List<SrcInclude>();

        public static void Require(string name)
        {
            string path = uRetroSystem.GetRoot() + "/uRetroEngine_Libs/" + name;

            if (File.Exists(path))
            {
                string code = File.ReadAllText(path);
                uRetroLua.script.DoString(code);
            }
            else
            {
                uRetroConsole.Show();
                uRetroConsole.Print("Library:Require LIB ERROR: Missing file (" + path + ")");
            }
        }

        public static void Include(string name)
        {
            DoIncludeSrc(name);
        }

        private static void DoIncludeSrc(string name)
        {
            string code = "";
            string path = uRetroSystem.GetRoot() + "/uRetroEngine_Cards/" + uRetroConfig.cartridgeName + "/" + name;

            if (File.Exists(path))
            {
                // from file
                code = File.ReadAllText(path);

                bool state = include.Exists(src => src.name == name);

                if (state)
                {
                    int idx = include.FindIndex(src => src.name == name);
                    include.RemoveAt(idx);
                }
                include.Add(new SrcInclude(name, code));
            }
            else
            {
                code = include.Find(i => i.name == name).code;
            }

            if (code == "")
            {
                uRetroConsole.Show();
                uRetroConsole.Print("Library:Include INCLUDE ERROR: Missing file (" + path + ")");
                return;
            }

            uRetroLua.script.DoString(code);
        }
    }
}