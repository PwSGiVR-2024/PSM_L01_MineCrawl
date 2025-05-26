using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.CreateRoom.CreateRoom;

namespace Assets.Scripts.WorldGen
{
    public static class PathFinding
    {
        //drunkardWalk(grid, startX, startY, targetTileCount) :
        //x, y = startX, startY
        //carved = 0

        //while carved<targetTileCount:
        //    if grid[x][y] is wall:
        //        grid[x][y] = floor
        //        carved += 1

        //    direction = random(up, down, left, right)

        //    if direction == up and y > 0: y -= 1
        //    if direction == down and y<height - 1: y += 1
        //    if direction == left and x > 0: x -= 1
        //    if direction == right and x<width - 1: x += 1

        //return grid

        //TODO
        //add direction bias

        private static Queue<(int x, int y)> visitedTiles = new Queue<(int x, int y)>();

        public static MapData RandomWalk(MapData mapData, Vector2Int start, int lengthLimit, List<Room> rooms)
        {
            MapData tempMapData = mapData;
            int startX = start.x;
            int startY = start.y;
            int walked = 0;

            int width = tempMapData.mapArray.Length;
            int height = tempMapData.mapArray[0].Length; // assumes uniform row length

            while (walked < lengthLimit)
            {
                // Bounds check before access
                if (startX >= 0 && startX < width &&
                    startY >= 0 && startY < height)
                {
                    if (visitedTiles.Contains((startX, startY)) == false){ //check if visited
                        if (tempMapData.mapArray[startX][startY] >= 0) // is wall 
                        {
                            tempMapData.mapArray[startX][startY] = -3;
                            visitedTiles.Enqueue((startX, startY));

                            if (walked > 5)
                            {
                                visitedTiles.Dequeue();
                            }

                            (int x, int y) closestRoom = FindClosestRoom(mapData, rooms, (startX, startY));
                            int distanceX = closestRoom.x - startX;
                            int distanceY = closestRoom.y - startY;
                            
                            if(distanceX > distanceY)
                            {
                                if(distanceX > 0) // + > left, - > right
                                {
                                    Directions.AddWeight(Directions.Left, 1);
                                }
                                else
                                {
                                    Directions.AddWeight(Directions.Right, 1);
                                }
                            }
                            else// X < Y
                            {
                                if (distanceY > 0) // + > up, - > down
                                {
                                    Directions.AddWeight(Directions.Up, 1);
                                }
                                else
                                {
                                    Directions.AddWeight(Directions.Down, 1);
                                }
                            }

                        }
                    }
                }

                Vector2Int lastDirection = Directions.GetRandom();
                Vector2Int dir = lastDirection;
                if (Random.value < 0.3f)
                {
                    dir = Directions.GetRandom();
                }
                lastDirection = dir;

                // preview move
                int nextX = startX + dir.x;
                int nextY = startY + dir.y;

                // only apply move if in bounds
                if (nextX >= 0 && nextX < width &&
                    nextY >= 0 && nextY < height)
                {
                    startX = nextX;
                    startY = nextY;
                }

                walked++;
            }

            return tempMapData;
        }


        private static class Directions
        {
            public static readonly Vector2Int Up = new Vector2Int(0, 1);
            public static readonly Vector2Int Down = new Vector2Int(0, -1);
            public static readonly Vector2Int Left = new Vector2Int(-1, 0);
            public static readonly Vector2Int Right = new Vector2Int(1, 0);

            public static Dictionary<Vector2Int, int> weightedDirections = new()
            {
                { Up, 1 },
                { Down, 1 },
                { Left, 1 },
                { Right, 1 }
            };

            private static System.Random rng = new System.Random();

            public static void AddWeight(Vector2Int direction, int amount = 1)
            {
                foreach (var key in weightedDirections.Keys.ToList())
                {
                    weightedDirections[key] = (key == direction) ? Mathf.Max(0, weightedDirections[direction] + amount) : Mathf.Max(0, weightedDirections[direction] - amount); ;
                }
            }

            public static Vector2Int GetRandom()
            {
                int totalWeight = weightedDirections.Values.Sum();
                int roll = rng.Next(totalWeight);
                int cumulative = 0;

                foreach (var kvp in weightedDirections)
                {
                    cumulative += kvp.Value;
                    if (roll < cumulative)
                    {
                        return kvp.Key;
                    }
                }

                return Right; // fallback
            }
        }

        private static (int x, int y) FindClosestRoom(MapData mapData, List<Room> rooms, (int x, int y) currPosition)
        {
            (int x, int y) closestRoomCoords = (-1,-1);
            float minDistance = 0.0f;

            foreach(var room in rooms)
            {
                float currDistance = Mathf.Sqrt(Mathf.Pow(room.rootCoords.x - currPosition.x, 2) + Mathf.Pow(room.rootCoords.y - currPosition.y, 2)); //distance (not manhattan)
                if (currDistance < minDistance)
                {
                    closestRoomCoords.x = room.rootCoords.x;
                    closestRoomCoords.y = room.rootCoords.y;
                }
            }

            return closestRoomCoords;
        }
    }
}
