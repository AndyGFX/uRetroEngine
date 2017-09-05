using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public static class uRetroMetaSprites
    {
        //private static List<uRetroImage> metaSprites = new List<uRetroImage>();
        private static Hashtable metaSprites = new Hashtable();

        public static void Clear()
        {
            metaSprites.Clear();
        }

        public static int Count()
        {
            return metaSprites.Count;
        }

        public static void Add(string name, int gridWidth, int gridHeight, int[] ids, bool[] flipX, bool[] flipY)
        {
            int mW = gridWidth * uRetroConfig.sprite_width;
            int mH = gridHeight * uRetroConfig.sprite_height;

            uRetroImage metaSprite = new uRetroImage(mW, mH);

            int tmpID = 0;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    uRetroImage tmpSprite = uRetroSprites.GetSpriteRetroImage(ids[tmpID]).GetFlipedImage(flipX[tmpID], flipY[tmpID]);

                    for (int sx = 0; sx < tmpSprite.width; sx++)
                    {
                        for (int sy = 0; sy < tmpSprite.height; sy++)
                        {
                            byte colorID = tmpSprite.GetPixel(sx, sy);

                            metaSprite.SetPixel((gridWidth - 1 - x) * uRetroConfig.sprite_width + sx, y * uRetroConfig.sprite_height + sy, colorID);
                        }
                    }

                    tmpID++;
                }
            }

            metaSprites.Add(name, metaSprite);
        }

        public static void FlipSprite(int id, bool flipX, bool flipY)
        {
            ((uRetroImage)metaSprites[id]).Flip(flipX, flipY);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="transparent"></param>
        public static void Draw(string name, int x, int y, bool transparent = true)
        {
            uRetroUtils.DrawImage((uRetroImage)metaSprites[name], x, y, transparent);
        }

        public static void Store(string name)
        {
            ((uRetroImage)metaSprites[name]).Store();
        }

        public static void Restore(string name)
        {
            ((uRetroImage)metaSprites[name]).Restore();
        }
    }
}