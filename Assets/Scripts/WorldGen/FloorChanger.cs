using Assets.Scripts.CreateRoom;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;

/*
Keeps info about floors in List<MapData>
Responsible for creation and management of FloorAltars, i.e.:
    - where they are on the map
    - which way do they go
*/
public class FloorChanger : FloorCreator
{
    public GameObject nextFloorAltarPrefab;

    private FloorCreator floorData;
    private FloorRenderer floorRenderer;
    private List<MapData> floors;
    private int currentFloor;
    private static int highestFloor;
    public static int GetHighestFloor => highestFloor;
    private void Start()
    {
        floorRenderer = GetComponent<FloorRenderer>();
        floorData = GetComponent<FloorCreator>();
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

    public void ChangeFloor(FinishAltar altar)
    {
        int targetFloor = currentFloor;

        // Determine new target floor
        switch (altar.direction)
        {
            case FinishAltar.Direction.Up:
                targetFloor = currentFloor + 1;
                break;
            case FinishAltar.Direction.Down:
                targetFloor = currentFloor - 1;
                break;
            default:
                Debug.LogWarning("Unknown altar direction.");
                return;
        }

        // Prevent going below 0th floor
        if (targetFloor < 0)
        {
            Debug.Log("Cannot go below 0th floor.");
            return;
        }

        // Save current floor state
        if (floors.Count > 0)
        {
            floors[currentFloor] = mapData;
        }


        // Load or generate floor
        if (floors.Count == 0 && targetFloor == 0)
        {
            Debug.Log("INITIALIZE FIRST FLOOR");
            mapData = CreateFloor();
            floors.Add(mapData);
        }
        else if (targetFloor >= floors.Count)
        {
            Debug.Log("CREATE NEW: " + targetFloor + " FLOOR");
            mapData = CreateFloor();
            floors.Add(mapData);
        }
        else
        {
            Debug.Log("LOAD OLD: " + targetFloor + " FLOOR");
            mapData = floors[targetFloor];
        }
        // Update floor index
        currentFloor = targetFloor;
        highestFloor = Mathf.Max(highestFloor, currentFloor);

        if (LogManager.Instance != null)
        {
            Tutorial.Instance.clear(); 

            LogManager.Instance.Log($"Player reached floor {currentFloor}.");

            if (LogManager.Instance.scoreText != null)
                LogManager.Instance.scoreText.text = $"Score: {CharacterInstance.Score}";
            else
                Debug.LogWarning("LogManager scoreText is null.");
        }
        else
        {
            Debug.LogWarning("LogManager.Instance is null.");
        }

        // Load tile palettes
        tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
        Array.Resize(ref tilePallete, 32);
        tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");

        // Render updated floor
        floorRenderer.RenderMap(mapData);

        // ⬇️ Only spawn altars if new floor was created
        SpawnAltars();
    }

    public void SpawnAltars()
    {
        if (mapData.altars == null || mapData.altars.Length != 2)
            mapData.altars = new FinishAltar[2];

        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 0; x < mapData.width; x++)
        {
            for (int y = 0; y < mapData.height; y++)
            {
                if (mapData.mapArray[x][y] == -2) // room tile
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (validPositions.Count < 2)
        {
            Debug.LogWarning("Not enough valid positions to place both altars.");
            return;
        }

        // Shuffle valid positions to pick randomly
        for (int i = 0; i < validPositions.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, validPositions.Count);
            (validPositions[i], validPositions[rnd]) = (validPositions[rnd], validPositions[i]);
        }

        // Assign altar positions
        Vector2Int backPos = validPositions[0];
        Vector2Int forwardPos = validPositions[1];

        GameObject altarPrefab = Resources.Load<GameObject>("Altar"); // must be under a Resources folder

        // Blue back altar
        GameObject backAltarObj = Instantiate(altarPrefab, new Vector3(backPos.x, backPos.y, 0), Quaternion.identity);
        FinishAltar backAltar = backAltarObj.GetComponent<FinishAltar>();
        backAltar.direction = FinishAltar.Direction.Down;
        //backAltarObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/BlueAltar"); // adjust path
        backAltarObj.GetComponent<SpriteRenderer>().color = Color.blue;
        mapData.altars[0] = backAltar;

        // Green forward altar
        GameObject forwardAltarObj = Instantiate(altarPrefab, new Vector3(forwardPos.x, forwardPos.y, 0), Quaternion.identity);
        FinishAltar forwardAltar = forwardAltarObj.GetComponent<FinishAltar>();
        forwardAltar.direction = FinishAltar.Direction.Up;
        //forwardAltarObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/GreenAltar"); // adjust path\
        forwardAltarObj.GetComponent<SpriteRenderer>().color = Color.green;
        mapData.altars[1] = forwardAltar;

        // Update spawn position
        lastPlayerPos = backPos;
    }




}

