using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uRetroEngine
{
    public class uRetroEngine_Runner_UNITY : MonoBehaviour
    {
        private int xx = 0;
        private int yy = 0;
        private int line = 0;

        private void Awake()
        {
            uRetroSystem.Init();

            // if (!uRetroSystem.CheckArguments())
            {
                uRetroSystem.LoadData("uRE-Test-TilemapImport");

                //uRetroSystem.SaveCartridge();
                //uRetroSystem.LoadCartridge("RetroGameTest");
                //uRetroSystem.ExtractCartridge();
            }
        }

        // Use this for initialization
        private void Start()
        {
            //uRetroSystem.ShowFPS(true);
            //uRetroSystem.CreateGame("aaaa");
            //uRetroTilemap.ImportPyxelEditTilemap("Py_TestTilemap.json");
            //uRetroTilemap.Save();
            //uRetroTilemap.Load();
            //uRetroSystem.SaveCartridge();
            //Test_MetaSPrite_Init();
            //Test_Tilemap_Init();
            uRetroTilemap.Clear();

            uRetroConfig.flipScreenY = true;

            uRetroGraphics.PutPixel(10, 10, 3);
            int c = uRetroGraphics.GetPixel(10, 10);
            uRetroConsole.Print(c);

            int l0 = uRetroTilemap.GetID(0, 0, 0);
            int l1 = uRetroTilemap.GetID(1, 0, 0);

            uRetroConsole.Print(l0 + "," + l1);

            uRetroTilemap.SetTile(0, 0, 0, 0);
            uRetroTilemap.SetTile(1, 1, 0, 1);

            l0 = uRetroTilemap.GetID(0, 0, 0);

            l1 = uRetroTilemap.GetID(1, 1, 0);

            uRetroConsole.Print(l0 + "," + l1);

            uRetroTilemap.SetTileLine(5, 5, 20, 20, 0, 5);
        }

        // Update is called once per frame
        private void Update()
        {
            uRetroInput.UpdateMousePosition();

            uRetroDisplay.Clear();

            if (Input.GetKeyDown(KeyCode.F1)) uRetroConsole.SwitchVisibility();
            if (Input.GetKeyDown(KeyCode.F10)) uRetroSystem.SwitchFPSVisibility();
            if (Input.GetKeyDown(KeyCode.F11)) uRetroUtils.SwitchProfiler();
            if (Input.GetKeyDown(KeyCode.F12)) uRetroConfig.flipScreenY = !uRetroConfig.flipScreenY;

            if (Input.GetKeyDown(KeyCode.Alpha1)) uRetroDisplay.PixelSize(1, 1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) uRetroDisplay.PixelSize(2, 1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) uRetroDisplay.PixelSize(1, 2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) uRetroDisplay.PixelSize(2, 2);

            /*

// row 0
	        uRetroTilemap.SetTile(0, 0, 0, 123,0,true,false);
	        uRetroTilemap.SetTile(0, 1, 0, 114,0,false,false);
	        uRetroTilemap.SetTile(0, 2, 0, 113,0,false,false);
	        uRetroTilemap.SetTile(0, 3, 0, 114,0,true,false);
	        uRetroTilemap.SetTile(0, 4, 0, 123,0,false,false);

 // row 1
	        uRetroTilemap.SetTile(0, 0, 1, 128,0,false,false);
	        uRetroTilemap.SetTile(0, 1, 1, 119,0,true,false);
	        uRetroTilemap.SetTile(0, 2, 1, 138,0,false,false);
	        uRetroTilemap.SetTile(0, 3, 1, 119,0,false,false);
	        uRetroTilemap.SetTile(0, 4, 1, 130,0,false,false);

 // row 2
	        uRetroTilemap.SetTile(0, 0, 2, 112,0,false,false);
	        uRetroTilemap.SetTile(0, 1, 2, 105,0,false,false);
	        uRetroTilemap.SetTile(0, 2, 2, 106,0,false,false);
	        uRetroTilemap.SetTile(0, 3, 2, 105,0,true,false);
	        uRetroTilemap.SetTile(0, 4, 2, 114,0,false,false);

 // row 3
	        uRetroTilemap.SetTile(0, 0, 3, 98,0,true,false);
	        uRetroTilemap.SetTile(0, 1, 3, 97,0,false,false);
	        uRetroTilemap.SetTile(0, 2, 3, 97,0,false,false);
	        uRetroTilemap.SetTile(0, 3, 3, 97,0,true,false);
	        uRetroTilemap.SetTile(0, 4, 3, 98,0,false,false);
	        */

            uRetroTilemap.DrawMap(0, 0, 32, 30, 0, 0);

            //uRetroTilemap.DrawLayer(1,0,0,5,4,48,40);
            //uRetroTilemap.DrawLayer(0,0,0,5,4,48,40);

            //uRetroTilemap.DrawMap(0, 0, 5, 4, 0,0);

            uRetroGraphics.PutPixel(0, 0, 5);
            uRetroGraphics.PutPixel(uRetroConfig.screen_width - 1, uRetroConfig.screen_height - 1, 5);

            uRetroGraphics.DrawRectangle(0, 0, uRetroConfig.screen_width - 1, uRetroConfig.screen_height - 1, 1, false);

            uRetroGraphics.DrawRectangle(16, 16, 64, 128, 8, false);
            uRetroGraphics.DrawRectangle(18, 18, 62, 126, 8, true);
            uRetroGraphics.DrawLine(8, 8, uRetroConfig.screen_width - 8 - 1, uRetroConfig.screen_height - 8 - 1, 6);
            uRetroGraphics.DrawCircle(128, 128, 32, 9);

            if (uRetroInput.ButtonHold(uRetroConfig.LEFT)) xx--;
            if (uRetroInput.ButtonHold(uRetroConfig.RIGHT)) xx++;

            if (uRetroConfig.flipScreenY)
            {
                if (uRetroInput.ButtonHold(uRetroConfig.UP)) yy--;
                if (uRetroInput.ButtonHold(uRetroConfig.DOWN)) yy++;
            }
            else
            {
                if (uRetroInput.ButtonHold(uRetroConfig.UP)) yy++;
                if (uRetroInput.ButtonHold(uRetroConfig.DOWN)) yy--;
            }

            uRetroUtils.CodeProfilerStart("Sprite");
            uRetroSprites.DrawSprite(0, xx + 16 + 0, yy);
            uRetroUtils.CodeProfilerEnd("Sprite");

            uRetroSprites.DrawSprite(1, xx + 16 + 8, yy);

            uRetroText.Draw(0, 10, " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            uRetroText.Draw(0, 30, "AndyGFX", 5);

            Test_DrawPixels_Draw();

            uRetroDisplay.Flip();
        }

        /*
        public void Test_Sprites()
        {
            if (uRetroInput.ButtonHold(uRetroConfig.LEFT)) xx--;
            if (uRetroInput.ButtonHold(uRetroConfig.RIGHT)) xx++;
            if (uRetroInput.ButtonHold(uRetroConfig.UP)) yy++;
            if (uRetroInput.ButtonHold(uRetroConfig.DOWN)) yy--;

            uRetroSprites.DrawSprite(0, xx + 0, yy);
            uRetroSprites.DrawSprite(1, xx + 8, yy);
            uRetroSprites.DrawSprite(2, xx + 16, yy);
            uRetroSprites.DrawSprite(3, xx + 24, yy);
            uRetroSprites.DrawSprite(4, xx + 32, yy);
            uRetroSprites.DrawSprite(5, xx + 40, yy);
            uRetroSprites.DrawSprite(6, xx + 0, yy - 8);
            uRetroSprites.DrawSprite(7, xx + 8, yy - 8);
            uRetroSprites.DrawSprite(8, xx + 16, yy - 8);
            uRetroSprites.DrawSprite(9, xx + 24, yy - 8);
            uRetroSprites.DrawSprite(10, xx + 32, yy - 8);
            uRetroSprites.DrawSprite(11, xx + 40, yy - 8);

            Test_Graphics();
            Test_DrawText();
        }

        public void Test_DrawText()
        {
            uRetroText.Draw(10, 10, " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
            uRetroText.Draw(10, 30, "AndyGFX", 5);
        }

        public void Test_MetaSPrite_Init()
        {
            uRetroMetaSprites.Add("Avatar", 2, 2, new[] { 0, 1, 6, 7 }, new[] { false, false, false, false }, new[] { false, false, false, false });
            uRetroMetaSprites.Add("Avatar2", 2, 2, new[] { 2, 3, 8, 9 }, new[] { false, false, false, false }, new[] { false, false, false, false });
            uRetroMetaSprites.Add("Cisla", 2, 2, new[] { 12, 13, 18, 19 }, new[] { true, false, false, false }, new[] { true, false, false, false });
            uRetroMetaSprites.Add("MouseCursor", 2, 2, new[] { 4, 5, 10, 11 }, new[] { false, false, false, false }, new[] { false, false, false, false });
        }

        public void Test_MetaSPrite_Draw()
        {
            uRetroMetaSprites.Draw("Cisla", 0, 0);
            uRetroMetaSprites.Draw("Avatar", 50, 50);
            uRetroMetaSprites.Draw("Avatar2", 82, 50);
            uRetroMetaSprites.Draw("MouseCursor", 150, 50);
        }

        public void Test_Graphics()
        {
            uRetroGraphics.DrawRectangle(32, 32, 128, 128, 6);
            uRetroGraphics.DrawRectangle(34, 34, 126, 126, 6, true);

            for (int c = 0; c < 32; c++) uRetroGraphics.PutPixel(20 + c, 20 + c, (byte)c);

            uRetroGraphics.DrawCircle(100, 100, line % 32, 20, true);
            uRetroGraphics.DrawCircle(100, 100, line % 32, 21, false);

            uRetroGraphics.DrawLine(0, line, uRetroConfig.screen_width, line, 7);
            uRetroGraphics.DrawLine(line, 0, line, uRetroConfig.screen_height, 7);
            line++;
            if (line > 256) line = 0;
        }

        public void Test_Console()
        {
            uRetroConsole.Clear();
            uRetroConsole.Print(xx.ToString());
        }
        */

        public void Test_Tilemap_Init()
        {
            //uRetroTilemap.CreateTilemap(64, 64);

            // import PyxelEdit tilemaps
            uRetroTilemap.ImportPyxelEditTilemap("Py_TestTilemap.json");

            int[] flags = new int[9];

            flags[uRetroTilemap.FLAG_FREE] = 0;
            flags[uRetroTilemap.FLAG_WALL] = 6;
            flags[uRetroTilemap.FLAG_OBSTACLE] = 0;
            flags[uRetroTilemap.FLAG_LADDER] = 7;
            flags[uRetroTilemap.FLAG_HAZARD] = 0;
            flags[uRetroTilemap.FLAG_CHEST] = 0;
            flags[uRetroTilemap.FLAG_ITEM] = 0;
            flags[uRetroTilemap.FLAG_COIN] = 0;
            flags[uRetroTilemap.FLAG_LIFE] = 0;

            // import PyxelEdit collision/flag map
            uRetroTilemap.ImportPyxelCollisionMap("Py_TestTilemapCol.json", flags, 1);

            // save uRetroTilemap
            uRetroTilemap.Save();
        }

        public void Test_DrawPixels_Draw()
        {
            byte[] pixels = {
                      0,1,1,1,1,1,1,1,1,0,
                      1,5,5,5,5,5,5,5,5,1,
                      1,0,0,0,4,4,0,0,5,1,
                      1,0,0,4,0,0,4,0,5,1,
                      1,0,0,4,0,0,4,0,5,1,
                      1,0,0,0,4,4,0,0,5,1,
                      1,0,0,0,0,0,0,0,5,1,
                      7,1,1,1,1,1,1,1,1,0
                  };

            uRetroGraphics.DrawPixels(96, 96, 10, pixels);
        }
    }
}