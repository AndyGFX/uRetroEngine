using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CubesTeam;

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Diagnostics;

namespace uRetroEngine
{
    public class uRetroEngine_Runner_LUA : uRetroEngineComponent
    {
        public string runGame = "Template";

        [Button("Build uRetroEngine", "BuildEngine", true)]
        public bool h1;

        [Button("Move Cartridges", "MoveCartridges", true)]
        public bool h2;

        // Use this for initialization
        private void Awake()
        {
            uRetroSystem.Init();

            if (!uRetroSystem.CheckArguments())
            {
#if UNITY_EDITOR

                // in editor test code here
                // ------------------------
                // ...

                uRetroSystem.LoadData(this.runGame);

                // ...
                // -----------------------
#else
                uRetroConsole.PrintError("Missing cartridge or game definition argument. Use -cartridge <name> or -game <folder name> for load game.");
#endif
            }

            // load main LUA script from "Game" cartridge
            uRetroLua.Load();

            // RUN loaded script from "Game" cartridge
            uRetroLua.Run();
        }

        private void Start()
        {
            base.OnStart();

            if (!uRetroLua.wasExecuted) return;

            uRetroLua.OnStart();

#if UNITY_EDITOR

            // in editor test code here
            // ------------------------
            // ...

            // ...
            // ------------------------

#endif
        }

        private void Update()
        {
            base.OnUpdate();

            // exit when LUA has any error
            if (!uRetroLua.wasExecuted) return;

            // exit when console is opened
            if (uRetroConsole.visible) return;

#if UNITY_EDITOR

            // in editor test code here
            // ------------------------
            // ...

            // ...
            // -----------------------
#endif

            uRetroLua.OnUpdate();
        }

        private void OnDestroy()
        {
            uRetroLua.OnClose();
        }

        private void OnGUI()
        {
            //uRetroLua.OnGUI();
        }

#if UNITY_EDITOR

        //----------------------------------------------------------------
        // BUILD MENU
        //----------------------------------------------------------------
        [ContextMenu("Build LUA Engine")]
        public void BuildEngine()
        {
            // Get filename.
            string[] levels = new string[] { "Assets/uRe-Runner-LUA/Scenes/Test-uRetroEngine-LUA.unity" };

            // Build player.
            BuildPipeline.BuildPlayer(levels, uRetroSystem.GetRoot() + "/_Build_/uRetroEngine.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

            // Copy files
            FileUtil.ReplaceDirectory(uRetroSystem.GetRoot() + "/uRetroEngine_Cards", uRetroSystem.GetRoot() + "/_Build_/uRetroEngine_Cards");
            FileUtil.ReplaceDirectory(uRetroSystem.GetRoot() + "/uRetroEngine_Libs", uRetroSystem.GetRoot() + "/_Build_/uRetroEngine_Libs");
        }

        public void MoveCartridges()
        {
            // Copy files
            FileUtil.ReplaceDirectory(uRetroSystem.GetRoot() + "/uRetroEngine_Cards", uRetroSystem.GetRoot() + "/_Build_/uRetroEngine_Cards");
            FileUtil.ReplaceDirectory(uRetroSystem.GetRoot() + "/uRetroEngine_Libs", uRetroSystem.GetRoot() + "/_Build_/uRetroEngine_Libs");
        }

#endif
    }
}