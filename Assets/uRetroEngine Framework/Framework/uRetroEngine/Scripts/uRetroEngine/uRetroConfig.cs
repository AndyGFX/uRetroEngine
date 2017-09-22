using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    /// <summary>
    /// uRetroEngine configuration
    /// </summary>
    [System.Serializable]
    public class uRetroConfig
    {
        public static string cartridgesFolder = "uRetroEngine_Cards";
        public static string cartridgeName = "Game";
        public static string fileLua = "main.lua";
        public static string fileFont = "uRE_Font.png";
        public static string fileColors = "uRE_Colors.png";
        public static string fileSprites = "uRE_Sprites.png";
        public static string fileTilemap = "uRE_Tilemap.json";
        public static string chars = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ";
        public static int charSpacing = 8;

        // display resolution
        public static int screen_width = 256;

        public static int screen_height = 240;
        public static bool flipScreenY = true;

        // sprite size
        public static int sprite_width = 8;

        public static int sprite_height = 8;

        // tilemap size
        public static int tilemap_width = 32;

        public static int tilemap_height = 32;
        public static int tilemap_layers = 2;

        // max color in palette
        public static int max_colors = 16;

        // capture to gif
        public static string capture_filename = "capture";

        public static int capture_framerate = 15;
        public static int capture_downscale = 2;
        public static int capture_time = 10;
        public static bool capture_bilinear = false;
        // keys

        public static KeyCode UP = KeyCode.UpArrow;
        public static KeyCode DOWN = KeyCode.DownArrow;
        public static KeyCode RIGHT = KeyCode.RightArrow;
        public static KeyCode LEFT = KeyCode.LeftArrow;
        public static KeyCode A = KeyCode.Z;
        public static KeyCode B = KeyCode.X;
        public static KeyCode X = KeyCode.A;
        public static KeyCode Y = KeyCode.S;
        public static KeyCode START = KeyCode.Return;
        public static KeyCode OPTION = KeyCode.Escape;
    }
}