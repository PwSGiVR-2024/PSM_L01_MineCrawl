using Assets.Scripts.CreateRoom;
using UnityEngine;

/*
Enables player to go up/down between floors
Goal is to go down, therefore player should look for Down Altars.
Up Altar is in the same place the player is when moved to new floor.
Down Altar becoms Up Altar on the next floor.

To keep only one prefab but change its sprite based on the current ability, 
you can handle this with a simple script that updates the SpriteRenderer component based on the state
(e.g., "Ability A" vs. "Ability B").
*/

public class FinishAltar : FloorCreator
{
    public enum Direction
    {
        Up = 0,
        Down = 1
    };
    public Direction direction;
    public GameObject altarPrefab;

    private FloorChanger floorChanger;
    private bool playerInRange;
    private Vector2 coords;

    private void Start()
    {
        floorChanger = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorChanger>();
        playerInRange = false;
    }

    public Vector2 GetCoords()
    {
        return coords;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //public MapData SpawnAltar(MapData mapData, Direction dir)
    //{
    //    MapData tempMapData = mapData;

    //    var room = rooms[Random.Range(0, rooms.Count)];
    //    coords = new Vector2(room.rootCoords.x, room.rootCoords.y);
    //    direction = dir;
    //    switch (dir)
    //    {
    //        case Direction.Up:
    //            tempMapData.altars[0] = this;
    //            break;
    //        case Direction.Down:
    //            tempMapData.altars[1] = this;
    //            break;
    //        default:
    //            break;
    //    }
        
    //    Instantiate(altarPrefab, coords, Quaternion.identity);

    //    return tempMapData;
    //}

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                floorChanger.ChangeFloor(this);
            }
            
        }
    }

}



