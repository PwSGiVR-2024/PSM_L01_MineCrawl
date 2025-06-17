using UnityEngine.Tilemaps;
using UnityEngine;
using System;

namespace Assets.Scripts.CreateRoom
{
    /*
    Takes MapData and draws it on the scene

    TODO:
    - SO instead of LoadAll
    - batch rendering SetTilesBLock:
        public void RenderMap(FloorCreator.MapData mapData)
{
            groundMap.ClearAllTiles();
            wallsMap.ClearAllTiles();

            BoundsInt bounds = new BoundsInt(0, 0, 0, mapData.width, mapData.height, 1);
            TileBase[] groundTiles = new TileBase[mapData.width * mapData.height];
            TileBase[] wallTiles = new TileBase[mapData.width * mapData.height];

            for (int x = 0; x < mapData.width; x++)
            {
                for (int y = 0; y < mapData.height; y++)
                {
                    int value = mapData.mapArray[x][y];
                    int index = x + y * mapData.width;

                    switch (value)
                    {
                        case -1:
                            groundTiles[index] = palette.rootTile;
                            break;
                        case -2:
                            groundTiles[index] = palette.roomTile;
                            break;
                        case -3:
                            groundTiles[index] = palette.pathTile;
                            break;
                        default:
                            if (value >= 0 && value < palette.wallTiles.Length)
                            {
                                wallTiles[index] = palette.wallTiles[value];
                            }
                            break;
                    }
                }
            }

            groundMap.SetTilesBlock(bounds, groundTiles);
            wallsMap.SetTilesBlock(bounds, wallTiles);
}   
    -

    */
    public class FloorRenderer : MonoBehaviour
    {
        public Tilemap groundMap;
        public Tilemap wallsMap;
        public TileBase[] tilePallete;
        public TileBase rootTile;
        public TileBase roomTile;
        public TileBase pathTile;

        public void RenderMap(FloorCreator.MapData mapData)
        {
            print("Render");
            groundMap.ClearAllTiles();
            wallsMap.ClearAllTiles();
            
            tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
            //Array.Resize(ref tilePallete.tiles, 32); //include only clean grass
            Array.Resize(ref tilePallete, 32); //include only clean grass
            //tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");
            for (int x = 0; x < mapData.width; x++)
            {
                for (int y = 0; y < mapData.height; y++)
                {
                    int value = mapData.mapArray[x][y];
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    switch (value)
                    {
                        case -1:
                            groundMap.SetTile(pos, rootTile);
                            break;
                        case -2:
                            groundMap.SetTile(pos, roomTile);
                            break;
                        case -3:
                            groundMap.SetTile(pos, pathTile);
                            break;
                        default:
                            if (value >= 0 && value < tilePallete.Length)
                            {
                                wallsMap.SetTile(pos, tilePallete[value]);
                            }
                            break;
                    }
                }
            }


        }
    }
}