using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace uRetroEngine
{
    public class uRetroImage
    {
        public int width = 8;
        public int height = 8;
        public byte[] data;
        public byte[] stored;
        public int flags = 0;

        public uRetroImage(int w, int h)
        {
            this.width = w;
            this.height = h;
            this.data = new byte[width * height];
            this.stored = new byte[1];
        }

        public void Flip(bool flipH, bool flipV)
        {
            byte[] tmpData = new byte[width * height];

            Array.Copy(this.data, tmpData, this.data.Length);

            for (var iy = 0; iy < this.height; iy++)
            {
                for (var ix = 0; ix < this.width; ix++)
                {
                    var newX = ix;
                    var newY = iy;
                    if (flipH) newX = this.width - 1 - ix;
                    if (flipV) newY = this.height - 1 - iy;
                    this.data[iy + ix * this.height] = this.data[newY + newX * this.height];
                }
            }
        }

        public uRetroImage GetFlipedImage(bool flipX, bool flipY)
        {
            uRetroImage res = new uRetroImage(this.width, this.height);
            Array.Copy(data, res.data, this.data.Length);

            for (var iy = 0; iy < this.height; iy++)
            {
                for (var ix = 0; ix < this.width; ix++)
                {
                    var newx = ix;
                    var newy = iy;
                    if (flipX) newx = this.width - 1 - ix;
                    if (flipY) newy = this.height - 1 - iy;
                    res.data[iy + ix * this.height] = this.data[newy + newx * this.height];
                }
            }

            return res;
        }

        public void SetPixel(int x, int y, byte colorID)
        {
            this.data[uRetroUtils.SpritePixelPositionToIndex(x, y, this.width, this.height)] = colorID;
        }

        public byte GetPixel(int x, int y)
        {
            return this.data[uRetroUtils.SpritePixelPositionToIndex(x, y, this.width, this.height)];
        }

        public void Store()
        {
            Array.Copy(this.data, this.stored, data.Length);
        }

        public void Restore()
        {
            Array.Copy(this.stored, this.data, data.Length);
        }
    }
}