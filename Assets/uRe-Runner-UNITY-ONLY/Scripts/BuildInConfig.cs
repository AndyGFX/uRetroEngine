using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public class BuildInConfig : MonoBehaviour
    {
        public string cartridgesFolder = "uRetroEngine_Game";
        public string cartridgeName = "Game";
        public TextAsset luaCode;
        private string fileLua = "<none>";
        public Texture2D fonts;
        private string fileFont = "<none>";
        public Texture2D colors;
        private string fileColors = "<none>";
        public Texture2D sprites;
        private string fileSprites = "<none>";
        public TextAsset tilemap;
        private string fileTilemap = "<none>";
        public string chars = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ";
        public int charSpacing = 8;

        // display resolution
        public int screen_width = 256;

        public int screen_height = 240;
        public bool flipScreenY = true;

        // sprite size
        public int sprite_width = 8;

        public int sprite_height = 8;

        // tilemap size
        public int tilemap_width = 32;

        public int tilemap_height = 32;
        public int tilemap_layers = 2;

        // max color in palette
        public int max_colors = 16;

        // capture to gif
        public string capture_filename = "capture";

        public int capture_framerate = 15;
        public int capture_downscale = 2;
        public int capture_time = 10;
        public bool capture_bilinear = false;
        // keys

        public KeyCode UP = KeyCode.UpArrow;
        public KeyCode DOWN = KeyCode.DownArrow;
        public KeyCode RIGHT = KeyCode.RightArrow;
        public KeyCode LEFT = KeyCode.LeftArrow;
        public KeyCode A = KeyCode.Z;
        public KeyCode B = KeyCode.X;
        public KeyCode X = KeyCode.A;
        public KeyCode Y = KeyCode.S;
        public KeyCode START = KeyCode.Return;
        public KeyCode OPTION = KeyCode.Escape;

        public void Initialize()
        {
            this.fileLua = this.luaCode.name;
            this.fileSprites = this.sprites.name;
            this.fileFont = this.fonts.name;
            this.fileTilemap = this.tilemap.name;
            this.fileColors = this.colors.name;

            uRetroConfig.cartridgesFolder = this.cartridgesFolder;
            uRetroConfig.cartridgeName = this.cartridgeName;
            uRetroConfig.fileLua = this.fileLua;
            uRetroConfig.fileFont = this.fileFont;
            uRetroConfig.fileColors = this.fileColors;
            uRetroConfig.fileSprites = this.fileSprites;
            uRetroConfig.fileTilemap = this.fileTilemap;
            uRetroConfig.chars = this.chars;
            uRetroConfig.charSpacing = this.charSpacing;
            uRetroConfig.screen_width = this.screen_width;
            uRetroConfig.screen_height = this.screen_height;
            uRetroConfig.flipScreenY = this.flipScreenY;
            uRetroConfig.sprite_width = this.sprite_width;
            uRetroConfig.sprite_height = this.sprite_height;
            uRetroConfig.tilemap_width = this.tilemap_width;
            uRetroConfig.tilemap_height = this.tilemap_height;
            uRetroConfig.tilemap_layers = this.tilemap_layers;
            uRetroConfig.max_colors = this.max_colors;
            uRetroConfig.capture_filename = this.capture_filename;
            uRetroConfig.capture_framerate = this.capture_framerate;
            uRetroConfig.capture_downscale = this.capture_downscale;
            uRetroConfig.capture_time = this.capture_time;
            uRetroConfig.capture_bilinear = this.capture_bilinear;
            uRetroConfig.UP = this.UP;
            uRetroConfig.DOWN = this.DOWN;
            uRetroConfig.RIGHT = this.RIGHT;
            uRetroConfig.LEFT = this.LEFT;
            uRetroConfig.A = this.A;
            uRetroConfig.B = this.B;
            uRetroConfig.X = this.X;
            uRetroConfig.Y = this.Y;
            uRetroConfig.START = this.START;
            uRetroConfig.OPTION = this.OPTION;
        }
    }
}