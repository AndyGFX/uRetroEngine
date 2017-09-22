using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace uRetroEngine
{
    /// <summary>
    /// VRAM class
    /// </summary>
    public static class uRetroVRAM
    {
        /// <summary>
        /// VRAm array
        /// </summary>
        public static byte[] buffer = new byte[1];

        public static bool changed = false;
        private static byte[] emptyBuffer = new byte[1];

        /// <summary>
        /// Default clipping area
        /// </summary>
        public static int clipX = 0;

        public static int clipY = 0;
        public static int clipW = 256;
        public static int clipH = 240;

        /// <summary>
        /// Create VRAm buffer
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
        /// Write pixel to VRAM
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="color">color id</param>
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
                        //idx = uRetroUtils.ScreenPositionToIndex(nx + px, uRetroUtils.FlipPixelY(ny + py));
                        //uRetroVRAM.buffer[idx] = color;
                        uRetroVRAM.buffer[(nx + px) + uRetroUtils.FlipPixelY(ny + py) * uRetroConfig.screen_width] = color;
                    }
                }
            }
            else
            {
                //idx = uRetroUtils.ScreenPositionToIndex(nx + 0, uRetroUtils.FlipPixelY(ny + 0));
                //uRetroVRAM.buffer[idx] = color;
                uRetroVRAM.buffer[nx + uRetroUtils.FlipPixelY(ny) * uRetroConfig.screen_width] = color;
            }

            changed = true;
        }

        /// <summary>
        /// Clear VRAm buffer
        /// </summary>
        public static void ClearBuffer()
        {
            Array.Copy(emptyBuffer, buffer, emptyBuffer.Length);
        }

        /// <summary>
        /// Clear VRAM buffer with defind color
        /// </summary>
        /// <param name="color"></param>
        public static void ClearBuffer(byte color)
        {
            int size = emptyBuffer.Length;
            for (int i = 0; i < size; i++) emptyBuffer[i] = color;
            Array.Copy(emptyBuffer, buffer, emptyBuffer.Length);
        }
    }
}