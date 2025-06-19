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
    private List<GameObject> spawnedAltars = new List<GameObject>();
    private static int currentFloor;
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

    public void ChangeFloor(FinishAltar altar)
    {
        int targetFloor = currentFloor;

        switch (altar.direction)
        {
            case FinishAltar.Direction.Up:
                targetFloor++;
                break;
            case FinishAltar.Direction.Down:
                targetFloor--;
                break;
            default:
                Debug.LogWarning("Unknown altar direction.");
                return;
        }

        if (targetFloor < 1)
        {
            Debug.Log("Cannot go below 1st floor.");
            return;
        }

        // Save current map data in case it's modified (optional)
        if (floors.Count > currentFloor)
        {
            floors[currentFloor] = mapData;
        }

        // Load or create new floor
        if (targetFloor < floors.Count)
        {
            Debug.Log("LOAD OLD: " + targetFloor + " FLOOR");
            mapData = floors[targetFloor];
        }
        else
        {
            Debug.Log("CREATE NEW: " + targetFloor + " FLOOR");
            mapData = CreateFloor();
            floors.Add(mapData);
        }

        currentFloor = targetFloor;
        highestFloor = Mathf.Max(highestFloor, currentFloor);

        LogManager.Instance.Log($"Player reached floor {currentFloor}.");
        LogManager.Instance.scoreText.text = $"Score: {CharacterInstance.Score}";

        // Reload palettes
        tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
        Array.Resize(ref tilePallete, 32);
        tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");

        floorRenderer.RenderMap(mapData);
        SpawnAltars();
        SpawnPlayer(mapData.altars[0].GetCoords());
    }


    private void SpawnPlayer(Vector2 coords)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(coords.x, coords.y, player.transform.position.z);
    }

    public void SpawnAltars()
    {
        var defaultAltar = GameObject.Find("Altar");
        if(defaultAltar != null)
            spawnedAltars.Add(defaultAltar);

        // Clean up previously spawned altars
        foreach (var altar in spawnedAltars)
        {
            if (altar != null)
                Destroy(altar);
        }
        spawnedAltars.Clear();

        if (mapData.altars == null || mapData.altars.Length != 2)
            mapData.altars = new FinishAltar[2];

        GameObject altarPrefab = Resources.Load<GameObject>("Altar");

        // Reuse coordinates if altars already exist (e.g. loading an old floor)
        if (mapData.altars[0] != null && mapData.altars[1] != null)
        {
            Vector2 backPos = mapData.altars[0].GetCoords();
            Vector2 forwardPos = mapData.altars[1].GetCoords();

            var backAltarObj = Instantiate(altarPrefab, backPos, Quaternion.identity);
            var backAltar = backAltarObj.GetComponent<FinishAltar>();
            backAltar.direction = FinishAltar.Direction.Down;
            backAltarObj.GetComponent<SpriteRenderer>().color = Color.blue;
            backAltar.SetCoords(backPos);
            mapData.altars[0] = backAltar;
            spawnedAltars.Add(backAltarObj);

            var forwardAltarObj = Instantiate(altarPrefab, forwardPos, Quaternion.identity);
            var forwardAltar = forwardAltarObj.GetComponent<FinishAltar>();
            forwardAltar.direction = FinishAltar.Direction.Up;
            forwardAltarObj.GetComponent<SpriteRenderer>().color = Color.green;
            forwardAltar.SetCoords(forwardPos);
            mapData.altars[1] = forwardAltar;
            spawnedAltars.Add(forwardAltarObj);

            lastPlayerPos = Vector2Int.RoundToInt(backPos);
            return;
        }

        // Otherwise, generate new altar positions
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 0; x < mapData.width; x++)
        {
            for (int y = 0; y < mapData.height; y++)
            {
                if (mapData.mapArray[x][y] == -2) // room tile
                    validPositions.Add(new Vector2Int(x, y));
            }
        }

        if (validPositions.Count < 2)
        {
            Debug.LogWarning("Not enough valid positions to place both altars.");
            return;
        }

        // Shuffle and choose 2 positions
        for (int i = 0; i < validPositions.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, validPositions.Count);
            (validPositions[i], validPositions[rnd]) = (validPositions[rnd], validPositions[i]);
        }

        Vector2Int backPosNew = validPositions[0];
        Vector2Int forwardPosNew = validPositions[1];

        var backAltarObjNew = Instantiate(altarPrefab, new Vector3(backPosNew.x, backPosNew.y, 0), Quaternion.identity);
        var backAltarNew = backAltarObjNew.GetComponent<FinishAltar>();
        backAltarNew.direction = FinishAltar.Direction.Down;
        backAltarObjNew.GetComponent<SpriteRenderer>().color = Color.blue;
        backAltarNew.SetCoords(backPosNew);
        mapData.altars[0] = backAltarNew;
        spawnedAltars.Add(backAltarObjNew);

        var forwardAltarObjNew = Instantiate(altarPrefab, new Vector3(forwardPosNew.x, forwardPosNew.y, 0), Quaternion.identity);
        var forwardAltarNew = forwardAltarObjNew.GetComponent<FinishAltar>();
        forwardAltarNew.direction = FinishAltar.Direction.Up;
        forwardAltarObjNew.GetComponent<SpriteRenderer>().color = Color.green;
        forwardAltarNew.SetCoords(forwardPosNew);
        mapData.altars[1] = forwardAltarNew;
        spawnedAltars.Add(forwardAltarObjNew);

        lastPlayerPos = backPosNew;
    }





}

