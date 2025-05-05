using Assets.Scripts.WorldGen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.CreateRoom
{
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
        public TileBase pathTile; //tests
        public Tilemap map;

        private struct Room
        {
            public Vector2Int rootCoords;
            public int width;
            public int height;

            public Room(Vector2Int position, int width, int height)
            {
                this.rootCoords = position;
                this.width = width;
                this.height = height;
            }
        }
        private static List<Room> rooms;

        private void Awake()
        {
            rooms = new List<Room>();
            tilePallete.tiles = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
            tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");
        }

        void Start()
        {
            MapData mapData = GenerateArray(64, 64);
            mapData = GenerateFloor(mapData);
            mapData = GenerateCorridors(mapData);//test
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
                    else if (mapData.mapArray[x][y] == -2)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                    else if (mapData.mapArray[x][y] == -3)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), pathTile);
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

            for (int i = 0; i < width; i++)
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

        private static MapData GenerateFloor(MapData mapData)
        {
            int numberOfRooms = Mathf.RoundToInt(Random.Range(5, 10));

            mapData = GenerateRoom(mapData, numberOfRooms);

            return mapData;
        }

        private static MapData GenerateRoom(MapData mapData, int count, int assignValue = -1)
        {
            int width = mapData.width;
            int height = mapData.height;

            int regionCols = Mathf.CeilToInt(Mathf.Sqrt(count));
            int regionRows = Mathf.CeilToInt((float)count / regionCols);

            int regionWidth = width / regionCols;
            int regionHeight = height / regionRows;

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

                    int roomWidth = Mathf.RoundToInt(Random.Range(3, 6));
                    int roomHeight = Mathf.RoundToInt(Random.Range(3, 6));

                    int widthLimit = roomWidth;

                    mapData.mapArray[x][y] = assignValue; //root
                    rooms.Add(new Room(new Vector2Int(x, y), roomWidth, roomHeight));

                    for (int i = x; i < width - roomWidth; i++)
                    {
                        if (widthLimit <= 0) break;

                        int heightLimit = roomHeight;

                        for (int j = y; j < height - roomHeight; j++)
                        {
                            if (heightLimit <= 0) break;

                            mapData.mapArray[i][j] = -2; //room

                            heightLimit--;
                        }

                        widthLimit--;
                    }

                }
            }

            return mapData;
        }

        private static MapData GenerateCorridors(MapData mapData)
        {
            MapData tempMapData = mapData;

            foreach(Room room in rooms)
            {
                Vector2Int start = room.rootCoords;
                //Vector2Int goal = rooms[j].rootCoords;
                tempMapData = PathFinding.RandomWalk(mapData, new Vector2Int(start.x, start.y), 200);
            }

            return tempMapData;
        }
    }
}


