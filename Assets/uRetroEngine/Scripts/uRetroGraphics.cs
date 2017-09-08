using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    /// <summary>
    /// Graphics function for draw pixel, line, rectange and circle
    /// </summary>
    public static class uRetroGraphics
    {
        /// <summary>
        /// Draw pixel color id at [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void PutPixel(int x, int y, byte color)
	    {
		    
		    uRetroVRAM.Pixel(x, y, color);
		    
        }

        /// <summary>
        /// Get pixel color ID at [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static byte GetPixel(int x, int y)
        {
	        return uRetroVRAM.buffer[uRetroUtils.ScreenPositionToIndex(x, uRetroUtils.FlipPixelY(y))];
	        
        }

        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="x0">from x position</param>
        /// <param name="y0">fromn y position</param>
        /// <param name="x1">to x position</param>
        /// <param name="y1">to y posiyion</param>
        /// <param name="color">color id</param>
        public static void DrawLine(int x1, int y1, int x2, int y2, byte color)
        {
            float L;
            int xx = 0;
            int yy = 0;

            if (Mathf.Abs(x2 - x1) > Mathf.Abs(y2 - y1))
            {
                L = Mathf.Abs(x2 - x1);
            }
            else
            {
                L = Mathf.Abs(y2 - y1);
            }

            double dx = (x2 - x1) / L;
            double dy = (y2 - y1) / L;
            double x = x1;
            double y = y1;

            xx = (int)x;
            yy = (int)y;
            PutPixel(xx, yy, color);

            for (int i = 0; i < L; i += 1)
            {
                if (x1 != x2)
                {
                    x = x + dx;
                }

                if (y1 != y2)
                {
                    y = y + dy;
                }
                xx = (int)x;
                yy = (int)y;
                PutPixel(xx, yy, color);
            }
        }

        /// <summary>
        /// Draw rectangle
        /// </summary>
        /// <param name="x0">from x position</param>
        /// <param name="y0">fromn y position</param>
        /// <param name="x1">to x position</param>
        /// <param name="y1">to y posiyion</param>
        /// <param name="color">color id</param>
        /// <param name="filled">enable/disable fill</param>
        public static void DrawRectangle(int x0, int y0, int x1, int y1, byte color, bool filled = false)
        {
            int Ax = x0;
            int Ay = y0;

            int Bx = x1;
            int By = y0;

            int Cx = x1;
            int Cy = y1;

            int Dx = x0;
            int Dy = y1;

            if (filled)
            {
                if (y0 < y1)
                {
                    for (int yy = y0; yy <= y1; yy++)
                    {
                        DrawLine(Ax, yy, Bx, yy, color);
                    }
                }
                else
                {
                    for (int yy = y1; yy <= y0; yy++)
                    {
                        DrawLine(Ax, yy, Bx, yy, color);
                    }
                }
            }
            else
            {
                DrawLine(Ax, Ay, Bx, By, color);
                DrawLine(Bx, By, Cx, Cy, color);
                DrawLine(Cx, Cy, Dx, Dy, color);
                DrawLine(Dx, Dy, Ax, Ay, color);
            }
        }

        /// <summary>
        /// Draw circle
        /// </summary>
        /// <param name="x0">circle x center</param>
        /// <param name="y0">circle y center</param>
        /// <param name="radius">circle radius</param>
        /// <param name="color">color id</param>
        /// <param name="filled">enable/disable circle fill</param>

        public static void DrawCircle(int x0, int y0, int radius, byte color, bool filled = false)
        {
            if (radius > 0 && radius <= 2)
            {
                PutPixel(x0, y0, color);
                return;
            }

            if (radius > 2 && radius <= 4)
            {
                if (filled) PutPixel(x0, y0, color);
                PutPixel(x0 + 1, y0, color);
                PutPixel(x0 - 1, y0, color);
                PutPixel(x0, y0 + 1, color);
                PutPixel(x0, y0 - 1, color);
                return;
            }

            int x = radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (radius << 1);

            while (x >= y)
            {
                if (filled)
                {
                    DrawLine(x0 - x, y0 + y, x0 + x, y0 + y, color);
                    DrawLine(x0 - y, y0 + x, x0 + y, y0 + x, color);
                    DrawLine(x0 - x, y0 - y, x0 + x, y0 - y, color);
                    DrawLine(x0 - y, y0 - x, x0 + y, y0 - x, color);
                }
                else
                {
                    PutPixel(x0 + x, y0 + y, color);
                    PutPixel(x0 + y, y0 + x, color);
                    PutPixel(x0 - y, y0 + x, color);
                    PutPixel(x0 - x, y0 + y, color);

                    PutPixel(x0 - x, y0 - y, color);
                    PutPixel(x0 - y, y0 - x, color);
                    PutPixel(x0 + y, y0 - x, color);
                    PutPixel(x0 + x, y0 - y, color);
                }
                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }
                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += (-radius << 1) + dx;
                }
            }
        }

        /// <summary>
        /// Draw user defined color array with defined width. 0 = transparent
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="width">image width</param>
        /// <param name="pixels">color array of bytes</param>
        public static void DrawPixels(int x, int y, int width, byte[] pixels)
        {
            int px = 0;
            int py = 0;
            int height = (int)pixels.Length / width;

            for (int p = 0; p < pixels.Length; p++)
            {
                if (py >= width)
                {
                    py = 0;
                    px++;
                }

                if (pixels[p] != 0)
                {
                    //PutPixel(x + px, (height - 1 + y) + py - (height - 1), pixels[p]);
                    PutPixel(x + px, y + py, pixels[p]);
                }

                py++;
            }
        }
    }
}