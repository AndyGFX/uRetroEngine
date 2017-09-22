using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace uRetroEngine
{
    /// uRetroEngine utilities
    public static class uRetroUtils
    {
        public static GameObject codeProfiler;
        private static bool profilerState = false;

        /// <summary>
        /// Check if [x,y] is inside screen/cliped area
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsPointInsideScreen(int x, int y)
        {
            if (x < uRetroVRAM.clipX) return false;
            if (y < uRetroVRAM.clipY) return false;
            if (x > (uRetroVRAM.clipX + uRetroVRAM.clipW) - 1) return false;
            if (y > (uRetroVRAM.clipY + uRetroVRAM.clipH) - 1) return false;
            return true;
        }

        /// <summary>
        /// screen space to array index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int ScreenPositionToIndex(int x, int y)
        {
            return x + y * uRetroConfig.screen_width;
        }

        /// <summary>
        /// Pixel sprite postition to array index
        /// </summary>
        /// <param name="x">sprite x position</param>
        /// <param name="y">sprite y position</param>
        /// <param name="w">sprite width</param>
        /// <param name="h">sprite height</param>
        /// <returns></returns>
        public static int SpritePixelPositionToIndex(int x, int y, int w, int h)
        {
            return x + y * w;
        }

        /// <summary>
        /// Index to x,y position
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="width">array width</param>
        /// <param name="x">out of calculated x postion</param>
        /// <param name="y">out of calculated y position</param>
        public static void IndexToPosition(int index, int width, out int x, out int y)
        {
            x = index % width;
            y = index / width;
        }

        /// <summary>
        /// Direct draw uRetroImage to VRAM
        /// </summary>
        /// <param name="source">image source</param>
        /// <param name="x">screen x position</param>
        /// <param name="y">screen y position</param>
        /// <param name="transparent"></param>
        public static void DrawImage(uRetroImage source, int x, int y, bool transparent = true)
        {
            int idx = 0;
            if (uRetroConfig.flipScreenY)
            {
                for (int px = 0; px < source.width; px++)
                {
                    for (int py = 0; py < source.height; py++)
                    {
                        if (source.data[idx] == 0)
                        {
                            if (!transparent)
                            {
                                uRetroVRAM.Pixel(x + px, y + 7 - py, source.data[idx]);
                            }
                        }
                        else
                        {
                            uRetroVRAM.Pixel(x + px, y + 7 - py, source.data[idx]);
                        }
                        idx++;
                    }
                }
            }
            else
            {
                for (int px = 0; px < source.width; px++)
                {
                    for (int py = 0; py < source.height; py++)
                    {
                        if (source.data[idx] == 0)
                        {
                            if (!transparent)
                            {
                                uRetroVRAM.Pixel(x + px, y + py, source.data[idx]);
                            }
                        }
                        else
                        {
                            uRetroVRAM.Pixel(x + px, y + py, source.data[idx]);
                        }
                        idx++;
                    }
                }
            }
        }

        /// <summary>
        /// Draw uRetroImage to screen with color id replace
        /// </summary>
        /// <param name="source">image source</param>
        /// <param name="x">screen x position</param>
        /// <param name="y">screen y position</param>
        /// <param name="backgroundColor">background color (color id=0 is replaced)</param>
        /// <param name="foregroundColor">all iamge color are replaced with this color</param>
        public static void DrawImageWithFixedColor(uRetroImage source, int x, int y, byte backgroundColor = 255, byte foregroundColor = 1)
        {
            int idx = 0;

            if (uRetroConfig.flipScreenY)
            {
                for (int px = 0; px < source.width; px++)
                {
                    for (int py = 0; py < source.height; py++)
                    {
                        if (source.data[idx] == 0)
                        {
                            if (backgroundColor != 255)
                            {
                                uRetroVRAM.Pixel(x + px, (uRetroConfig.sprite_height - 1) + y - py, backgroundColor);
                            }
                        }
                        else
                        {
                            uRetroVRAM.Pixel(x + px, (uRetroConfig.sprite_height - 1) + y - py, foregroundColor);
                        }
                        idx++;
                    }
                }
            }
            else
            {
                for (int px = 0; px < source.width; px++)
                {
                    for (int py = 0; py < source.height; py++)
                    {
                        if (source.data[idx] == 0)
                        {
                            if (backgroundColor != 255)
                            {
                                uRetroVRAM.Pixel(x + px, y + py, backgroundColor);
                            }
                        }
                        else
                        {
                            uRetroVRAM.Pixel(x + px, y + py, foregroundColor);
                        }
                        idx++;
                    }
                }
            }
        }

        /// <summary>
        /// Converet HEX stgring to color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            if (hex == null)
            {
                hex = "FF00FF";
            }

            if (hex[0] == '#')
                hex = hex.Substring(1);

            Color res = new Color();

            res.a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / (float)byte.MaxValue;
            res.r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / (float)byte.MaxValue;
            res.g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / (float)byte.MaxValue;
            res.b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber) / (float)byte.MaxValue;

            return res;
        }

        /// <summary>
        /// Convert HEX color to color32
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color32 HexToColor32(string hex)
        {
            if (hex == null)
            {
                hex = "FF00FF";
            }

            if (hex[0] == '#')
                hex = hex.Substring(1);
            hex.ToUpper();
            Color32 res = new Color32();
            res.a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            res.r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            res.g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            res.b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return res;
        }

        /// <summary>
        /// Convert COlor32 to HEX string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string Color32ToHex(Color32 color)
        {
            return string.Format("#{0}{1}{2}{3}", color.a.ToString("X2"), color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        /// <summary>
        /// Convert Color to HEX string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color color)
        {
            Color32 color32 = (Color32)color;
            return string.Format("#{0}{1}{2}{3}", color32.a.ToString("X2"), color32.r.ToString("X2"), color32.g.ToString("X2"), color32.b.ToString("X2"));
        }

        /// <summary>
        /// Convert y position by screen origin (bottom-left/top_left)
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int FlipPixelY(int y)
        {
            if (uRetroConfig.flipScreenY)
            {
                return (uRetroConfig.screen_height - 1) - y;
            }
            return y;
        }

        /// <summary>
        /// Convert y tile position by screen origin (bottom-left/top_left)
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int FlipTileY(int y)
        {
            if (uRetroConfig.flipScreenY)
            {
                return (uRetroConfig.tilemap_height - 1) - y;
            }
            return y;
        }

        /// <summary>
        /// Start codeprofile timer
        /// </summary>
        /// <param name="label">timer name</param>
        public static void CodeProfilerStart(string label)
        {
            if (codeProfiler != null) CodeProfiler.Begin(label);
        }

        /// <summary>
        /// Stop codeprofile timer
        /// </summary>
        /// <param name="label">timer name</param>
        public static void CodeProfilerEnd(string label)
        {
            if (codeProfiler != null) CodeProfiler.End(label);
        }

        /// <summary>
        /// Show code profiler
        /// </summary>
        public static void ShowProfiler()
        {
            profilerState = true;
            if (codeProfiler != null) codeProfiler.SetActive(profilerState);
        }

        /// <summary>
        /// Hide codeprofiler
        /// </summary>
        public static void HideProfiler()
        {
            profilerState = false;
            if (codeProfiler != null) codeProfiler.SetActive(profilerState);
        }

        /// <summary>
        /// Switch codeprofiler visibility
        /// </summary>
        public static void SwitchProfiler()
        {
            profilerState = !profilerState;
            if (codeProfiler != null) codeProfiler.SetActive(profilerState);
        }
    }
}