using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NLUA

using NLua;

#else

using MoonSharp.Interpreter;

#endif

namespace uRetroEngine
{
    // ******************************************************************
    // uRetroEngine: LIBRARY
    // ******************************************************************

    #region CAPTURE

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Capture
    {
        public void Start()
        {
            uRetroCapture.Start();
        }

        public void Setup(string filename, int framerate, int downscale, int time, bool bilinear)
        {
            uRetroCapture.Setup(filename, framerate, downscale, time, bilinear);
        }
    }

    #endregion CAPTURE

    #region LIBRARY

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Library
    {
        public void Require(string filename)
        {
            uRetroLuaLibrary.Require(filename);
        }

        public void Include(string filename)
        {
            uRetroLuaLibrary.Include(filename);
        }
    }

    #endregion LIBRARY

    #region DISPLAY

    // ******************************************************************
    // uRetroEngine: DISPLAY
    // ******************************************************************

    /// <summary>
    /// Display LUA method
    /// </summary>
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Display
    {
        /// <summary>
        /// [Display:TargetFrameRate] set and lock FPS to defined value
        /// </summary>
        /// <param name="fps"></param>
        public void TargetFrameRate(int fps)
        {
            Application.targetFrameRate = fps;
        }

        /// <summary>
        /// [Display:Width] Return screen width
        /// </summary>
        /// <returns></returns>
        public int Width()
        {
            return uRetroConfig.screen_width;
        }

        /// <summary>
        ///[Display:Height]  return screen height
        /// </summary>
        /// <returns></returns>
        public int Height()
        {
            return uRetroConfig.screen_height;
        }

        /// <summary>
        /// [Display:Flip]  Flip backbuffer to VRAM
        /// </summary>
        public void Flip()
        {
            uRetroDisplay.Flip();
        }

        /// <summary>
        /// [Display:Clear] Clear screen with defined color id
        /// </summary>
        /// <param name="color"></param>
        public void Clear(byte color = 0)
        {
            uRetroDisplay.Clear(color);
        }

        /// <summary>
        /// [Display:Clip] Reset defined clip rectangle to screen size
        /// </summary>
        public void Clip()
        {
            uRetroDisplay.Clip();
        }

        /// <summary>
        /// [Display:Clip] Set clipping screen size
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Clip(int x, int y, int width, int height)
        {
            uRetroDisplay.Clip(x, y, width, height);
        }

        /// <summary>
        /// [Display:PixelSize] Set pixel size
        /// </summary>
        /// <param name="size"> set pixel size (1,2,4) </param>
	    public void PixelSize(int px, int py)
        {
            uRetroDisplay.PixelSize(px, py);
        }

        public void CallScanline(bool state)
        {
            uRetroDisplay.callScanline = state;
        }
    }

    #endregion DISPLAY

    #region SPRITES

    // ******************************************************************
    // uRetroEngine: SPRITES
    // ******************************************************************
    /// <summary>
    /// Sprites LUA methods
    /// </summary>
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Sprite
    {
        /// <summary>
        /// [Sprites:DrawSprite] Draw sprite ID at [x,y]
        /// </summary>
        /// <param name="id">sprite id</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="flipX">flip sprite in X</param>
        /// <param name="flipY">flip sprite in Y</param>
        public void DrawSprite(int id, int x, int y, bool flipX, bool flipY)
        {
            uRetroSprites.DrawSprite(id, x, y, flipX, flipY);
        }

        /// <summary>
        /// [Sprites:SetPixel] Set pixel in sprite at [x,y] to color id
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colorID"></param>
        public void SetPixel(int ID, int x, int y, byte colorID)
        {
            uRetroSprites.SetPixel(ID, x, y, colorID);
        }

        /// <summary>
        /// [Sprites:GetPixel] Get pixel color in sprite at [x,y]
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colorID"></param>
        /// <returns></returns>
        public byte GetPixel(int ID, int x, int y, byte colorID)
        {
            return uRetroSprites.GetPixel(ID, x, y);
        }

        /// <summary>
        /// Return sprite color id as 1D array
        /// </summary>
        /// <param name="ID">sprite id</param>
        /// <returns></returns>
        public byte[] GetPixels(int ID)
        {
            return uRetroSprites.GetPixels(ID);
        }

        /// <summary>
        /// [Sprites:Store] Store pixels data of sprite table
        /// </summary>
        /// <param name="id">sprite id</param>
        public void Store(int id)
        {
            uRetroSprites.Store(id);
        }

        /// <summary>
        /// [Sprites:Restore] restore pixels data back to sprite table
        /// </summary>
        /// <param name="id"></param>
        public void Restore(int id)
        {
            uRetroSprites.Restore(id);
        }

        /// <summary>
        /// [Sprites:FlipSprite] Flip sprite in sprite table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fX"></param>
        /// <param name="fY"></param>
        public void FlipSprite(int id, bool fX, bool fY)
        {
            uRetroSprites.FlipSprite(id, fX, fY);
        }
    }

    #endregion SPRITES

    #region META_SPRITES

    // ******************************************************************
    // uRetroEngine: METASPRITES
    // ******************************************************************

    /// <summary>
    /// MetaSPrites LUA methods
    /// </summary>
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_MetaSprites
    {
        /// <summary>
        /// Add meta sprite grid
        /// </summary>
        /// <param name="name">sprite name</param>
        /// <param name="gridWidth">sprite grid width</param>
        /// <param name="gridHeight">sprite grid height</param>
        /// <param name="ids">sprite id list (zero is bottom left in grid)</param>
        /// <param name="flipX">flip X flag array</param>
        /// <param name="flipY">flip Y flag array</param>
        public void Add(string name, int gridWidth, int gridHeight, int[] ids, bool[] flipX, bool[] flipY)
        {
            uRetroMetaSprites.Add(name, gridWidth, gridHeight, ids, flipX, flipY);
        }

        /// <summary>
        /// Draw MetaSprite at X,Y defiend by name
        /// </summary>
        /// <param name="name">metasprite name in list</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="transparent">enable or disable transparent color</param>
        public void Draw(string name, int x, int y, bool transparent = true)
        {
            uRetroMetaSprites.Draw(name, x, y, transparent);
        }

        /// <summary>
        /// Store metasprite
        /// </summary>
        /// <param name="name"></param>
        public void Store(string name)
        {
            uRetroMetaSprites.Store(name);
        }

        /// <summary>
        /// Restore metasprite
        /// </summary>
        /// <param name="name"></param>
        public void Restore(string name)
        {
            uRetroMetaSprites.Restore(name);
        }
    }

    #endregion META_SPRITES

    #region GRAPHICS

    // ******************************************************************
    // uRetroEngine: GRAPHICS
    // ******************************************************************
    /// <summary>
    /// [Graphics: ... ] LUA methods
    /// </summary>

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Graphics
    {
        /// <summary>
        /// [Graphics:PutPixel] - Put pixel to screen
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void PutPixel(int x, int y, byte color)
        {
            uRetroGraphics.PutPixel(x, y, color);
        }

        /// <summary>
        /// [Graphics:GetPixel] - Get color id from screen at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public byte GetPixel(int x, int y)
        {
            return uRetroGraphics.GetPixel(x, y);
        }

        public void DrawPixels(int x, int y, int width, byte[] pixels)
        {
            uRetroGraphics.DrawPixels(x, y, width, pixels);
        }

        /// <summary>
        /// [Graphics:DrawLine] - Draw line to screen in defined color
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void DrawLine(int x1, int y1, int x2, int y2, byte color)
        {
            uRetroGraphics.DrawLine(x1, y1, x2, y2, color);
        }

        /// <summary>
        /// [Graphics:DrawCircle] - Draw circle
        /// </summary>
        /// <param name="x0">x center</param>
        /// <param name="y0">y center</param>
        /// <param name="radius">circle roadius</param>
        /// <param name="color">color</param>
        /// <param name="filled">enable/disable fill</param>
        public void DrawCircle(int x0, int y0, int radius, int color, bool filled = false)
        {
            uRetroGraphics.DrawCircle(x0, y0, radius, (byte)color, filled);
        }

        /// <summary>
        /// [Graphics:DrawRectangle] - Draw rectangle
        /// </summary>
        /// <param name="x0">from x</param>
        /// <param name="y0">from y</param>
        /// <param name="x1">to x</param>
        /// <param name="y1">to y</param>
        /// <param name="color">color</param>
        /// <param name="filled">enable/disable fill</param>
        public void DrawRectangle(int x0, int y0, int x1, int y1, byte color, bool filled)
        {
            uRetroGraphics.DrawRectangle(x0, y0, x1, y1, color, filled);
        }
    }

    #endregion GRAPHICS

    #region TEXT

    // ******************************************************************
    // uRetroEngine: TEXT
    // ******************************************************************
    /// <summary>
    /// [Text: ... ] Text LUA methods
    /// </summary>

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Text
    {
        /// <summary>
        /// Text:Draw - draw text at x,y
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="text">printed text</param>
        public void Draw(int x, int y, string text)
        {
            uRetroText.Draw(x, y, text);
        }

        /// <summary>
        /// Text:Draw - draw text at x,y
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="text">printed text</param>
        /// <param name="fontColor">font color</param>
        /// <param name="backgroundColor">background color (default transparent)</param>
        public void Draw(int x, int y, string text, byte fontColor, int backgroundColor = -1)
        {
            uRetroText.Draw(x, y, text, fontColor, backgroundColor);
        }

        public void SetFont(int fontID, int fontCharStart, int fontSpacing)
        {
            uRetroText.SetFont(fontID, fontCharStart, fontSpacing);
        }

        public void Font(int id)
        {
            uRetroText.Font(id);
        }
    }

    #endregion TEXT

    #region SOUND_FX

    // ******************************************************************
    // uRetroEngine: SOUND FX
    // ******************************************************************
    /// <summary>
    /// [Sound: ... ] SoundFX lua methods
    /// http://www.bfxr.net/
    /// </summary>
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Sound
    {
        /// <summary>
        /// [Sound:Add] add sounf fx definition to SFX list
        /// </summary>
        /// <param name="name">sfx name in list</param>
        /// <param name="definition"></param>
        /// <param name="cache">add sfx to cache after initialization</param>
        public void Add(string name, string definition, bool cache)
        {
            uRetroSound.Add(name, definition, cache);
        }

        /// <summary>
        /// [Sound:Remove] remove sfx from list
        /// </summary>
        /// <param name="name">sfx name in list</param>
        public void Remove(string name)
        {
            uRetroSound.Remove(name);
        }

        /// <summary>
        /// [Sound:Play] play sfx
        /// </summary>
        /// <param name="name">sfx name in list</param>
        public void Play(string name)
        {
            uRetroSound.Play(name);
        }

        /// <summary>
        /// [Sound:Stop] stop play sfx
        /// </summary>
        /// <param name="name">sfx name in list</param>
        public void Stop(string name)
        {
            uRetroSound.Stop(name);
        }
    }

    #endregion SOUND_FX

    #region COLOR

    // ******************************************************************
    // uRetroEngine: COLOR
    // ******************************************************************
    /// <summary>
    /// [Colors: ... ] Colors lua methods
    /// </summary>
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Color
    {
        /// <summary>
        /// [Colors:Set] set color ID by RGBA valyes
        /// </summary>
        /// <param name="ID">idx in color palette</param>
        /// <param name="r">red component</param>
        /// <param name="g">green component</param>
        /// <param name="b">blue component</param>
        /// <param name="a">alpha component</param>
        public void Set(byte ID, byte r, byte g, byte b, byte a)
        {
            uRetroColors.Set(ID, r, g, b, a);
        }

        /// <summary>
        /// [Colors:Get] get color from palette at idx ID
        /// </summary>
        /// <param name="ID">position in palette</param>
        /// <returns>unity color type is returned</returns>
        public Color32 Get(byte ID)
        {
            return uRetroColors.Get(ID);
        }

        /// <summary>
        /// [Colors:GetAsHex] get color from palette at idx ID as HEX string
        /// </summary>
        /// <param name="ID">position in palette</param>
        /// <returns>retrun color in format #ffrrggbb</returns>
        public string GetAsHex(byte ID)
        {
            return uRetroColors.GetAsHex(ID);
        }

        public void Restore()
        {
            uRetroColors.Restore();
        }

        /// <summary>
        /// [Colors:SetFromHex] set color in palette at ID
        /// </summary>
        /// <param name="id">position in palette</param>
        /// <param name="hexColor">color defined in HEX ( #ffrrggbb/ffrrggbb)</param>
        public void SetFromHex(int id, string hexColor)
        {
            uRetroColors.SetFromHex(id, hexColor);
        }
    }

    #endregion COLOR

    #region SYSTEM

    // ******************************************************************
    // uRetroEngine: SYSTEM
    // ******************************************************************
    /// <summary>
    /// [System: ... ] system lua methods
    /// </summary>
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_System
    {
        /// <summary>
        /// [System:ShowFPS] show FPS and redering performance graph CPU/GPU
        /// </summary>
        /// <param name="state"></param>
        public void ShowFPS(bool state)
        {
            uRetroSystem.ShowFPS(state);
        }

        /// <summary>
        /// [System:TimeSinceStart ] set framerate limit
        /// </summary>
        /// <returns></returns>
        public float TimeSinceStart()
        {
            return Time.realtimeSinceStartup;
        }

        public string RootPath()
        {
            return uRetroSystem.GetRoot();
        }
    }

    #endregion SYSTEM

    #region TILEMAP

    // ******************************************************************
    // uRetroEngine: TILEMAP
    // ******************************************************************
    /// <summary>
    /// [Tilemap: ... ] Tilemap lua methods
    /// </summary>

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Tilemap
    {
        /// <summary>
        /// Clear tilemap
        /// </summary>
        public void Clear()
        {
            uRetroTilemap.Clear();
        }

        /// <summary>
        /// [Tilemap:] Return tilemap width in cells count
        /// </summary>
        /// <returns></returns>
        public int Width()
        {
            return uRetroConfig.tilemap_width;
        }

        /// <summary>
        /// [Tilemap:] return tilemap height in cells count
        /// </summary>
        /// <returns></returns>
        public int Height()
        {
            return uRetroConfig.tilemap_height;
        }

        /// <summary>
        /// [Tilemap:] Return layers count in tilemap
        /// </summary>
        /// <returns></returns>
        public int LayersCount()
        {
            return uRetroConfig.tilemap_layers;
        }

        /// <summary>
        /// [Tilemap:] Create tilemat with defined size
        /// </summary>
        /// <param name="w">colums count</param>
        /// <param name="h">rows count</param>
        public void Create(int w, int h)
        {
            uRetroTilemap.CreateTilemap(w, h);
        }

        /// <summary>
        /// [Tilemap:] Set tile and tile properties in map
        /// </summary>
        /// <param name="layer">target tilemap layer (default count = 2)</param>
        /// <param name="x">x tilemap cell position</param>
        /// <param name="y">y tilemap cell position</param>
        /// <param name="spriteID">sprite ID</param>
        /// <param name="flag">user defined value as flag 0..255</param>
        /// <param name="flipX">flip tile horizontaly</param>
        /// <param name="flipY">flip tile verticaly</param>
        public void SetTile(int layer, int x, int y, int spriteID, byte flag = 0, bool flipX = false, bool flipY = false)
        {
            uRetroTilemap.SetTile(layer, x, y, spriteID, flag = 0, flipX, flipY);
        }

        public void SetTileLine(int x1, int y1, int x2, int y2, int layer, int spriteID, byte flag = 0, bool flipX = false, bool flipY = false)
        {
            uRetroTilemap.SetTileLine(x1, y1, x2, y2, layer, spriteID, flag, flipX, flipY);
        }

        /// <summary>
        /// [Tilemap:] Draw tilemap with all layers at once
        /// </summary>
        /// <param name="_tx">x cell postion</param>
        /// <param name="_ty">y cell postion</param>
        /// <param name="_tw">width cels count </param>
        /// <param name="_th">height cels count</param>
        /// <param name="_sx">at screen x position</param>
        /// <param name="_sy">at screen y position</param>
        /// <param name="_maskColor">mask color ID (unuported now, ID with alpha=0 is used)</param>
        /// <param name="_scale">unsused, insted use Display:PixelSize(<1/2/4>)</param>
        public void DrawTilemap(int _tx, int _ty, int _tw, int _th, int _sx, int _sy, byte _maskColor = 0, int _scale = 1)
        {
            uRetroTilemap.DrawMap(_tx, _ty, _tw, _th, _sx, _sy, _maskColor, _scale);
        }

        /// <summary>
        /// [Tilemap:] Draw tilemap layer
        /// </summary>
        /// <param name="layer">selected layer to draw</param>
        /// <param name="_tx">x cell postion</param>
        /// <param name="_ty">y cell postion</param>
        /// <param name="_tw">width cels count </param>
        /// <param name="_th">height cels count</param>
        /// <param name="_sx">at screen x position</param>
        /// <param name="_sy">at screen y position</param>
        /// <param name="_maskColor">mask color ID (unuported now, ID with alpha=0 is used)</param>
        /// <param name="_scale">unsused, insted use Display:PixelSize(<1/2/4>)</param>
        public void DrawLayer(int layer, int _tx, int _ty, int _tw, int _th, int _sx, int _sy, byte _maskColor = 0, int _scale = 1)
        {
            uRetroTilemap.DrawLayer(layer, _tx, _ty, _tw, _th, _sx, _sy, _maskColor, _scale);
        }

        /// <summary>
        /// [Tilemap:GetTileID] get tile ID
        /// </summary>
        /// <param name="layer">target tilemap layer ID</param>
        /// <param name="x">x tilemap position</param>
        /// <param name="y">y tilemap position</param>
        /// <returns></returns>
        public int GetTileID(int layer, int x, int y)
        {
            return uRetroTilemap.GetID(layer, x, y);
        }

        /// <summary>
        /// [Tilemap:GetTileFlag] get tile flag
        /// </summary>
        /// <param name="layer">target layer ID</param>
        /// <param name="x">tile x position</param>
        /// <param name="y">tile y position</param>
        /// <returns></returns>
        public int GetTileFlag(int layer, int x, int y)
        {
            return uRetroTilemap.GetFlag(layer, x, y);
        }

        /// <summary>
        /// [Tilemap:ImportTilemap] Import PyxelEdit exported tilemap
        /// </summary>
        /// <param name="filename">name of exported PyxelEdit tilemap in json format</param>
        public void ImportTilemap(string filename)
        {
            uRetroTilemap.ImportPyxelEditTilemap(filename);
        }

        /// <summary>
        /// [Tilemap:ImportCollisionMap] import PyxelEdit tilemap as collision map, then is converted to flags on tiles
        /// </summary>
        /// <param name="filename">name of exported PyxelEdit tilemap in json format</param>
        /// <param name="flags">array of defined tile substitution to flags (0..10) values are preddefined </param>
        /// <param name="targetLayerID"></param>
        public void ImportCollisionMap(string filename, int[] flags, int targetLayerID)
        {
            uRetroTilemap.ImportPyxelCollisionMap(filename, flags, targetLayerID);
        }

        /// <summary>
        /// [Tilemap:Save] save modified tilemap to game folder (uRe_Tilemap.json)
        /// </summary>
        public void Save()
        {
            uRetroTilemap.Save();
        }

        /// <summary>
        /// [Tilemap:Load] load tilemap from expanded cartridge (uRe_Tilemap.json)
        /// </summary>
        public void Load()
        {
            uRetroTilemap.Load();
        }
    }

    #endregion TILEMAP

    #region UTILS

    // ******************************************************************
    // uRetroEngine: UTILS
    // ******************************************************************
#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Utils
    {
        public void CodeProfilerStart(string label)
        {
            uRetroUtils.CodeProfilerStart(label);
        }

        public void CodeProfilerEnd(string label)
        {
            uRetroUtils.CodeProfilerEnd(label);
        }
    }

    #endregion UTILS

    #region CONSOLE

    // ******************************************************************
    // uRetroEngine: CONSOLE
    // ******************************************************************
    /// <summary>
    /// [Console: ... ] Build in console lua method or press [F1]
    /// </summary>

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Console
    {
        /// <summary>
        /// [Console: Show] show console from code
        /// </summary>
        public void Show()
        {
            uRetroConsole.Show();
        }

        /// <summary>
        /// [Console:Hide] hide opened console from code
        /// </summary>
        public void Hide()
        {
            uRetroConsole.Hide();
        }

        /// <summary>
        /// [Console:SwitchVisibility] open/close console from code, depend on actual state
        /// </summary>
        public void SwitchVisibility()
        {
            uRetroConsole.SwitchVisibility();
        }

        /// <summary>
        /// [Console:Print] print string value to console
        /// </summary>
        /// <param name="value">string value</param>
        public void Print(string value)
        {
            uRetroConsole.Print(value);
        }

        /// <summary>
        /// [Console:Print] print float value to console
        /// </summary>
        /// <param name="value">float value</param>
        public void Print(float value)
        {
            uRetroConsole.Print(value);
        }

        /// <summary>
        /// [Console:Print] print int value to console
        /// </summary>
        /// <param name="value">int value</param>
        public void Print(int value)
        {
            uRetroConsole.Print(value);
        }

        /// <summary>
        /// [Console:Clear] clear console
        /// </summary>
        public void Clear()
        {
            uRetroConsole.Clear();
        }
    }

    #endregion CONSOLE

    #region INPUT

    // ******************************************************************
    // uRetroEngine: INPUT
    // ******************************************************************
    /// <summary>
    /// [Input: ... ] Input lua methods
    /// </summary>

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_Input
    {
        /// <summary>
        /// [Input:ButtonDown] Check key button DOWN event
        /// </summary>
        /// <param name="key">key code (is predefined constant in lua globals)</param>
        /// <returns></returns>
        public bool ButtonDown(KeyCode key)
        {
            return uRetroInput.ButtonDown(key);
        }

        /// <summary>
        /// [Input:ButtonPressed] Check key button HOLD event
        /// </summary>
        /// <param name="key">key code (is predefined constant in lua globals)</param>
        /// <returns></returns>
        public bool ButtonPressed(KeyCode key)
        {
            return uRetroInput.ButtonHold(key);
        }

        /// <summary>
        /// [Input:ButtonUp] Check key button UP event
        /// </summary>
        /// <param name="key">key code (is predefined constant in lua globals)</param>
        /// <returns></returns>
        public bool ButtonUp(KeyCode key)
        {
            return uRetroInput.ButtonUp(key);
        }

        /// <summary>
        /// [Input:MouseX] return x mouse position
        /// </summary>
        /// <returns>x position</returns>
        public int MouseX()
        {
            return uRetroInput.MouseX();
        }

        /// <summary>
        /// [Input:MouseY] return y mouse position
        /// </summary>
        /// <returns>y position</returns>
        public int MouseY()
        {
            return uRetroInput.MouseY();
        }

        /// <summary>
        /// [Input:MouseButtonDown] Check mouse button DOWN event
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return true if mouse butin ID is down else false</returns>
        public bool MouseButtonDown(int id)
        {
            return uRetroInput.MouseButtonDown(id);
        }

        /// <summary>
        /// [Input:MouseButtonHold] Check mouse button HOLD event
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return true if mouse butin ID is hold else false</returns>
        public bool MouseButtonHold(int id)
        {
            return uRetroInput.MouseButtonHold(id);
        }

        /// <summary>
        /// [Input:MouseButtonUp] Check mouse button UP event
        /// </summary>
        /// <param name="id">button ID</param>
        /// <returns>return true if mouse butin ID is released else false</returns>
        public bool MouseButtonUp(int id)
        {
            return uRetroInput.MouseButtonUp(id);
        }

        /// <summary>
        /// [Input:IsInside] check if mouse pointer is inside active window
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsInside()
        {
            return uRetroInput.IsInside();
        }
    }

    #endregion INPUT

    #region GAMEDATA

    // ******************************************************************
    // uRetroEngine: GAME DATA
    // ******************************************************************
    /// <summary>
    /// [GameData: ... ] Game data lua methed
    /// </summary>

#if MOONSHARP

    [MoonSharpUserData]
#endif

    public class uRetroLuaClasses_GameData
    {
        /// <summary>
        /// [GameData:SetInt] Set integer value with defined name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">int value</param>
        public void SetInt(string name, int value)
        {
            uRetroGameData.SetInt(name, value);
        }

        /// <summary>
        /// [GameData:GetInt] Get int value from variable name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns> integer value</returns>
        public int GetInt(string name)
        {
            return uRetroGameData.GetAsInt(name);
        }

        /// <summary>
        /// [GameData:SetFloat] Set float value with defined name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">float value</param>
        public void SetFloat(string name, float value)
        {
            uRetroGameData.SetFloat(name, value);
        }

        /// <summary>
        /// [GameData:GetFloat] Get float value from variable name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns> float value</returns>
        public float GetFloat(string name)
        {
            return uRetroGameData.GetAsFloat(name);
        }

        /// <summary>
        /// [GameData:SetString] Set string value with defined name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">string value</param>
        public void SetString(string name, string value)
        {
            uRetroGameData.SetString(name, value);
        }

        /// <summary>
        /// [GameData:GetString] Get string value from variable name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns> string value</returns>
        public string GetString(string name)
        {
            return uRetroGameData.GetAsString(name);
        }

        /// <summary>
        /// [GameData:Save] Save game data in json format to uRetroEngine persistent data path with name <cartridgename.gamedata>
        /// </summary>
        public void Save()
        {
            uRetroGameData.Save();
        }

        /// <summary>
        /// [GameData:Load] Load game data in json format to uRetroEngine from persistent data path with name <cartridgename>.gamedata
        /// </summary>
        public void Load()
        {
            uRetroGameData.Load();
        }
    }

    #endregion GAMEDATA
}