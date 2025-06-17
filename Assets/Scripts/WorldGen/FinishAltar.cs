using Assets.Scripts.CreateRoom;
using UnityEngine;

public class FinishAltar : CreateRoom
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
    private GameObject player;
    private Vector2 coords;

    private void Start()
    {
        floorChanger = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorChanger>();
        playerInRange = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    public MapData SpawnAltar(MapData mapData, Direction dir)
    {
        MapData tempMapData = mapData;

        var room = rooms[Random.Range(0, rooms.Count)];
        coords = new Vector2(room.rootCoords.x, room.rootCoords.y);
        direction = dir;
        tempMapData.altars.Add(this);
        Instantiate(altarPrefab, coords, Quaternion.identity);

        return tempMapData;
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                floorChanger.ChangeFloor(1);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                floorChanger.ChangeFloor(-1);
            }
        }
    }

}



