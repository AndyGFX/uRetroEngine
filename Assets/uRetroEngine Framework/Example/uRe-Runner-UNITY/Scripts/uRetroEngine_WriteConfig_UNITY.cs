using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uRetroEngine
{
    public class uRetroEngine_WriteConfig_UNITY : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            uRetroSystem.Init();
            uRetroConsole.Show();
            uRetroSystem.ShowFPS(false);
            uRetroSystem.CreateGame("RetroGameTest");
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) uRetroConsole.SwitchVisibility();
            if (Input.GetKeyDown(KeyCode.F10)) uRetroSystem.SwitchFPSVisibility();
        }
    }
}