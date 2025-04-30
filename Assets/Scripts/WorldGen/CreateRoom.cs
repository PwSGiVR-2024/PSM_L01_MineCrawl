using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class CreateRoom : MonoBehaviour
{
    public TilePalette tilePallete;
    public TileBase[] tilePalleteStone;//tests
    public struct MapData
    {
        public int width;
        public int height;
        public int[][] mapArray;
    }
    public TileBase tile;
    public TileBase rootTile; //tests
    public Tilemap map;

    private void Awake()
    {
        Debug.Log("AWAKE");
        tilePallete.tiles = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
        tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");
    }

    void Start()
    {
        MapData mapData = GenerateArray(64, 64);
        mapData = GenerateRoom(mapData);
        RenderMap(mapData, map, tile);  
    }

    public void RenderMap(MapData mapData, Tilemap tilemap, TileBase tile)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < mapData.width; x++)
        {
            for (int y = 0; y < mapData.height; y++)
            {
                if (mapData.mapArray[x][y] == -1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), rootTile);

                }
                else if(mapData.mapArray[x][y] == -2)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tilePallete.tiles[mapData.mapArray[x][y]]);
                }
            }
        }
    }

    private MapData GenerateArray(int width, int height)
    {
        MapData mapData = new MapData();
        mapData.width = width;
        mapData.height = height;
        mapData.mapArray = new int[width][];

        for(int i = 0; i < width; i++)
        {
            mapData.mapArray[i] = new int[height];
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mapData.mapArray[x][y] = Mathf.RoundToInt(Random.Range(0, tilePallete.tiles.Length));
            }
        }
        return mapData;
    }

    private static MapData GenerateRoom(MapData mapData)
    {
        int numberOfRooms = Mathf.RoundToInt(Random.Range(5, 10));

        mapData = CreateRoomRoot(mapData, numberOfRooms);

        return mapData;
    }

    private static MapData CreateRoomRoot(MapData mapData, int count, int assignValue = -1)
    {
        int width = mapData.width;
        int height = mapData.height;

        int regionCols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int regionRows = Mathf.CeilToInt((float)count / regionCols);

        int regionWidth = width / regionCols;
        int regionHeight = height / regionRows;

        int roomWidth = Mathf.RoundToInt(Random.Range(3, 6));
        int roomHeight = Mathf.RoundToInt(Random.Range(3, 6));

        System.Random rnd = new System.Random();

        for (int row = 0; row < regionRows; row++)
        {
            for (int col = 0; col < regionCols; col++)
            {
                int minX = col * regionWidth;
                int minY = row * regionHeight;
                int maxX = Mathf.Min(width, minX + regionWidth);
                int maxY = Mathf.Min(height, minY + regionHeight);

                if (maxX <= minX || maxY <= minY) continue;

                int x = rnd.Next(minX, maxX);
                int y = rnd.Next(minY, maxY);

                int widthLimit = roomWidth;

                for (int i = x; i < width - roomWidth; i++)
                {
                    if (widthLimit <= 0) break;

                    int heightLimit = roomHeight;

                    for (int j = y; j < height - roomHeight; j++)
                    {
                        if (heightLimit <= 0) break;
                       
                        mapData.mapArray[i][j] = -2;
                        
                        heightLimit--;
                    }

                    widthLimit--;
                }
                
                mapData.mapArray[x][y] = assignValue;
            }
        }

        return mapData;
    }
}
