using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace uRetroEngine
{
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

        public static void CreateDisplay(RawImage _display)
        {
            display = _display;

            Clip();
        }

        public static void Clip()
        {
            uRetroVRAM.clipX = 0;
            uRetroVRAM.clipY = 0;
            uRetroVRAM.clipW = uRetroConfig.screen_width;
            uRetroVRAM.clipH = uRetroConfig.screen_height;
        }

        public static void Clip(int x, int y, int width, int height)
        {
            uRetroVRAM.clipX = x;
            uRetroVRAM.clipY = y;
            uRetroVRAM.clipW = width;
            uRetroVRAM.clipH = height;
        }

        public static void CreateDisplay(GameObject _display)
        {
            display = _display.GetComponent<RawImage>();
        }

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

        public static void Flip()
        {
            FlipBuffer();

            //uRetroDisplay.instance.FlipBufferPerSegment();
        }

        public static void PixelSize(int pSizeX, int pSizeY)
        {
            pixelSizeX = pSizeX;
            pixelSizeY = pSizeY;
        }

        // consume 91% of CPU time
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
                        backBuffer[idx] = (Color)uRetroColors.colors[uRetroVRAM.buffer[idx]];
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