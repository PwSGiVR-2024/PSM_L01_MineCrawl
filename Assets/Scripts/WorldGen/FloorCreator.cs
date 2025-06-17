using Assets.Scripts.WorldGen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;

namespace Assets.Scripts.CreateRoom
{
    /*
    Creates and manages data about floor in MapData struct.
    Generates rooms, walls and paths* and saves them to 2D array as values.
    They are then used by FloorRenderer to draw them on a tilemap.

    *Paths are generated using PathFinding class.
    

    TODO:
    - refactor
    - prefer unityengine.random
    - player.transform.position = new Vector3(room.rootCoords.x, room.rootCoords.y, player.transform.position.z); insead of translate
    - scriptable object instaed of LoadAll
    */
    public class FloorCreator : MonoBehaviour
    {
        public struct MapData
        {
            public int width;
            public int height;
            public int[][] mapArray;
            public FinishAltar[] altars;
        }

        public struct Room
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

        //public TilePalette tilePallete;
        public TileBase[] tilePallete;
        public TileBase[] tilePalleteStone;//tests
        public TileBase tile;
        public TileBase rootTile; //tests
        public TileBase pathTile; //tests
        public Tilemap groundMap;
        public Tilemap wallsMap;

        protected static List<Room> rooms;
        protected MapData mapData;
        protected FloorRenderer renderer;

        private Vector2Int lastPlayerPos;
        private GameObject player;
        private FinishAltar altarCreator;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            altarCreator = GetComponent<FinishAltar>();
            lastPlayerPos = new Vector2Int(-1, -1);
            print("Start: " + lastPlayerPos);
            //CreateFloor();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DumpMapToFile();
            }
        }

        public MapData CreateFloor()
        {
            rooms = new List<Room>();
            //tilePallete.tiles = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
            tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
            //Array.Resize(ref tilePallete.tiles, 32); //include only clean grass
            Array.Resize(ref tilePallete, 32); //include only clean grass
            tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");

            mapData = GenerateArray(64, 64);
            mapData = GenerateFloor(mapData);
            mapData = GenerateCorridors(mapData);
            foreach(var altar in mapData.altars)
            {
                altar.SpawnAltar(mapData, 0);
            }

            //renderer = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorRenderer>();
            //renderer.RenderMap(mapData);
            SpawnPlayer();

            return mapData;
        }

        private void SpawnPlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            print("X " + lastPlayerPos);

            if (lastPlayerPos.x <= 0 && lastPlayerPos.y <= 0)
            {
                var room = GetRandomRoom();
                var coordsValue = mapData.mapArray[room.rootCoords.x][room.rootCoords.y];
                print("First: "+ coordsValue);
                while (coordsValue >= 0)//wall
                {
                    room = GetRandomRoom();
                    coordsValue = mapData.mapArray[room.rootCoords.x][room.rootCoords.y];
                }
                print("Final: " + coordsValue);
                player.transform.Translate(room.rootCoords.x, room.rootCoords.y, 0);
                lastPlayerPos = new Vector2Int(room.rootCoords.x, room.rootCoords.y);
                print("A " + lastPlayerPos);
            }
            else
            {
                player.transform.Translate(lastPlayerPos.x, lastPlayerPos.y, 0);
                print("B " + lastPlayerPos);
            }
        }

        protected MapData GenerateArray(int width, int height)
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
                    //mapData.mapArray[x][y] = Mathf.RoundToInt(UnityEngine.Random.Range(0, tilePallete.tiles.Length));
                    mapData.mapArray[x][y] = Mathf.RoundToInt(UnityEngine.Random.Range(0, tilePallete.Length));
                }
            }
            return mapData;
        }

        protected static MapData GenerateFloor(MapData mapData)
        {
            int numberOfRooms = Mathf.RoundToInt(UnityEngine.Random.Range(5, 10));

            mapData = GenerateRoom(mapData, numberOfRooms);

            return mapData;
        }

        protected static MapData GenerateRoom(MapData mapData, int count, int assignValue = -1)
        {
            int padding = 5;
            int width = mapData.width - padding;
            int height = mapData.height - padding;

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

                    int roomWidth = Mathf.RoundToInt(UnityEngine.Random.Range(4, 8));
                    int roomHeight = Mathf.RoundToInt(UnityEngine.Random.Range(4, 8));

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

        protected static MapData GenerateCorridors(MapData mapData)
        {
            MapData tempMapData = mapData;

            foreach (Room room in rooms)
            {
                Vector2Int start = room.rootCoords;
                //Vector2Int goal = rooms[j].rootCoords;
                tempMapData = PathFinding.RandomWalk(mapData, new Vector2Int(start.x, start.y), 500, rooms);//500 cigarettes default
            }


            return tempMapData;
        }

        public Room GetRandomRoom()
        {
            return rooms[UnityEngine.Random.Range(0, rooms.Count)];
        }

        private void DumpMapToFile()
        {
            string fileName = "MapDump.txt";
            string path = Path.Combine(Application.dataPath, fileName);
            using (StreamWriter writer = new StreamWriter(path))
            {
                int rows = mapData.width;
                int cols = mapData.height;

                for (int y = 0; y < rows; y++)
                {
                    string line = "";
                    for (int x = 0; x < cols; x++)
                    {
                        line += mapData.mapArray[x][y].ToString().PadLeft(2, ' ') + " ";
                    }
                    writer.WriteLine(line.Trim());
                }
            }

            Debug.Log("Map data dumped to: " + path);
        }

    }
}

