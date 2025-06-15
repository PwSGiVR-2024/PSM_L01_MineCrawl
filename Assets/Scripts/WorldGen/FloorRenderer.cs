using UnityEngine.Tilemaps;
using UnityEngine;
using System;

namespace Assets.Scripts.CreateRoom
{
    public class FloorRenderer : MonoBehaviour
    {
        public Tilemap groundMap;
        public Tilemap wallsMap;
        public TileBase[] tilePallete;
        public TileBase rootTile;
        public TileBase roomTile;
        public TileBase pathTile;

        public void RenderMap(CreateRoom.MapData mapData)
        {
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