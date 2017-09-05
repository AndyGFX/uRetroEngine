using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public static class uRetroColors
    {
        public static Color32[] colors = new Color32[1];
        public static Color32[] _backupColors = new Color32[1];

        /// <summary>
        /// Create Retro palette from image
        /// Scan image and create indexed color array
        /// </summary>
        /// <param name="palette">image color palette</param>
        public static void CreatePalette(Texture2D palette)
        {
            /*
            colors = new Color32[palette.GetPixels().Length];
            _backupColors = new Color32[palette.GetPixels().Length];

            int i = 0;
            for (int x = 0; x < palette.width; x++)
            {
                for (int y = palette.height - 1; y >= 0; y--)
                {
                    colors[i] = (Color32)palette.GetPixel(x, y);
                    _backupColors[i++] = (Color32)palette.GetPixel(x, y);
                }
            }
            */

            colors = new Color32[uRetroConfig.max_colors];
            _backupColors = new Color32[uRetroConfig.max_colors];

            for (int x = 0; x < palette.width; x++)
            {
                colors[x] = (Color32)palette.GetPixel(x, 0);
                _backupColors[x] = (Color32)palette.GetPixel(x, 0);
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
            colors[ID] = new Color32(r, g, b, a);
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
            colors = new Color32[palette.Length];
            for (int i = 0; i < palette.Length; i++)
            {
                colors[i] = uRetroUtils.HexToColor32(palette[i]);
            }
        }
    }
}