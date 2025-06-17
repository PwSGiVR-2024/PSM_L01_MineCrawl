using Assets.Scripts.CreateRoom;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class FloorChanger : CreateRoom
{
    public GameObject nextFloorAltarPrefab;

    private CreateRoom floorData;
    private FloorRenderer floorRenderer;
    private Room altarSpawnRoom;
    private Vector2 altarSpawnPoint;
    private List<MapData> floors;
    private int currentFloor;
    private int highestFloor;

    private void Start()
    {
        floorRenderer = GetComponent<FloorRenderer>();
        floorData = GetComponent<CreateRoom>();
        currentFloor = 0;
        floors = new List<MapData>();
        //CreateFloorChanger(mapData);
    }

    //private Vector2 GetSpawnPoint()
    //{
    //    altarSpawnRoom = floorData.GetRandomRoom();
    //    altarSpawnPoint = altarSpawnRoom.rootCoords;
    //    altarSpawnPoint.x += altarSpawnRoom.width / 2;
    //    altarSpawnPoint.y += altarSpawnRoom.height / 2;
    //    //check if ground

    //    return altarSpawnPoint;
    //}

    //private void CreateFloorChanger(MapData mapData)
    //{
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    altarSpawnPoint = GetSpawnPoint();
    //    float distanceToAltarSpawn = Vector3.Distance(player.transform.position, altarSpawnPoint);
    //    float allowedDistance = 2.0f;

    //    while (distanceToAltarSpawn <= allowedDistance)
    //    {
    //        altarSpawnPoint = GetSpawnPoint();
    //        distanceToAltarSpawn = Vector3.Distance(player.transform.position, altarSpawnPoint);
    //    }

    //    if (GameObject.FindGameObjectWithTag("Finish") == null)
    //    {
    //        Instantiate(nextFloorAltarPrefab, altarSpawnPoint, Quaternion.identity);
    //    }
    //}

    public void ChangeFloor(int floorChange)
    {
        // Prevent going below 0th floor
        if (currentFloor + floorChange < 0)
        {
            Debug.LogWarning("Cannot go below 0th floor.");
            return;
        }

        // Save current floor before changing
        if (floors.Count > 0)
        {
            floors[currentFloor] = mapData;
        }

        int floorNumber = currentFloor + floorChange;

        // First floor initialization
        if (floors.Count == 0 && floorNumber == 0)
        {
            Debug.Log("INITIALIZE FIRST FLOOR");
            //mapData = GenerateArray(64, 64);
            //mapData = GenerateFloor(mapData);
            //mapData = GenerateCorridors(mapData);
            mapData = CreateFloor();
            floors.Add(mapData);
            currentFloor = 0;
            highestFloor = 0;
        }
        // Generate new floor if it's beyond current count
        else if (floorNumber >= floors.Count)
        {
            Debug.Log("CREATE NEW: " + floorNumber + " FLOOR");
            //mapData = GenerateArray(64, 64);
            //mapData = GenerateFloor(mapData);
            //mapData = GenerateCorridors(mapData);
            mapData = CreateFloor();
            floors.Add(mapData);

            currentFloor = floorNumber;
            highestFloor = Mathf.Max(highestFloor, currentFloor);
        }
        // Load previously saved floor
        else
        {
            Debug.Log("LOAD OLD: " + floorNumber + " FLOOR");
            mapData = floors[floorNumber];
            currentFloor = floorNumber;
        }

        // Load tile palettes
        tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
        Array.Resize(ref tilePallete, 32);
        tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");

        // Render updated floor
        
        floorRenderer.RenderMap(mapData);
    }

}

