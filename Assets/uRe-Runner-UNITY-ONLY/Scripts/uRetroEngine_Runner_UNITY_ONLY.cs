using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uRetroEngine
{
    public class uRetroEngine_Runner_UNITY_ONLY : uRetroEngineComponent
    {
        private int xx = 0;
        private int yy = 0;
        private int line = 0;

        private void Awake()
        {
            // set targetDisplay for Mouse Input
            uRetroDisplay.displayTarget = GameObject.FindGameObjectWithTag("uRetroDisplay").GetComponent<RectTransform>();

            // set code profiler canvas
            //uRetroUtils.codeProfiler = GameObject.FindGameObjectWithTag("uRetroProfiler");

            // initialize screen capture to GIF
            uRetroCapture.Init();

            // prepare and set console window
            //uRetroConsole.console = GameObject.FindGameObjectWithTag("uRetroConsole");
        }

        // Use this for initialization
        private void Start()
        {
            base.OnStart();
        }

        // Update is called once per frame
        private void Update()
        {
            base.OnUpdate();

            // ---------------------- TEST CODE .....

            uRetroDisplay.Clear();

            uRetroDisplay.Flip();
        }
    }
}