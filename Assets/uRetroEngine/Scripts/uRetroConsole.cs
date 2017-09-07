using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uRetroEngine
{
    public static class uRetroConsole
    {
        public static bool visible = false;
        public static GameObject console;

        public static void Initialize()
        {
            // stupid but works after build when is print string to console first time
            Show();
            Hide();
        }

        public static void Show()
        {
            visible = true;
            console.SetActive(visible);
        }

        public static void Hide()
        {
            visible = false;
            console.SetActive(visible);
        }

        public static void SwitchVisibility()
        {
            visible = !visible;
            console.SetActive(visible);
        }

        public static void Print(System.Object txt)
        {
            PrintText(txt);
        }

        public static void DebugLog(System.Object obj)
        {
            Debug.Log(obj);
        }

        public static void PrintError(System.Object txt)
        {
            PrintErrorText(txt);
        }

        public static void Print(float f)
        {
            PrintText(f.ToString());
        }

        public static void Print(int i)
        {
            PrintText(i.ToString());
        }

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
            Debug.Log("<color=orange>u</color><color=white>Retro</color><color=orange>Engine </color><color=red>ERROR: " + txt + "</color>");
        }
    }
}