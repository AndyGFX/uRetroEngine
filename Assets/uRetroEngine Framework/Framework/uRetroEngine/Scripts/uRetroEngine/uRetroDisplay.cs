using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace uRetroEngine
{
    /// <summary>
    /// Scene dyspplay componet for rendering to texture from uRetro VRAM
    /// </summary>
    public class uRetroDisplay : MonoBehaviour
    {
        public static Texture2D displayBuffer;
        public static RectTransform displayTarget;
        public static int pixelSizeX = 1;
        public static int pixelSizeY = 1;
        private static RawImage display;
        private static int segmentCount = 8;
        private static int segmentHeight = 0;
        private static Color[] backBuffer = null;
        public static bool callScanline = false;
        public static uRetroDisplay instance = null;

        private void Awake()
        {
            uRetroDisplay.instance = this;
        }

        private void Update()
        {
        }

        /// <summary>
        /// Create display from canvas RAWImage
        /// </summary>
        /// <param name="_display"></param>
        public static void CreateDisplay(RawImage _display)
        {
            display = _display;

            Clip();
        }

        /// <summary>
        /// Reset rendering area back to screen size
        /// </summary>
        public static void Clip()
        {
            uRetroVRAM.clipX = 0;
            uRetroVRAM.clipY = 0;
            uRetroVRAM.clipW = uRetroConfig.screen_width;
            uRetroVRAM.clipH = uRetroConfig.screen_height;
        }

        /// <summary>
        /// Define rendering area
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Clip(int x, int y, int width, int height)
        {
            uRetroVRAM.clipX = x;
            uRetroVRAM.clipY = y;
            uRetroVRAM.clipW = width;
            uRetroVRAM.clipH = height;
        }

        /// <summary>
        /// Create display from scene gameObject (where is assigned UI RAWImage)
        /// </summary>
        /// <param name="_display"></param>
        public static void CreateDisplay(GameObject _display)
        {
            display = _display.GetComponent<RawImage>();
        }

        /// <summary>
        /// Set screen resolution and set VRAM size
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="background_color_id"></param>
        public static void SetResolution(int w, int h, byte background_color_id)
        {
            if (display == null)
            {
                Debug.LogError("uRetroDisplay: Can't set resolution before create Display");
                return;
            }

            uRetroConfig.screen_width = w;
            uRetroConfig.screen_height = h;

            // set screen clipping rectangle
            Clip();

            float scale = h / (float)w;
            float fitScale = 1 / scale;
            displayBuffer = new Texture2D(w, h, TextureFormat.RGBA32, false);
            displayBuffer.filterMode = FilterMode.Point;

            displayBuffer.Apply();

            display.rectTransform.transform.localScale = new Vector3(1f, h / (float)w, 1f);
            Vector3 locScale = display.rectTransform.transform.localScale * fitScale;

            display.rectTransform.transform.localScale = locScale;

            uRetroVRAM.CreateBuffer(w, h, background_color_id);
            backBuffer = new Color[displayBuffer.GetPixels32().Length];
        }

        /// <summary>
        /// Flip VRAM backbuffer to screen
        /// </summary>
        public static void Flip()
        {
            FlipBuffer();

            //uRetroDisplay.instance.FlipBufferPerSegment();
        }

        /// <summary>
        /// Set pixel size
        /// </summary>
        /// <param name="pSizeX">x size</param>
        /// <param name="pSizeY">y size</param>
        public static void PixelSize(int pSizeX, int pSizeY)
        {
            pixelSizeX = pSizeX;
            pixelSizeY = pSizeY;
        }

        /// <summary>
        /// Flip VRAm buffer to texture (convert color ID to RGB color)
        /// </summary>
        public static void FlipBuffer()
        {
            if (!uRetroVRAM.changed) return;

            int idx = 0;

            for (int y = 0; y < uRetroConfig.screen_height; y++)
            {
                if (callScanline) uRetroLua.OnScaline(y);

                for (int x = 0; x < uRetroConfig.screen_width; x++)
                {
                    if (uRetroVRAM.buffer[idx] == 0)
                    {
                        backBuffer[idx] = Color.black;
                    }
                    else
                    {
                        backBuffer[idx] = uRetroColors.colors[uRetroVRAM.buffer[idx]];
                    }
                    idx++;
                }
            }
            /*
            for (int idx = 0; idx < uRetroConfig.screen_height * uRetroConfig.screen_width; idx++)
            {
                if (uRetroVRAM.buffer[idx] == 0)
                {
                    backBuffer[idx] = Color.black;
                }
                else
                {
                    backBuffer[idx] = (Color)uRetroColors.colors[uRetroVRAM.buffer[idx]];
                }
            }
            */
            displayBuffer.SetPixels(backBuffer);
            displayBuffer.Apply();
            display.texture = displayBuffer;
            uRetroVRAM.changed = false;
        }

        // consume 53% of CPU time
        private void FlipBufferPerSegment()
        {
            if (!uRetroVRAM.changed) return;

            segmentHeight = uRetroConfig.screen_height / segmentCount;

            for (int y = 0; y < segmentCount; y++)
            {
                int sStart = y * segmentHeight;
                int sEnd = (y * segmentHeight) + segmentHeight;
                StartCoroutine(DrawBufferRegion(sStart, sEnd, (y == segmentCount - 1) ? true : false));
            }
        }

        /// <summary>
        /// Clear buffer (screen) with default color id=0
        /// </summary>
        /// <param name="color">color id from palette</param>
        public static void Clear(byte color = 0)
        {
            uRetroVRAM.ClearBuffer(color);
        }

        private IEnumerator DrawBufferRegion(int segmentStart, int segmentEnd, bool isDone)
        {
            int idx = segmentStart * uRetroConfig.screen_width;

            for (int y = segmentStart; y < segmentEnd; y++)
            {
                for (int x = 0; x < uRetroConfig.screen_width; x++)
                {
                    Color c = uRetroColors.colors[uRetroVRAM.buffer[idx++]];
                    displayBuffer.SetPixel(x, y, c);
                }
            }

            if (isDone)
            {
                displayBuffer.Apply();
                display.texture = displayBuffer;
                uRetroVRAM.changed = false;
            }
            yield return 0;
        }
    }
}