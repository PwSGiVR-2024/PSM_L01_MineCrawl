using Assets.Scripts.CreateRoom;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorChanger : CreateRoom
{
    public GameObject nextFloorAltarPrefab;

    private CreateRoom floorData;
    private Room altarSpawnRoom;
    private Vector2 altarSpawnPoint;
    private List<MapData> floors;
    private FloorRenderer floorRenderer;
    int currentFloor;

    private void Start()
    {
        floorRenderer = GetComponent<FloorRenderer>();
        floorData = GetComponent<CreateRoom>();
        currentFloor = 0;
        floors = new List<MapData>();
    }

    void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            altarSpawnPoint = GetSpawnPoint();
            float distanceToAltarSpawn = Vector3.Distance(player.transform.position, altarSpawnPoint);
            float allowedDistance = 2.0f;

            while(distanceToAltarSpawn <= allowedDistance)
            {
                altarSpawnPoint = GetSpawnPoint();
                distanceToAltarSpawn = Vector3.Distance(player.transform.position, altarSpawnPoint);
            }
            print(altarSpawnPoint);
            if (GameObject.FindGameObjectWithTag("Finish") == null)
            {
                Instantiate(nextFloorAltarPrefab, altarSpawnPoint, Quaternion.identity);
            }
            else
            {
                Debug.Log("Prefab już istnieje na scenie.");
            }
        }
        //test

        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    if (currentFloor < floors.Count - 1)
        //    {
        //        currentFloor++;
        //        mapData = floors[currentFloor];
        //        floorRenderer.RenderMap(mapData);
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    if (currentFloor > 0)
        //    {
        //        currentFloor--;
        //        mapData = floors[currentFloor];
        //        floorRenderer.RenderMap(mapData);
        //    }
        //}
    }

    private Vector2 GetSpawnPoint()
    {
        altarSpawnRoom = floorData.GetRandomRoom();
        altarSpawnPoint = altarSpawnRoom.rootCoords;
        altarSpawnPoint.x += altarSpawnRoom.width / 2;
        altarSpawnPoint.y += altarSpawnRoom.height / 2;

        return altarSpawnPoint;
    }

    public MapData ChangeFloor(int floorNumber)
    {
        if(currentFloor == 0 && floors.Count == 0)
        {
            print("FIRST FLOOR ADD");
            floors.Add(mapData);
        }
        else
        {
            if (floorNumber == floors.Count) //generate new
            {
                print("CREATE NEW: " + floorNumber + " FLOOR");
                mapData = GenerateArray(64, 64);
                mapData = GenerateFloor(mapData);
                mapData = GenerateCorridors(mapData);
                floors.Add(mapData);
            }
            else //load old
            {
                print("LOAD OLD: " + floorNumber + " FLOOR");
                mapData = floors[floorNumber];
            }
        }

        tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
        Array.Resize(ref tilePallete, 32); //include only clean grass
        tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");

        floorRenderer.RenderMap(mapData);

        return mapData;
    }
}
