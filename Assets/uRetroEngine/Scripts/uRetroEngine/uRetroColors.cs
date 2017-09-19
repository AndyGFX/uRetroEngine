using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public static class uRetroColors
    {
        public static Color[] colors = new Color[1];
        public static Color[] _backupColors = new Color[1];

        /// <summary>
        /// Create Retro palette from image
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

        public static void Restore()
        {
            for (int i = 0; i < uRetroConfig.max_colors; i++)
            {
                colors[i] = _backupColors[i];
            }
        }

        public static void Set(byte ID, byte r, byte g, byte b, byte a)
        {
            colors[ID] = (Color)(new Color32(r, g, b, a));
        }

        public static Color32 Get(byte ID)
        {
            return colors[ID];
        }

        public static string GetAsHex(byte ID)
        {
            return uRetroUtils.ColorToHex(colors[ID]);
        }

        public static byte GetColorIndex(Color color)
        {
            byte c_id = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == color) c_id = (byte)i;
            }

            return c_id;
        }

        public static string[] GetColorsAsHexArray()
        {
            string[] res = new string[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                res[i] = uRetroUtils.Color32ToHex(colors[i]);
            }

            return res;
        }

        public static void SetFromHex(int id, string hexColor)
        {
            colors[id] = uRetroUtils.HexToColor32(hexColor);
        }

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