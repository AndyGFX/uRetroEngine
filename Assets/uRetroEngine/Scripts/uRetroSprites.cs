using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace uRetroEngine
{
    public static class uRetroSprites
    {
        // TODO: change to array instead List
        private static List<uRetroImage> sprites;

        //private static uRetroImage[] sprites;

        private static int sheetWidth = 0;
        private static int sheetHeight = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="texture"></param>
        public static void CreateSprites(Texture2D texture)
        {
            if (texture.width % uRetroConfig.sprite_width != 0)
            {
                Debug.LogError("Sprites source image width is not power of 8");
                return;
            }

            if (texture.height % uRetroConfig.sprite_height != 0)
            {
                Debug.LogError("Sprites source image height is not power of 8");
                return;
            }

            sprites = new List<uRetroImage>();

            // Sprite grid [X,Y] = (8x8 pixels)

            sheetWidth = texture.width;
            sheetHeight = texture.height;

            for (int y = texture.height / uRetroConfig.sprite_height; y >= 0; y--)
            {
                for (int x = 0; x < texture.width / uRetroConfig.sprite_width; x++)
                {
                    uRetroImage sprite = new uRetroImage(uRetroConfig.sprite_width, uRetroConfig.sprite_height);
                    int idx = 0;
                    // sprite cell (sprite width x sprite height)
                    for (int sx = 0; sx < uRetroConfig.sprite_width; sx++)
                    {
                        for (int sy = 0; sy < uRetroConfig.sprite_height; sy++)
                        {
                            Color c = texture.GetPixel(x * uRetroConfig.sprite_width + sx, y * uRetroConfig.sprite_height + sy - 8);
                            sprite.data[idx] = uRetroColors.GetColorIndex(c);
                            idx++;
                        }
                    }

                    sprites.Add(sprite);
                }
            }

            //Debug.Log("Sprites count: " + sprites.Count);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="transparent"></param>
        public static void DrawSprite(int id, int x, int y, bool flipX = false, bool flipY = false, bool transparent = true)
        {
            uRetroImage sprite = sprites[id].GetFlipedImage(flipX, flipY);
            uRetroUtils.DrawImage(sprite, x, y, transparent);
        }

        public static void FlipSprite(int id, bool flipX, bool flipY)
        {
            sprites[id].Flip(flipX, flipY);
        }

        public static uRetroImage GetSpriteRetroImage(int id)
        {
            return sprites[id];
        }

        public static uRetroImage[] GetSpritesAsArray()
        {
            return sprites.ToArray();
        }

        public static void SetPixel(int ID, int x, int y, byte colorID)
        {
            if ((x < 0) || (x >= uRetroConfig.sprite_width) || (y < 0) || (y >= uRetroConfig.sprite_height))
            {
                Debug.LogError("uRE: position in sprite is out of sprite size!");
                return;
            }

            sprites[ID].data[x + y * uRetroConfig.sprite_width] = colorID;
        }

        public static byte GetPixel(int ID, int x, int y)
        {
            if ((x < 0) || (x >= uRetroConfig.sprite_width) || (y < 0) || (y >= uRetroConfig.sprite_height))
            {
                Debug.LogError("uRE: position in sprite is out of sprite size!");
                return 0;
            }

            return sprites[ID].data[x + y * uRetroConfig.sprite_width];
        }

        public static void Store(int ID)
        {
            sprites[ID].Store();
        }

        public static void Restore(int ID)
        {
            sprites[ID].Restore();
        }

        public static void CreateFromRetroImageList(uRetroImage[] images)
        {
            sprites = new List<uRetroImage>();
            sprites.AddRange(images);
        }

        public static Texture2D GetAsImage()
        {
            int w = 16;
            int h = Mathf.FloorToInt(sprites.Count % w) > 0f ? Mathf.FloorToInt(sprites.Count / w) + 1 : Mathf.FloorToInt(sprites.Count / w);

            Texture2D img = new Texture2D(w * uRetroConfig.sprite_width, h * uRetroConfig.sprite_height);

            int idx = 0;
            for (int r = h - 1; r >= 0; r--)
            {
                for (int c = 0; c < w; c++)
                {
                    if (idx < sprites.Count)
                    {
                        for (int x = 0; x < uRetroConfig.sprite_width; x++)
                        {
                            for (int y = 0; y < uRetroConfig.sprite_height; y++)
                            {
                                byte colID = sprites[idx].GetPixel(y, x);
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