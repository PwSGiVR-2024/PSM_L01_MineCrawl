using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class CreateRoom : MonoBehaviour
{
    public TilePalette tilePallete;
    public struct MapData
    {
        public int width;
        public int height;
        public int[][] mapArray;
    }
    public TileBase tile;
    public Tilemap map;

    private void Awake()
    {
        tilePallete.tiles = Resources.LoadAll<Tile>("Tile Pallete/TP Grass");

        foreach (var t in tilePallete.tiles)
        {
            Debug.Log(t.name);
        }
    }

    void Start()
    {
        MapData mapData = GenerateArray(24, 12);
        RenderMap(mapData, map, tile);
    }

    public void RenderMap(MapData mapData, Tilemap tilemap, TileBase tile)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < mapData.width; x++)
        {
            for (int y = 0; y < mapData.height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tilePallete.tiles[mapData.mapArray[x][y]]);
            }
        }
    }

    public MapData GenerateArray(int width, int height)
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
                print(tilePallete.tiles.Length);
                /*
                if (random == 0)
                {
                    mapData.mapArray[x][y] = 1;
                }
                else
                {
                    mapData.mapArray[x][y] = 1;
                }
                */
                //print(mapData.mapArray[x][y]);
            }
        }
        return mapData;
    }
}
