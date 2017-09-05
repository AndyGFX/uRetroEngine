using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace uRetroEngine
{
    public static class uRetroVRAM
    {
        public static byte[] buffer = new byte[1];
        public static bool changed = false;
        private static byte[] emptyBuffer = new byte[1];

        public static int clipX = 0;
        public static int clipY = 0;
        public static int clipW = 256;
        public static int clipH = 240;

        /// <summary>
        ///
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="background_color_id"></param>
        public static void CreateBuffer(int w, int h, byte background_color_id)
        {
            buffer = new byte[w * h];
            emptyBuffer = new byte[w * h];
            for (int i = 0; i < (w * h); i++) emptyBuffer[i] = background_color_id;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void Pixel(int x, int y, byte color)
        {
            int nx = x * uRetroDisplay.pixelSizeX;
            int ny = y * uRetroDisplay.pixelSizeY;
            int idx = 0;

            // ignore pixel out of screen
            if (!uRetroUtils.IsPointInsideScreen(nx, ny)) return;

            if ((uRetroDisplay.pixelSizeX != 1) & (uRetroDisplay.pixelSizeY != 1))
            {
                for (int px = 0; px < uRetroDisplay.pixelSizeX; px++)
                {
                    for (int py = 0; py < uRetroDisplay.pixelSizeY; py++)
                    {
                        idx = uRetroUtils.ScreenPositionToIndex(nx + px, uRetroUtils.FlipPixelY(ny + py));
                        uRetroVRAM.buffer[idx] = color;
                    }
                }
            }
            else
            {
                idx = uRetroUtils.ScreenPositionToIndex(nx + 0, uRetroUtils.FlipPixelY(ny + 0));
                uRetroVRAM.buffer[idx] = color;
            }

            changed = true;
        }

        /// <summary>
        ///
        /// </summary>
        public static void ClearBuffer()
        {
            Array.Copy(emptyBuffer, buffer, emptyBuffer.Length);
        }

        public static void ClearBuffer(byte color)
        {
            int size = emptyBuffer.Length;
            for (int i = 0; i < size; i++) emptyBuffer[i] = color;
            Array.Copy(emptyBuffer, buffer, emptyBuffer.Length);
        }
    }
}