using UnityEngine;
using static Assets.Scripts.CreateRoom.CreateRoom;

namespace Assets.Scripts.WorldGen
{
    public static class PathFinding
    {
        //    drunkardWalk(grid, startX, startY, targetTileCount) :
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
        //add visited places
        //add direction bias
        public static MapData RandomWalk(MapData mapData, Vector2Int start, int lengthLimit)
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
                    if (tempMapData.mapArray[startX][startY] >= 0) // is wall 
                    {
                        tempMapData.mapArray[startX][startY] = -3; 
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


        public static class Directions
        {
            public static Vector2Int Up = new Vector2Int(0, 1);
            public static Vector2Int Down = new Vector2Int(0, -1);
            public static Vector2Int Left = new Vector2Int(-1, 0);
            public static Vector2Int Right = new Vector2Int(1, 0);

            public static Vector2Int[] Cardinal =
            {
                Up, Down, Left, Right
            };

            private static System.Random rng = new System.Random();

            public static Vector2Int GetRandom()
            {
                int index = rng.Next(Cardinal.Length);
                return Cardinal[index];
            }
        }
    }
}
