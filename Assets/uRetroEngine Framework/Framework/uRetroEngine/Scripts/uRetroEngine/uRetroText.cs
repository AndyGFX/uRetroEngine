using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace uRetroEngine
{
    /// <summary>
    /// Font definition
    /// </summary>
    public struct FontData
    {
        public int fontID;
        public int fontOffset;
        public int fontSpacing;
    }

    /// <summary>
    /// Text class
    /// </summary>
    public static class uRetroText
    {
        // TODO: change to array instead List
        public static List<uRetroImage> characters;

        private static FontData[] fonts = new FontData[4];
        private static int currentFont = 0;

        /// <summary>
        /// Create fonts from texture
        /// </summary>
        /// <param name="texture">texture 2d</param>
        public static void CreateFont(Texture2D texture)
        {
            if (texture.width % uRetroConfig.sprite_width != 0)
            {
                Debug.LogError("Character source image width is not power of 8");
                return;
            }

            if (texture.height % uRetroConfig.sprite_height != 0)
            {
                Debug.LogError("Character source image height is not power of 8");
                return;
            }

            characters = new List<uRetroImage>();

            for (int y = texture.height / uRetroConfig.sprite_height; y >= 0; y--)
            {
                for (int x = 0; x < texture.width / uRetroConfig.sprite_width; x++)
                {
                    uRetroImage character = new uRetroImage(uRetroConfig.sprite_width, uRetroConfig.sprite_height);
                    int idx = 0;

                    // char cell (sprite width x sprite height)
                    for (int sx = 0; sx < uRetroConfig.sprite_width; sx++)
                    {
                        for (int sy = 0; sy < uRetroConfig.sprite_height; sy++)
                        {
                            Color c = texture.GetPixel(x * uRetroConfig.sprite_width + sx, y * uRetroConfig.sprite_height + sy - 8);
                            character.data[idx] = uRetroColors.GetColorIndex(c);
                            idx++;
                        }
                    }

                    characters.Add(character);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                SetFont(i, 0, uRetroConfig.charSpacing);
            }
        }

        /// <summary>
        /// Set font (when multiple fonts are in texture)
        /// </summary>
        /// <param name="fontID">font id</param>
        /// <param name="fontCharStart">font start</param>
        /// <param name="fontSpacing">character spacing</param>
        public static void SetFont(int fontID, int fontCharStart, int fontSpacing)
        {
            fonts[fontID].fontID = fontID;
            fonts[fontID].fontOffset = fontCharStart;
            fonts[fontID].fontSpacing = fontSpacing;
        }

        /// <summary>
        /// Set current font for DrawText
        /// </summary>
        /// <param name="fontID"></param>
        public static void Font(int fontID)
        {
            currentFont = fontID;
        }

        /// <summary>
        /// Draw text to screen
        /// </summary>
        /// <param name="x">screen x position (pixels)</param>
        /// <param name="y">screen y position (pixels)</param>
        /// <param name="text">text</param>
        public static void Draw(int x, int y, string text)
        {
            char[] chars = text.ToCharArray();

            int fStart = fonts[currentFont].fontOffset;
            int fSpace = fonts[currentFont].fontSpacing;
            int fID = 0;
            for (int idx = fStart; idx < chars.Length + fStart; idx++)
            {
                int char_id = (int)chars[idx - fStart] - 32;
                if (char_id > 0)
                {
                    int tx = x + fID * fSpace;
                    int ty = y;
                    uRetroUtils.DrawImage(characters[char_id + fStart], tx, ty);
                }
                fID++;
            }
        }

        /// <summary>
        /// Draw text to screen
        /// </summary>
        /// <param name="x">screen x position (pixels)</param>
        /// <param name="y">screen y position (pixels)</param>
        /// <param name="text">text</param>
        /// <param name="frontColor">replace all color to this color id</param>
        /// <param name="backgroundColor">fill transparen color with tis color</param>
        public static void Draw(int x, int y, string text, byte frontColor, int backgroundColor = -1)
        {
            char[] chars = text.ToCharArray();

            int fStart = fonts[currentFont].fontOffset;
            int fSpace = fonts[currentFont].fontSpacing;
            int fID = 0;
            for (int idx = fStart; idx < chars.Length + fStart; idx++)
            {
                int char_id = (int)chars[idx - fStart] - 32;
                if (char_id >= 0)
                {
                    int tx = x + fID * fSpace;
                    int ty = y;

                    if (backgroundColor < 0)
                    {
                        uRetroUtils.DrawImageWithFixedColor(characters[char_id + fStart], tx, ty, 255, frontColor);
                    }
                    else
                    {
                        uRetroUtils.DrawImageWithFixedColor(characters[char_id + fStart], tx, ty, (byte)backgroundColor, frontColor);
                    }
                }

                fID++;
            }
        }

        /// <summary>
        /// Get charatcer as uRetroImage array
        /// </summary>
        /// <returns></returns>
        public static uRetroImage[] GetCharsAsArray()
        {
            return characters.ToArray();
        }

        /// <summary>
        /// Create fonts from uRetroImage array
        /// </summary>
        /// <param name="images"></param>
        public static void CreateFromRetroImageList(uRetroImage[] images)
        {
            characters = new List<uRetroImage>();
            characters.AddRange(images);
        }

        /// <summary>
        /// Create texture from fonts
        /// </summary>
        /// <returns></returns>
        public static Texture2D GetAsImage()
        {
            int w = 16;
            int h = Mathf.FloorToInt(characters.Count % w) > 0f ? Mathf.FloorToInt(characters.Count / w) + 1 : Mathf.FloorToInt(characters.Count / w);

            Texture2D img = new Texture2D(w * uRetroConfig.sprite_width, h * uRetroConfig.sprite_height);

            int idx = 0;
            for (int r = h - 1; r >= 0; r--)
            {
                for (int c = 0; c < w; c++)
                {
                    if (idx < characters.Count)
                    {
                        for (int x = 0; x < uRetroConfig.sprite_width; x++)
                        {
                            for (int y = 0; y < uRetroConfig.sprite_height; y++)
                            {
                                byte colID = characters[idx].GetPixel(y, x);
                                Color32 color = uRetroColors.Get(colID);
                                img.SetPixel(c * uRetroConfig.sprite_width + x, r * uRetroConfig.sprite_height + y, color);
                            }
                        }
                    }
                    idx++;
                }
            }

            return img;
        }
    }
}