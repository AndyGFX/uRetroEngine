using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace uRetroEngine
{
    /// <summary>
    /// Tile definition
    /// </summary>
    public struct TilemapCell
    {
        public int spriteID;
        public bool flipX;
        public bool flipY;
        public byte flag;
    }

    /// <summary>
    /// Tilemap layer definition
    /// </summary>
    public struct TilemapLayer
    {
        public TilemapCell[,] tilemap;
    }

    /// <summary>
    /// uRetro engine tilemap data
    /// </summary>
    public static class uRetroTilemap
    {
        public static List<TilemapLayer> layers = null;
        public static int FLAG_FREE = 0;
        public static int FLAG_WALL = 1;
        public static int FLAG_OBSTACLE = 2;
        public static int FLAG_LADDER = 3;
        public static int FLAG_HAZARD = 4;
        public static int FLAG_CHEST = 5;
        public static int FLAG_ITEM = 6;
        public static int FLAG_COIN = 7;
        public static int FLAG_LIFE = 8;

        /// <summary>
        /// Clear tilemap and all layer inside with value -1
        /// </summary>
        public static void Clear()
        {
            for (int i = 0; i < uRetroConfig.tilemap_layers; i++)
            {
                for (int tx = 0; tx < uRetroConfig.tilemap_width; tx++)
                {
                    for (int ty = 0; ty < uRetroConfig.tilemap_height; ty++)
                    {
                        layers[i].tilemap[tx, ty].spriteID = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Create tilemap. Tile[0,0] is bottom left.
        /// </summary>
        /// <param name="w">columns count</param>
        /// <param name="h">rows count</param>
        public static void CreateTilemap(int w, int h, int l)

        {
            layers = new List<TilemapLayer>();
            uRetroConfig.tilemap_width = w;
            uRetroConfig.tilemap_height = h;
            uRetroConfig.tilemap_layers = l;
            for (int i = 0; i < uRetroConfig.tilemap_layers; i++)
            {
                TilemapLayer layer = new TilemapLayer();
                layer.tilemap = new TilemapCell[w, h];
                /*
                for (int tx = 0; tx < w; tx++)
                {
                    for (int ty = 0; ty < w; ty++)
                    {
                        layer.tilemap[tx, ty].spriteID = -1;
                    }
                }
                */
                layers.Add(layer);
            }

            Clear();
        }

        /// <summary>
        /// Set Tile to tilemap
        /// </summary>
        /// <param name="layer">target layer ID (default layers count=2)</param>
        /// <param name="x">x position in map</param>
        /// <param name="y">y position in map</param>
        /// <param name="spriteID">sprite ID from sprites</param>
        /// <param name="flag">user defined flat stored on tile (max value 255)</param>
        /// <param name="flipX">flip tile in X axis</param>
        /// <param name="flipY">flip tile in Y axis</param>
        public static void SetTile(int layer, int x, int y, int spriteID, byte flag = 0, bool flipX = false, bool flipY = false)
        {
            if ((x < 0) || (x >= uRetroConfig.tilemap_width)) return;
            if ((y < 0) || (y >= uRetroConfig.tilemap_height)) return;

            if (layers[layer].tilemap == null)
            {
                uRetroConsole.Print("Tilemap doesn't exist. Add 'uRetroTilemap.CreateTilemap(64,64);' in initialization.");
            }

            if ((x >= uRetroConfig.tilemap_width) || (y >= uRetroConfig.tilemap_height))
            {
                uRetroConsole.Print("[x,y] is out of tilemap range");
                return;
            }

            if (layer >= uRetroConfig.tilemap_layers)
            {
                uRetroConsole.Print("Tilemap layer ID is out of tilemap range");
                return;
            }

            layers[layer].tilemap[x, uRetroUtils.FlipTileY(y)].spriteID = spriteID;
            layers[layer].tilemap[x, uRetroUtils.FlipTileY(y)].flag = flag;
            layers[layer].tilemap[x, uRetroUtils.FlipTileY(y)].flipX = flipX;
            layers[layer].tilemap[x, uRetroUtils.FlipTileY(y)].flipY = flipY;

            // DrawTile(layer, x, y, x * uRetroConfig.sprite_width, y * uRetroConfig.sprite_height);
        }

        /// <summary>
        /// Draw line to tilemap
        /// </summary>
        /// <param name="x0">from x position</param>
        /// <param name="y0">fromn y position</param>
        /// <param name="x1">to x position</param>
        /// <param name="y1">to y posiyion</param>
        /// <param name="layer">target tilemap layer</param>
        /// <param name="spriteID">sprite ID from sprites</param>
        /// <param name="flag">user defined flat stored on tile (max value 255)</param>
        /// <param name="flipX">flip tile in X axis</param>
        /// <param name="flipY">flip tile in Y axis</param>
        public static void SetTileLine(int x1, int y1, int x2, int y2, int layer, int spriteID, byte flag = 0, bool flipX = false, bool flipY = false)
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
            SetTile(layer, xx, yy, spriteID, flag, flipX, flipY);

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
                SetTile(layer, xx, yy, spriteID, flag, flipX, flipY);
            }
        }

        public static void DrawTile(int layer, int tx, int ty, int sx, int sy)
        {
            uRetroImage tilePixels = null;

            if (uRetroConfig.flipScreenY)
            {
                ty = (uRetroConfig.tilemap_height - 1) - ty;
            }

            int spriteID = spriteID = layers[layer].tilemap[tx, ty].spriteID;

            if (spriteID == -1)
            {
                tilePixels = new uRetroImage(uRetroConfig.sprite_width, uRetroConfig.sprite_height);
            }
            else
            {
                tilePixels = uRetroSprites.GetSpriteRetroImage(spriteID).GetFlipedImage(layers[layer].tilemap[tx, ty].flipX, layers[layer].tilemap[tx, ty].flipY);
            }

            int tW = uRetroConfig.sprite_width;
            int tH = uRetroConfig.sprite_height;
            int tX = tx * tW;
            int tY = ty * tH;

            uRetroUtils.DrawImage(tilePixels, sx, sy);
        }

        /// <summary>
        /// Return tile ID at position tilemap[X,Y] from specific layer
        /// </summary>
        /// <param name="layer">layer ID</param>
        /// <param name="x">x position in map</param>
        /// <param name="y">y position in map</param>
        /// <returns></returns>
        public static int GetID(int layer, int x, int y)
        {
            return layers[layer].tilemap[x, uRetroUtils.FlipTileY(y)].spriteID;
        }

        /// <summary>
        /// Return tile flag at position tilemap[X,Y] from specific layer
        /// </summary>
        /// <param name="layer">layer ID</param>
        /// <param name="x">x position in map</param>
        /// <param name="y">y position in map</param>
        /// <returns></returns>
        public static int GetFlag(int layer, int x, int y)
        {
            return layers[layer].tilemap[x, uRetroUtils.FlipTileY(y)].spriteID;
        }

        /// <summary>
        /// Draw visible part of tilemap to screen at once.
        /// </summary>
        /// <param name="offsetX">tilemap x offset </param>
        /// <param name="offsetY">tilemap y offset </param>
        public static void DrawMap(int _tx, int _ty, int _tw, int _th, int _sx, int _sy, byte _maskColor = 0, int _scale = 1)
        {
            for (int l = uRetroConfig.tilemap_layers - 1; l >= 0; l--)
            {
                DrawLayer(l, _tx, _ty, _tw, _th, _sx, _sy, _maskColor, _scale);
            }
        }

        public static void DrawLayer(int _layer, int _tx, int _ty, int _tw, int _th, int _sx, int _sy, byte _maskColor = 0, int _scale = 1)
        {
            // [1] transfer tiles to buffer [tx,ty] -> [tx+tw,ty+th]
            int ox = 0;
            int oy = 0;
            for (int tx = _tx; tx < _tx + _tw; tx++)
            {
                oy = 0;
                for (int ty = _ty; ty < _ty + _th; ty++)
                {
                    int sx = _sx + (ox * uRetroConfig.sprite_width);
                    int sy = _sy + (oy * uRetroConfig.sprite_height); ;
                    DrawTile(_layer, tx, ty, sx, sy);
                    oy++;
                }
                ox++;
            }

            // [2] draw tilemap [tx,ty] -> [tx+tw,ty+th] to screen at [sx,sy]
        }

        /// <summary>
        /// Import tilemap created in PyxelEdit program saved as JSON file
        /// </summary>
        /// <param name="filename"></param>
        public static void ImportPyxelEditTilemap(string filename)
        {
            PyxelEditTilemap tilemap;

            string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + filename;
            string json = File.ReadAllText(path);

            tilemap = JsonConvert.DeserializeObject<PyxelEditTilemap>(json);

            uRetroConfig.tilemap_layers = tilemap.layers.Length;
            uRetroTilemap.CreateTilemap(tilemap.tileswide, tilemap.tileshigh, tilemap.layers.Length);

            for (int i = 0; i < tilemap.layers.Length; i++)
            {
                int layer_id = i;

                for (int t = 0; t < tilemap.layers[layer_id].tiles.Length; t++)
                {
                    int x = tilemap.layers[layer_id].tiles[t].x;

                    int y = (tilemap.tileshigh - 1) - tilemap.layers[layer_id].tiles[t].y;

                    int sprite_id = tilemap.layers[layer_id].tiles[t].tile;
                    int rot = tilemap.layers[layer_id].tiles[t].rot;
                    bool fx = tilemap.layers[layer_id].tiles[t].flipX;
                    bool fy = (rot == 2) ? true : false;
                    if (rot == 2) fx = !fx;

                    uRetroTilemap.SetTile(layer_id, x, y, sprite_id, 0, fx, fy);
                }
            }

            uRetroSystem.SaveRetroEngineConfig();
        }

        /// <summary>
        /// Import PyxelEdit tilemap as collision for setup flags on target layer ID
        /// </summary>
        /// <param name="filename">PyxelEdit exported tilemap filename</param>
        /// <param name="flags">user defined flags rules table flagp[x]=spriteID</param>
        /// <param name="targetLayerID">target layer for write flags</param>
        public static void ImportPyxelCollisionMap(string filename, int[] flags, int targetLayerID)
        {
            PyxelEditTilemap tilemap;

            string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + filename;
            string json = File.ReadAllText(path);

            tilemap = JsonConvert.DeserializeObject<PyxelEditTilemap>(json);

            int layer_id = 0;

            for (int t = 0; t < tilemap.layers[layer_id].tiles.Length; t++)
            {
                int x = tilemap.layers[layer_id].tiles[t].x;
                int y = (tilemap.tileshigh - 1) - tilemap.layers[layer_id].tiles[t].y;

                int sprite_id = tilemap.layers[layer_id].tiles[t].tile;

                for (int f = 0; f < flags.Length; f++)
                {
                    if (sprite_id == flags[f])
                    {
                        layers[targetLayerID].tilemap[x, y].flag = (byte)f;
                    }
                }
            }
        }

        /// <summary>
        /// Save Tilemap to file (internal uRetro format)
        /// </summary>
        public static void Save()
        {
            string tilemap = JsonConvert.SerializeObject(uRetroTilemap.layers, Formatting.Indented);
            string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/";
            File.WriteAllText(path + uRetroConfig.fileTilemap, tilemap);
        }

        /// <summary>
        /// Load Tilemp from file (internal uRetro format)
        /// </summary>
        public static void Load()
        {
            string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/";
	        Load(path);
        }
	    
	    public static void Load(string path)
	    {		    
	    	Debug.Log(path+ uRetroConfig.fileTilemap);
		    string json = File.ReadAllText(path + uRetroConfig.fileTilemap);
		    layers = JsonConvert.DeserializeObject<List<TilemapLayer>>(json);
		    if (layers!=null) uRetroConfig.tilemap_layers = layers.Count;
	    }
	    
    }
}