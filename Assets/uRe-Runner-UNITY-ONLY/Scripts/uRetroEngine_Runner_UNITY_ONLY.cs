﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;


namespace uRetroEngine
{
    public class uRetroEngine_Runner_UNITY_ONLY : uRetroEngineComponent
    {
        private int xx = 0;
        private int yy = 0;
        private int line = 0;

        private void Awake()
        {
            // set targetDisplay for Mouse Input
            uRetroDisplay.displayTarget = GameObject.FindGameObjectWithTag("uRetroDisplay").GetComponent<RectTransform>();

            // set code profiler canvas
            uRetroUtils.codeProfiler = GameObject.FindGameObjectWithTag("uRetroProfiler");

            // initialize screen capture to GIF
            uRetroCapture.Init();

            // prepare and set console window
	        uRetroConsole.console = GameObject.FindGameObjectWithTag("uRetroConsole");
	        
	        
	        uRetroConfig.cartridgeName = "DENO-UnityOnlyGame";
	        
	        uRetroConfig.cartridgesFolder = "uRetroEngine_Game";
	        
	        string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder+"/";
	        
            // CONFIG
	        string json = File.ReadAllText(path + "config.json");
	        uRetroSystem.DefinitionToConfig(JsonConvert.DeserializeObject<RetroDefinition>(json));
	        
            // COLORS
	        Texture2D colors = PNG.LoadPNG(path + uRetroConfig.fileColors);
	        uRetroColors.CreatePalette(colors);
	        
            // SPRITES
	        Texture2D sprites = PNG.LoadPNG(path + uRetroConfig.fileSprites);
	        uRetroSprites.CreateSprites(sprites);
	        
            // FONTS
	        Texture2D fonts = PNG.LoadPNG(path + uRetroConfig.fileFont);
	        uRetroText.CreateFont(fonts);
	        
	        uRetroText.SetFont(0,0,6);
	        uRetroText.SetFont(1,16*8,6);
	        
	        
            // Tilemaps
	        uRetroTilemap.Load(path);
	        
            // Create Display
	        uRetroDisplay.CreateDisplay(GameObject.FindGameObjectWithTag("uRetroDisplay"));
	        
            // Set Resolution
	        uRetroDisplay.SetResolution(uRetroConfig.screen_width, uRetroConfig.screen_height, 0);
	        
	        
	        
        }

        // Use this for initialization
        private void Start()
        {
            base.OnStart();
        }

        // Update is called once per frame
        private void Update()
        {
            base.OnUpdate();

            // ---------------------- TEST CODE .....

	        uRetroDisplay.Clear(3);
	        
	        uRetroText.Font(0);
	        uRetroText.Draw(10,18,"2017");
	        
	        uRetroText.Font(1);
	        uRetroText.Draw(10,10,"Standalone unity version");
	        
            uRetroDisplay.Flip();
        }
    }
}