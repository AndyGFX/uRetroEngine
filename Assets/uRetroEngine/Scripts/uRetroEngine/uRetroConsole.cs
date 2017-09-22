using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uRetroEngine
{
    /// <summary>
    /// Build-in console class
    /// </summary>
    public static class uRetroConsole
    {
        public static bool visible = false;
        public static GameObject console;

        /// <summary>
        /// Initialize console
        /// </summary>
        public static void Initialize()
        {
            // stupid but works after build when is print string to console first time
            Show();
            Hide();
        }

        /// <summary>
        /// Show console
        /// </summary>
        public static void Show()
        {
            if (console != null)
            {
                visible = true;
                console.SetActive(visible);
            }
        }

        /// <summary>
        /// Hide console
        /// </summary>
        public static void Hide()
        {
            if (console != null)
            {
                visible = false;
                console.SetActive(visible);
            }
        }

        /// <summary>
        /// Switch console visibility
        /// </summary>
        public static void SwitchVisibility()
        {
            if (console != null)
            {
                visible = !visible;
                console.SetActive(visible);
            }
        }

        /// <summary>
        /// Print text to console
        /// </summary>
        /// <param name="txt"></param>
        public static void Print(System.Object txt)
        {
            PrintText(txt);
        }

        /// <summary>
        /// Print text to unity editor console only
        /// </summary>
        /// <param name="obj"></param>
        public static void DebugLog(System.Object obj)
        {
            Debug.Log(obj);
        }

        /// <summary>
        /// Print error message to console
        /// </summary>
        /// <param name="txt"></param>
        public static void PrintError(System.Object txt)
        {
            PrintErrorText(txt);
        }

        /// <summary>
        /// Print float value to console
        /// </summary>
        /// <param name="f"></param>
        public static void Print(float f)
        {
            PrintText(f.ToString());
        }

        /// <summary>
        /// Print int value to console
        /// </summary>
        /// <param name="i"></param>
        public static void Print(int i)
        {
            PrintText(i.ToString());
        }

        /// <summary>
        /// Clear console
        /// </summary>
        public static void Clear()
        {
            DebugLogs.Instance.ClearLogs();
        }

        private static void PrintText(System.Object txt)
        {
            Debug.Log("<color=orange>u</color><color=white>Retro</color><color=orange>Engine: </color>" + txt);
        }

        private static void PrintErrorText(System.Object txt)
        {
            Debug.LogError("<color=orange>u</color><color=white>Retro</color><color=orange>Engine </color><color=red>ERROR: " + txt + "</color>");
        }
    }
}