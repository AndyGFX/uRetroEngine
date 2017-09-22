using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if NLUA

using NLua;

#else

using MoonSharp.Interpreter;

#endif

namespace uRetroEngine
{
    /// <summary>
    /// uRetroEngine LUA interpreter (NLua or MOONSHARP) set compiler directive for assign
    /// </summary>
    public static class uRetroLua
    {
        public static string code = "";
#if NLUA
        public static Lua script;
        public static LuaFunction _OnStart;
        public static LuaFunction _OnUpdate;
        public static LuaFunction _OnClose;
        public static LuaFunction _OnScanline;
        public static LuaFunction _OnGui;

#else
        public static Script script;
#endif
        public static bool wasExecuted = false;
        public static bool isLoaded = false;
        public static bool fromCart = false;

        /// <summary>
        /// Create LUA engine and load main.lua script from crtridge game folder
        /// </summary>
        public static void Load()
        {
            string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/";

#if NLUA
            script = new Lua();

            script.LoadCLRPackage();

            // CLASSES

            script["Library"] = new uRetroLuaClasses_Library();
            script["Display"] = new uRetroLuaClasses_Display();
            script["Sprites"] = new uRetroLuaClasses_Sprite();
            script["MetaSprite"] = new uRetroLuaClasses_MetaSprites();
            script["Graphics"] = new uRetroLuaClasses_Graphics();
            script["Text"] = new uRetroLuaClasses_Text();
            script["Colors"] = new uRetroLuaClasses_Color();
            script["System"] = new uRetroLuaClasses_System();
            script["Utils"] = new uRetroLuaClasses_Utils();
            script["Tilemap"] = new uRetroLuaClasses_Tilemap();
            script["Input"] = new uRetroLuaClasses_Input();
            script["Console"] = new uRetroLuaClasses_Console();
            script["Sound"] = new uRetroLuaClasses_Sound();
            script["Data"] = new uRetroLuaClasses_GameData();
            script["Capture"] = new uRetroLuaClasses_Capture();

            script.RegisterFunction("print", typeof(uRetroConsole).GetMethod("DebugLog"));

            // INPUT BUTTON CONSTANTS

            script["KEY_UP"] = uRetroConfig.UP;
            script["KEY_DOWN"] = uRetroConfig.DOWN;
            script["KEY_LEFT"] = uRetroConfig.LEFT;
            script["KEY_RIGHT"] = uRetroConfig.RIGHT;
            script["KEY_A"] = uRetroConfig.A;
            script["KEY_B"] = uRetroConfig.B;
            script["KEY_X"] = uRetroConfig.X;
            script["KEY_Y"] = uRetroConfig.Y;
            script["KEY_START"] = uRetroConfig.START;
            script["KEY_OPTION"] = uRetroConfig.OPTION;

            // predefined tile flags
            script["FLAG_FREE"] = 0;
            script["FLAG_WALL"] = 1;
            script["FLAG_OBSTACLE"] = 2;
            script["FLAG_LADDER"] = 3;
            script["FLAG_HAZARD"] = 4;
            script["FLAG_CHEST"] = 5;
            script["FLAG_ITEM"] = 6;
            script["FLAG_COIN"] = 7;
            script["FLAG_LIFE"] = 8;
            script["FLAGS"] = new int[255];
#else
            UserData.RegisterAssembly();
            script = new Script();

            // CLASSES

            script.Globals["Library"] = new uRetroLuaClasses_Library();
            script.Globals["Display"] = new uRetroLuaClasses_Display();
            script.Globals["Sprites"] = new uRetroLuaClasses_Sprite();
            script.Globals["MetaSprite"] = new uRetroLuaClasses_MetaSprites();
            script.Globals["Graphics"] = new uRetroLuaClasses_Graphics();
            script.Globals["Text"] = new uRetroLuaClasses_Text();
            script.Globals["Colors"] = new uRetroLuaClasses_Color();
            script.Globals["System"] = new uRetroLuaClasses_System();
            script.Globals["Utils"] = new uRetroLuaClasses_Utils();
            script.Globals["Tilemap"] = new uRetroLuaClasses_Tilemap();
            script.Globals["Input"] = new uRetroLuaClasses_Input();
            script.Globals["Console"] = new uRetroLuaClasses_Console();
            script.Globals["Sound"] = new uRetroLuaClasses_Sound();
            script.Globals["Data"] = new uRetroLuaClasses_GameData();

            //script.RegisterFunction("print", typeof(uRetroConsole).GetMethod("DebugLog"));
            script.Globals["print"] = typeof(uRetroConsole).GetMethod("DebugLog");

            // INPUT BUTTON CONSTANTS

            script.Globals["KEY_UP"] = uRetroConfig.UP;
            script.Globals["KEY_DOWN"] = uRetroConfig.DOWN;
            script.Globals["KEY_LEFT"] = uRetroConfig.LEFT;
            script.Globals["KEY_RIGHT"] = uRetroConfig.RIGHT;
            script.Globals["KEY_A"] = uRetroConfig.A;
            script.Globals["KEY_B"] = uRetroConfig.B;
            script.Globals["KEY_X"] = uRetroConfig.X;
            script.Globals["KEY_Y"] = uRetroConfig.Y;
            script.Globals["KEY_START"] = uRetroConfig.START;
            script.Globals["KEY_OPTION"] = uRetroConfig.OPTION;

            // predefined tile flags
            script.Globals["FLAG_FREE"] = 0;
            script.Globals["FLAG_WALL"] = 1;
            script.Globals["FLAG_OBSTACLE"] = 2;
            script.Globals["FLAG_LADDER"] = 3;
            script.Globals["FLAG_HAZARD"] = 4;
            script.Globals["FLAG_CHEST"] = 5;
            script.Globals["FLAG_ITEM"] = 6;
            script.Globals["FLAG_COIN"] = 7;
            script.Globals["FLAG_LIFE"] = 8;
            script.Globals["FLAGS"] = new int[255];

#endif
            /*
            foreach (KeyValuePair<object, object> kvp in script.GetTableDict(script.GetTable("_G")))
            {
                uRetroConsole.Print(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            }
            */

            if (fromCart)
            {
                isLoaded = true;
            }
            else
            {
                if (File.Exists(path + uRetroConfig.fileLua))
                {
                    code = File.ReadAllText(path + uRetroConfig.fileLua);
                    isLoaded = true;
                }
                else
                {
                    uRetroConsole.Show();
                    uRetroConsole.PrintError("Lua ERROR: Missing file (" + path + uRetroConfig.fileLua + ")");
                }
            }
        }

        /// <summary>
        /// Run loaded main.lua script
        /// </summary>
        public static void Run()
        {
            if (isLoaded)
            {
                try
                {
                    script.DoString(code);
#if NLUA
                    _OnStart = script.GetFunction("OnStart");
                    _OnUpdate = script.GetFunction("OnUpdate");
                    _OnClose = script.GetFunction("OnClose");
                    _OnGui = script.GetFunction("OnGui");
                    _OnScanline = script.GetFunction("OnScanline");
#endif
                    wasExecuted = true;
                }
                catch (NLua.Exceptions.LuaException ex)
                {
                    uRetroConsole.Show();
                    uRetroConsole.PrintError("ERROR: " + ex);
                }
            }
        }

#if NLUA

        /// <summary>
        /// Callback to LUA script "OnStart" funtion
        /// </summary>
        public static void OnStart()
        {
            if (wasExecuted) _OnStart.Call();
        }

        /// <summary>
        /// Callback to LUA script "OnUpdate" funtion
        /// </summary>
        public static void OnUpdate()
        {
            if (wasExecuted) _OnUpdate.Call(Time.deltaTime);
        }

        /// <summary>
        /// Callback to LUA script "OnScanline" funtion
        /// </summary>
        /// <param name="line"></param>
        public static void OnScaline(int line)
        {
            if (wasExecuted) _OnScanline.Call(line);
        }

        /// <summary>
        /// Callback to LUA script "OnClose" funtion
        /// </summary>
        public static void OnClose()
        {
            if (wasExecuted) _OnClose.Call();
        }

        /// <summary>
        /// Callback to LUA script "OnGui" funtion
        /// </summary>
        public static void OnGUI()
        {
            if (wasExecuted) _OnGui.Call();
        }

        private static System.Object[] Call(string function, params System.Object[] args)
        {
            System.Object[] result = new System.Object[0];
            if (script == null) return result;
            LuaFunction lf = script.GetFunction(function);
            if (lf == null) return result;
            try
            {
                // Note: calling a function that does not
                // exist does not throw an exception.
                if (args != null)
                {
                    result = lf.Call(args);
                }
                else {
                    result = lf.Call();
                }
            }
            catch (NLua.Exceptions.LuaException e)
            {
                Debug.LogError(FormatException(e));
            }
            return result;
        }

        private static System.Object[] Call(string function)
        {
            return Call(function, null);
        }

        private static string FormatException(NLua.Exceptions.LuaException e)
        {
            string source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
            return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
        }

#else

        public static void OnStart()
        {
            if (wasExecuted)
                script.Call(script.Globals.Get("OnStart"));
        }

        public static void OnUpdate()
        {
            if (wasExecuted)
                script.Call(script.Globals.Get("OnUpdate"), Time.deltaTime);
        }

        public static void OnScaline(int line)
        {
            if (wasExecuted)
                script.Call(script.Globals.Get("OnScanline"), line);
        }

        public static void OnClose()
        {
            if (wasExecuted)
                script.Call(script.Globals.Get("OnClose"));
        }

        public static void OnGUI()
        {
            script.Call(script.Globals.Get("OnGUI"));
        }

#endif
    }
}