using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public class uRetroEngineComponent : MonoBehaviour
    {
        public void OnStart()
        {
        }

        public void OnUpdate()
        {
            // read mouse position
            uRetroInput.UpdateMousePosition();

            // Console ON/OFF
            if (Input.GetKeyDown(KeyCode.F1)) uRetroConsole.SwitchVisibility();

            // Start capture screen to  GIF
            if (Input.GetKeyDown(KeyCode.F9)) uRetroCapture.Start();

            // Visual CPU/GPU monitor
            if (Input.GetKeyDown(KeyCode.F10)) uRetroSystem.SwitchFPSVisibility();

            // Code profiler ON/OFF
            if (Input.GetKeyDown(KeyCode.F11)) uRetroUtils.SwitchProfiler();
        }
    }
}