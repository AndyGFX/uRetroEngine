using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public static class uRetroCapture
    {
        private static CaptureToGIF screenCapture = null;

        public static void Init()
        {
            screenCapture = Camera.main.GetComponent<CaptureToGIF>();
        }

        public static void Setup(string filename, int framerate, int downscale, int time, bool bilinear)
        {
            uRetroConfig.capture_bilinear = bilinear;
            uRetroConfig.capture_downscale = downscale;
            uRetroConfig.capture_framerate = framerate;
            uRetroConfig.capture_time = time;
            uRetroConfig.capture_filename = filename;

            uRetroSystem.SaveRetroEngineConfig();
        }

        public static void Start()
        {
            if (screenCapture.capture) return;

            screenCapture.downscale = uRetroConfig.capture_downscale;
            screenCapture.frameRate = uRetroConfig.capture_framerate;
            screenCapture.captureTime = uRetroConfig.capture_time;
            screenCapture.filename = uRetroConfig.capture_filename;
            screenCapture.useBilinearScaling = uRetroConfig.capture_bilinear;

            screenCapture.capture = true;
        }
    }
}