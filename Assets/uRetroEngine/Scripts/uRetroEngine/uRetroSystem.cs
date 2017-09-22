using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace uRetroEngine
{
    /// <summary>
    /// Game cartridge definiton
    /// </summary>
    public struct RetroCartridge
    {
        public string definition;
        public string[] palette;
        public uRetroImage[] sprites;
        public uRetroImage[] characters;
        public List<TilemapLayer> tilemap;
        public string luaCode;
        public List<SrcInclude> include;
    }

    /// <summary>
    /// serialized config definition
    /// </summary>
    public struct RetroDefinition
    {
        public string cartridgesFolder;
        public string cartridgeName;
        public string fileFont;
        public string fileColors;
        public string fileSprites;
        public string fileTilemap;
        public string chars;
        public int charSpacing;

        // display resolution
        public int screen_width;

        public int screen_height;

        public bool flipScreenY;

        // sprite size
        public int sprite_width;

        public int sprite_height;

        // tilemap size
        public int tilemap_width;

        public int tilemap_height;
        public int tilemap_layers;

        // max color in palette
        public int max_colors;

        // capture to gif
        public string capture_filename;

        public int capture_framerate;
        public int capture_downscale;
        public int capture_time;
        public bool capture_bilinear;
        // Input

        public KeyCode KeyCode_UP;
        public KeyCode KeyCode_DOWN;
        public KeyCode KeyCode_RIGHT;
        public KeyCode KeyCode_LEFT;
        public KeyCode KeyCode_A;
        public KeyCode KeyCode_B;
        public KeyCode KeyCode_X;
        public KeyCode KeyCode_Y;
        public KeyCode KeyCode_START;
        public KeyCode KeyCode_OPTION;
    }

    /// <summary>
    /// System commands for uRetroEngine
    /// </summary>
    public static class uRetroSystem
    {
        /// <summary>
        /// Return path where is BINARY build located
        /// </summary>
        /// <returns></returns>
        public static string GetRoot()
        {
            return Path.GetDirectoryName(Application.dataPath);
        }

        /// <summary>
        /// Initialize system from scene components
        /// </summary>
        public static void Init()
        {
            // set targetDisplay for Mouse Input
            uRetroDisplay.displayTarget = GameObject.FindGameObjectWithTag("uRetroDisplay").GetComponent<RectTransform>();

            // set code profiler canvas
            uRetroUtils.codeProfiler = GameObject.FindGameObjectWithTag("uRetroProfiler");

            // initialize screen capture to GIF
            uRetroCapture.Init();

            // prepare and set console window
            uRetroConsole.console = GameObject.FindGameObjectWithTag("uRetroConsole");

            if (!Directory.Exists(GetRoot() + "/uRetroEngine_Libs"))
            {
                Directory.CreateDirectory(GetRoot() + "/uRetroEngine_Libs");
            }
        }

        /// <summary>
        /// Load JSON game definition and all files for RUN
        /// </summary>
        /// <param name="cartridgeName"></param>
        /// <returns></returns>
        public static bool LoadData(string cartridgeName)
        {
            uRetroConfig.cartridgeName = cartridgeName;

            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName;

            if (!Directory.Exists(path))
            {
                uRetroConsole.PrintError("Cartridge folder with name '" + cartridgeName + "' missing!");
                return false;
            }

            path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/";

            // CONFIG
            uRetroSystem.LoadRetroEngineConfig();

            // COLORS
            Texture2D colors = PNG.LoadPNG(path + uRetroConfig.fileColors);
            uRetroColors.CreatePalette(colors);

            // SPRITES
            Texture2D sprites = PNG.LoadPNG(path + uRetroConfig.fileSprites);
            uRetroSprites.CreateSprites(sprites);

            // FONTS
            Texture2D fonts = PNG.LoadPNG(path + uRetroConfig.fileFont);
            uRetroText.CreateFont(fonts);

            // Tilemaps
            uRetroTilemap.Load();

            // Create Display
            uRetroDisplay.CreateDisplay(GameObject.FindGameObjectWithTag("uRetroDisplay"));

            // Set Resolution
            uRetroDisplay.SetResolution(uRetroConfig.screen_width, uRetroConfig.screen_height, 0);

            uRetroLua.fromCart = false;

            return true;
        }

        public static RetroDefinition ConfigToDefinition()
        {
            RetroDefinition definition;

            definition.cartridgesFolder = uRetroConfig.cartridgesFolder;
            definition.cartridgeName = uRetroConfig.cartridgeName;
            definition.fileFont = uRetroConfig.fileFont;
            definition.fileColors = uRetroConfig.fileColors;
            definition.fileSprites = uRetroConfig.fileSprites;
            definition.fileTilemap = uRetroConfig.fileTilemap;
            definition.chars = uRetroConfig.chars;
            definition.charSpacing = uRetroConfig.charSpacing;
            definition.max_colors = uRetroConfig.max_colors;
            definition.screen_height = uRetroConfig.screen_height;
            definition.screen_width = uRetroConfig.screen_width;
            definition.flipScreenY = uRetroConfig.flipScreenY;
            definition.sprite_height = uRetroConfig.sprite_height;
            definition.sprite_width = uRetroConfig.sprite_width;
            definition.tilemap_height = uRetroConfig.tilemap_height;
            definition.tilemap_width = uRetroConfig.tilemap_width;
            definition.tilemap_layers = uRetroConfig.tilemap_layers;

            // capture to gif
            definition.capture_filename = uRetroConfig.capture_filename;
            definition.capture_framerate = uRetroConfig.capture_framerate;
            definition.capture_downscale = uRetroConfig.capture_downscale;
            definition.capture_time = uRetroConfig.capture_time;
            definition.capture_bilinear = uRetroConfig.capture_bilinear;

            definition.KeyCode_UP = uRetroConfig.UP;
            definition.KeyCode_DOWN = uRetroConfig.DOWN;
            definition.KeyCode_RIGHT = uRetroConfig.RIGHT;
            definition.KeyCode_LEFT = uRetroConfig.LEFT;
            definition.KeyCode_A = uRetroConfig.A;
            definition.KeyCode_B = uRetroConfig.B;
            definition.KeyCode_X = uRetroConfig.X;
            definition.KeyCode_Y = uRetroConfig.Y;
            definition.KeyCode_START = uRetroConfig.START;
            definition.KeyCode_OPTION = uRetroConfig.OPTION;

            return definition;
        }

        public static void DefinitionToConfig(RetroDefinition definition)
        {
            uRetroConfig.cartridgesFolder = definition.cartridgesFolder;
            uRetroConfig.cartridgeName = definition.cartridgeName;
            uRetroConfig.fileFont = definition.fileFont;
            uRetroConfig.fileColors = definition.fileColors;
            uRetroConfig.fileSprites = definition.fileSprites;
            uRetroConfig.fileTilemap = definition.fileTilemap;

            uRetroConfig.chars = definition.chars;
            uRetroConfig.charSpacing = definition.charSpacing;
            uRetroConfig.max_colors = definition.max_colors;
            uRetroConfig.screen_height = definition.screen_height;
            uRetroConfig.screen_width = definition.screen_width;
            uRetroConfig.flipScreenY = definition.flipScreenY;
            uRetroConfig.sprite_height = definition.sprite_height;
            uRetroConfig.sprite_width = definition.sprite_width;
            uRetroConfig.tilemap_height = definition.tilemap_height;
            uRetroConfig.tilemap_width = definition.tilemap_width;
            uRetroConfig.tilemap_layers = definition.tilemap_layers;

            // capture to gif
            uRetroConfig.capture_filename = definition.capture_filename;
            uRetroConfig.capture_framerate = definition.capture_framerate;
            uRetroConfig.capture_downscale = definition.capture_downscale;
            uRetroConfig.capture_time = definition.capture_time;
            uRetroConfig.capture_bilinear = definition.capture_bilinear;

            uRetroConfig.UP = definition.KeyCode_UP;
            uRetroConfig.DOWN = definition.KeyCode_DOWN;
            uRetroConfig.RIGHT = definition.KeyCode_RIGHT;
            uRetroConfig.LEFT = definition.KeyCode_LEFT;
            uRetroConfig.A = definition.KeyCode_A;
            uRetroConfig.B = definition.KeyCode_B;
            uRetroConfig.X = definition.KeyCode_X;
            uRetroConfig.Y = definition.KeyCode_Y;
            uRetroConfig.START = definition.KeyCode_START;
            uRetroConfig.OPTION = definition.KeyCode_OPTION;
        }

        /// <summary>
        /// Save config to game folder in catridge mode
        /// </summary>
        public static void SaveRetroEngineConfig()
        {
            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + "config.json";

            SaveRetroEngineConfig(path);
        }

        /// <summary>
        /// Save config to user data folder
        /// </summary>
        /// <param name="path"></param>
        public static void SaveRetroEngineConfig(string path)
        {
            string json = JsonConvert.SerializeObject(ConfigToDefinition(), Formatting.Indented);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Load config from game folder in catridge mode
        /// </summary>
        public static void LoadRetroEngineConfig()
        {
            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + "config.json";
            LoadRetroEngineConfig(path);
        }

        /// <summary>
        /// Load config from user defined game folder
        /// </summary>
        public static void LoadRetroEngineConfig(string path)
        {
            string json = File.ReadAllText(path);
            DefinitionToConfig(JsonConvert.DeserializeObject<RetroDefinition>(json));
        }

        /// <summary>
        /// Create game in catridge folder with included folder = game name, with files from template
        /// </summary>
        /// <param name="name"></param>
        public static void CreateGame(string name)
        {
            uRetroConfig.cartridgeName = name;

            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/";

            Texture2D spr = Resources.Load("TemplateData/uRE_Sprites") as Texture2D;

            spr.filterMode = FilterMode.Point;
            spr.Apply();
            PNG.Save(path + uRetroConfig.fileSprites, spr);

            Texture2D pal = Resources.Load("TemplateData/uRE_Colors") as Texture2D;

            pal.filterMode = FilterMode.Point;
            pal.Apply();
            PNG.Save(path + uRetroConfig.fileColors, pal);

            Texture2D fnt = Resources.Load("TemplateData/uRE_Fonts") as Texture2D;

            fnt.filterMode = FilterMode.Point;
            fnt.Apply();
            PNG.Save(path + uRetroConfig.fileFont, fnt);

            string code =
            @"

function OnStart()
end

function OnUpdate(deltaTime)
end

function OnScanline(line)

end

function OnClose()
end

		        ";

            File.WriteAllText(path + "main.lua", code);

            string tilemap = JsonConvert.SerializeObject(uRetroTilemap.layers, Formatting.Indented);
            Debug.Log(path + uRetroConfig.fileTilemap);
            File.WriteAllText(path + uRetroConfig.fileTilemap, tilemap);

            SaveRetroEngineConfig();

            uRetroConsole.Show();
            uRetroConsole.Print("Game folder with name '" + name + "' was created.");
            uRetroConsole.Print("Press [Alt+F4] for exit");
        }

        #region Cartridge

        public static void SaveCartridge()
        {
            RetroCartridge card;

            card.definition = JsonUtility.ToJson(ConfigToDefinition());
            card.palette = uRetroColors.GetColorsAsHexArray();
            card.sprites = uRetroSprites.GetSpritesAsArray();
            card.characters = uRetroText.GetCharsAsArray();
            card.tilemap = uRetroTilemap.layers;
            card.include = uRetroLuaLibrary.include;

            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + uRetroConfig.fileLua;
            card.luaCode = File.ReadAllText(path);

            string json = JsonConvert.SerializeObject(card, Formatting.Indented);

            path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + ".ure";
            File.WriteAllText(path, json);

            uRetroConsole.Print("Cartridge with name '" + uRetroConfig.cartridgeName + "' saved");
        }

        public static void LoadCartridge(string name)
        {
            RetroCartridge card;

            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + name + ".ure";
            string json = File.ReadAllText(path);

            card = JsonConvert.DeserializeObject<RetroCartridge>(json);

            // Config
            DefinitionToConfig(JsonConvert.DeserializeObject<RetroDefinition>(card.definition));

            // colors
            uRetroColors.CreateFromHex(card.palette);

            // SPRITES
            uRetroSprites.CreateFromRetroImageList(card.sprites);

            // FONTS
            uRetroText.CreateFromRetroImageList(card.characters);

            // Tilemaps
            uRetroTilemap.layers = card.tilemap;

            // Sounds

            // lua code
            uRetroLua.code = card.luaCode;

            // lua included src
            uRetroLuaLibrary.include = card.include;

            // Create Display
            uRetroDisplay.CreateDisplay(GameObject.FindGameObjectWithTag("uRetroDisplay"));

            // Set Resolution
            uRetroDisplay.SetResolution(uRetroConfig.screen_width, uRetroConfig.screen_height, 0);

            uRetroLua.fromCart = true;
            uRetroLua.isLoaded = true;

            Debug.Log("Loaded from cartridge ...");
        }

        /// <summary>
        /// Extract cartidge data to game folder
        /// </summary>
        public static void ExtractCartridge()
        {
            // FOLDER
            string path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = path + "/";
            // CONFIG
            SaveRetroEngineConfig();

            // COLORS
            PNG.Save(path + uRetroConfig.fileColors, uRetroColors.GetAsImage());

            // SPRITES
            PNG.Save(path + uRetroConfig.fileSprites, uRetroSprites.GetAsImage());

            // FONTS
            PNG.Save(path + uRetroConfig.fileFont, uRetroText.GetAsImage());

            // TILEMAPS
            uRetroTilemap.Save();

            // CODE
            File.WriteAllText(path + uRetroConfig.fileLua, uRetroLua.code);

            // INCLUDE

            for (int i = 0; i < uRetroLuaLibrary.include.Count; i++)
            {
                File.WriteAllText(path + uRetroLuaLibrary.include[i].name, uRetroLuaLibrary.include[i].code);
            }

            uRetroConsole.Print("Cartridge extracted to folder: " + uRetroConfig.cartridgeName);
        }

        #endregion Cartridge

        /// <summary>
        /// Show GPU/CPU profiler with FPS
        /// </summary>
        /// <param name="state"></param>
        public static void ShowFPS(bool state)
        {
            Camera.main.GetComponent<DentedPixelPerformance.FPSGraphC>().enabled = state;
        }

        /// <summary>
        /// Switch profiler visibility
        /// </summary>
        public static void SwitchFPSVisibility()
        {
            Camera.main.GetComponent<DentedPixelPerformance.FPSGraphC>().enabled = !Camera.main.GetComponent<DentedPixelPerformance.FPSGraphC>().enabled;
        }

        /// <summary>
        /// Check argumets and start engine by defined values
        ///
        /// Example #1: uRetroEngine.exe -game LuaGame -resolution 1280 800 false
        ///    load folder LuaGame and set resulution
        ///
        /// Example #2: uRetroEngine.exe -cartridge LuaGame -resolution 1280 800 false
        ///    load cartridge and run game in defined resolution
        ///
        /// Example #3: uRetroEngine.exe -create LuaGame
        ///    create cartridge folder template
        /// </summary>
        ///
        /// <returns>false=no arguments</returns>
        public static bool CheckArguments()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length == 1) return false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-game":
                        uRetroSystem.LoadData(args[i + 1]);
                        break;

                    case "-resolution":
                        Screen.SetResolution(int.Parse(args[i + 1]), int.Parse(args[i + 2]), bool.Parse(args[i + 3]));
                        break;

                    case "-cartridge":
                        uRetroSystem.LoadCartridge(args[i + 1]);
                        break;

                    case "-create":
                        uRetroSystem.CreateGame(args[i + 1]);
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Reset game cartridge
        /// </summary>
        public static void ResetGame()
        {
            uRetroSystem.LoadData(uRetroConfig.cartridgeName);
            uRetroConsole.Print("Loading data ...");
            uRetroLua.Load();
            uRetroConsole.Print("Loading main.lua ...");
            uRetroLua.Run();
            uRetroConsole.Print("Executed.");
        }

        /// <summary>
        /// Load game extracted in catridge folder under game name folder
        /// </summary>
        /// <param name="cartridgeName"></param>
        public static void LoadGame(string cartridgeName)
        {
            bool state = uRetroSystem.LoadData(cartridgeName);
            if (state)
            {
                uRetroConsole.Print("Loading '" + cartridgeName + "' data ...");
                uRetroLua.Load();
                uRetroConsole.Print("Loading main.lua ...");

                uRetroConsole.Print("... game loaded.");
            }
        }

        /// <summary>
        /// Run loaded game (main.lua file is executed)
        /// </summary>
        public static void RunGame()
        {
            if (uRetroLua.isLoaded)
            {
                uRetroLua.Run();
                uRetroConsole.Print("Execute game.");
            }
        }

        /// <summary>
        /// Backup current cartridge to persistent data path
        /// </summary>
        /// <param name="version">user defined version id, attached to end of cartridge filename</param>
        public static void BackupCartridge(int version)
        {
            string path = Application.persistentDataPath + "/" + uRetroConfig.cartridgesFolder;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string verStr = string.Format("{0:000}", version);

            RetroCartridge card;

            card.definition = JsonUtility.ToJson(ConfigToDefinition());
            card.palette = uRetroColors.GetColorsAsHexArray();
            card.sprites = uRetroSprites.GetSpritesAsArray();
            card.characters = uRetroText.GetCharsAsArray();
            card.tilemap = uRetroTilemap.layers;
            card.include = uRetroLuaLibrary.include;

            path = GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + uRetroConfig.fileLua;
            card.luaCode = File.ReadAllText(path);

            string json = JsonConvert.SerializeObject(card, Formatting.Indented);

            path = Application.persistentDataPath + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + ".ure." + verStr;
            File.WriteAllText(path, json);

            uRetroConsole.Print("Cartridge with name '" + uRetroConfig.cartridgeName + "." + verStr + "' saved to " + path);
        }

        /// <summary>
        /// Restore current cartridge to version from persistent data path
        /// </summary>
        /// <param name="version">user defined version id, attached to end of cartridge filename</param>
        public static void RestoreCartridge(int version)
        {
            RetroCartridge card;
            string verStr = string.Format("{0:000}", version);
            string path = Application.persistentDataPath + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + ".ure." + verStr;
            string json = File.ReadAllText(path);

            card = JsonConvert.DeserializeObject<RetroCartridge>(json);

            // Config
            DefinitionToConfig(JsonConvert.DeserializeObject<RetroDefinition>(card.definition));

            // colors
            uRetroColors.CreateFromHex(card.palette);

            // SPRITES
            uRetroSprites.CreateFromRetroImageList(card.sprites);

            // FONTS
            uRetroText.CreateFromRetroImageList(card.characters);

            // Tilemaps
            uRetroTilemap.layers = card.tilemap;

            // lua code
            uRetroLua.code = card.luaCode;

            // lua included src
            uRetroLuaLibrary.include = card.include;

            // Create Display
            uRetroDisplay.CreateDisplay(GameObject.FindGameObjectWithTag("uRetroDisplay"));

            // Set Resolution
            uRetroDisplay.SetResolution(uRetroConfig.screen_width, uRetroConfig.screen_height, 0);
        }
    }
}