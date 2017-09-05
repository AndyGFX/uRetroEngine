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

        public static void Print(string txt)
        {
            PrintText(txt);
        }

        public static void DebugLog(string txt)
        {
            Debug.Log(txt);
        }

        public static void PrintError(string txt)
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

        private static void PrintText(string txt)
        {
            Debug.Log("<color=orange>u</color><color=white>Retro</color><color=orange>Engine: </color>" + txt);
        }

        private static void PrintErrorText(string txt)
        {
            Debug.Log("<color=orange>u</color><color=white>Retro</color><color=orange>Engine: </color><color=red>" + txt + "</color>");
        }
    }
}