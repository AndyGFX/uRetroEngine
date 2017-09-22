using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    /// <summary>
    /// Capture screen to animated gif
    /// </summary>
    public static class uRetroCapture
    {
        private static CaptureToGIF screenCapture = null;

        /// <summary>
        /// Initialize uGif from scene component
        /// </summary>
        public static void Init()
        {
            screenCapture = Camera.main.GetComponent<CaptureToGIF>();
        }

        /// <summary>
        /// Setup uGif for take animation
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="framerate"></param>
        /// <param name="downscale"></param>
        /// <param name="time"></param>
        /// <param name="bilinear"></param>
        public static void Setup(string filename, int framerate, int downscale, int time, bool bilinear)
        {
            uRetroConfig.capture_bilinear = bilinear;
            uRetroConfig.capture_downscale = downscale;
            uRetroConfig.capture_framerate = framerate;
            uRetroConfig.capture_time = time;
            uRetroConfig.capture_filename = filename;

            uRetroSystem.SaveRetroEngineConfig();
        }

        /// <summary>
        /// Start snapshot to animation
        /// </summary>
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