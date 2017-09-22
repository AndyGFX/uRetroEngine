using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    /// <summary>
    /// Engine color palette
    /// </summary>
    public static class uRetroColors
    {
        public static Color[] colors = new Color[1];
        public static Color[] _backupColors = new Color[1];

        /// <summary>
        /// Create Retro palette from image (defined in one line)
        /// Scan image and create indexed color array
        /// </summary>
        /// <param name="palette">image color palette</param>
        public static void CreatePalette(Texture2D palette)
        {
            colors = new Color[uRetroConfig.max_colors];
            _backupColors = new Color[uRetroConfig.max_colors];

            for (int x = 0; x < palette.width; x++)
            {
                colors[x] = palette.GetPixel(x, 0);
                _backupColors[x] = palette.GetPixel(x, 0);
            }
        }

        /// <summary>
        /// Get palette as image
        /// </summary>
        /// <returns></returns>
        public static Texture2D GetAsImage()
        {
            Texture2D img = new Texture2D(uRetroConfig.max_colors, 1);
            for (int i = 0; i < uRetroConfig.max_colors; i++)
            {
                img.SetPixel(i, 0, _backupColors[i]);
            }
            img.Apply();
            return img;
        }

        /// <summary>
        /// Restore palette to palatte stored on start
        /// </summary>
        public static void Restore()
        {
            for (int i = 0; i < uRetroConfig.max_colors; i++)
            {
                colors[i] = _backupColors[i];
            }
        }

        /// <summary>
        /// Set new color in platte for index ID
        /// </summary>
        /// <param name="ID">color index max. 255</param>
        /// <param name="r">red 0..255</param>
        /// <param name="g">green 0..255</param>
        /// <param name="b">blue 0..255</param>
        /// <param name="a">alpha 0..255</param>
        public static void Set(byte ID, byte r, byte g, byte b, byte a)
        {
            colors[ID] = (Color)(new Color32(r, g, b, a));
        }

        /// <summary>
        /// Get color at ID in COlor32 format
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Color32 Get(byte ID)
        {
            return colors[ID];
        }

        /// <summary>
        /// Get color at ID as HEX value
        /// </summary>
        /// <param name="ID">color index in palette</param>
        /// <returns></returns>
        public static string GetAsHex(byte ID)
        {
            return uRetroUtils.ColorToHex(colors[ID]);
        }

        /// <summary>
        /// Find color palette id by color
        /// </summary>
        /// <param name="color">RGBA color (0..1) float</param>
        /// <returns></returns>
        public static byte GetColorIndex(Color color)
        {
            byte c_id = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == color) c_id = (byte)i;
            }

            return c_id;
        }

        /// <summary>
        /// Get palette table as string list in HEX format
        /// </summary>
        /// <returns></returns>
        public static string[] GetColorsAsHexArray()
        {
            string[] res = new string[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                res[i] = uRetroUtils.Color32ToHex(colors[i]);
            }

            return res;
        }

        /// <summary>
        /// Set color palette at ID from HEX defintion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hexColor"></param>
        public static void SetFromHex(int id, string hexColor)
        {
            colors[id] = uRetroUtils.HexToColor32(hexColor);
        }

        /// <summary>
        /// Create color palette from HEX string array
        /// </summary>
        /// <param name="palette"></param>
        public static void CreateFromHex(string[] palette)
        {
            colors = new Color[palette.Length];
            for (int i = 0; i < palette.Length; i++)
            {
                colors[i] = uRetroUtils.HexToColor(palette[i]);
            }
        }
    }
}