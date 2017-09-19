using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace uRetroEngine
{
    public static class uRetroUtils
    {
        public static GameObject codeProfiler;
        private static bool profilerState = false;

        public static bool IsPointInsideScreen(int x, int y)
        {
            if (x < uRetroVRAM.clipX) return false;
            if (y < uRetroVRAM.clipY) return false;
            if (x > (uRetroVRAM.clipX + uRetroVRAM.clipW) - 1) return false;
            if (y > (uRetroVRAM.clipY + uRetroVRAM.clipH) - 1) return false;
            return true;
        }

        public static int ScreenPositionToIndex(int x, int y)
        {
            return x + y * uRetroConfig.screen_width;
        }

        public static int SpritePixelPositionToIndex(int x, int y, int w, int h)
        {
            return x + y * w;
        }

        public static void IndexToPosition(int index, int width, out int x, out int y)
        {
            x = index % width;
            y = index / width;
        }

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

        public static string Color32ToHex(Color32 color)
        {
            return string.Format("#{0}{1}{2}{3}", color.a.ToString("X2"), color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        public static string ColorToHex(Color color)
        {
            Color32 color32 = (Color32)color;
            return string.Format("#{0}{1}{2}{3}", color32.a.ToString("X2"), color32.r.ToString("X2"), color32.g.ToString("X2"), color32.b.ToString("X2"));
        }

        public static int FlipPixelY(int y)
        {
            if (uRetroConfig.flipScreenY)
            {
                return (uRetroConfig.screen_height - 1) - y;
            }
            return y;
        }

        public static int FlipTileY(int y)
        {
            if (uRetroConfig.flipScreenY)
            {
                return (uRetroConfig.tilemap_height - 1) - y;
            }
            return y;
        }

        public static void CodeProfilerStart(string label)
        {
            if (codeProfiler != null) CodeProfiler.Begin(label);
        }

        public static void CodeProfilerEnd(string label)
        {
            if (codeProfiler != null) CodeProfiler.End(label);
        }

        public static void ShowProfiler()
        {
            profilerState = true;
            if (codeProfiler != null) codeProfiler.SetActive(profilerState);
        }

        public static void HideProfiler()
        {
            profilerState = false;
            if (codeProfiler != null) codeProfiler.SetActive(profilerState);
        }

        public static void SwitchProfiler()
        {
            profilerState = !profilerState;
            if (codeProfiler != null) codeProfiler.SetActive(profilerState);
        }
    }
}