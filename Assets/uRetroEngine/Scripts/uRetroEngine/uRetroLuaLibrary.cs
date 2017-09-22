using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace uRetroEngine
{
    /// <summary>
    /// Included Lua code item
    /// </summary>
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

    /// <summary>
    /// Lua library class
    /// </summary>
    public static class uRetroLuaLibrary
    {
        /// <summary>
        /// List of all included lua codes
        /// </summary>
        public static List<SrcInclude> include = new List<SrcInclude>();

        /// <summary>
        /// Load lua scrip from LIBS folder
        /// </summary>
        /// <param name="name"></param>
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

        /// <summary>
        /// Include lua code to main.lua and store code in list
        /// </summary>
        /// <param name="name"></param>
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