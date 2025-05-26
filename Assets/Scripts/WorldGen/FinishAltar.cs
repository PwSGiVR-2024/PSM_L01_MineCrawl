using Assets.Scripts.CreateRoom;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FinishAltar : CreateRoom
{
    private void Awake()
    {
        wallsMap = GameObject.Find("Walls")?.GetComponent<Tilemap>();
        print(wallsMap);
        groundMap = GameObject.Find("Ground")?.GetComponent<Tilemap>();
        print(groundMap);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {   
            rooms.Clear();
            //tilePallete.tiles = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
            tilePallete = Resources.LoadAll<TileBase>("Tile Palette/TP Grass");
            //Array.Resize(ref tilePallete.tiles, 32); //include only clean grass
            Array.Resize(ref tilePallete, 32); //include only clean grass
            tilePalleteStone = Resources.LoadAll<TileBase>("Tile Palette/TP Wall");

            mapData = GenerateArray(64, 64);
            mapData = GenerateFloor(mapData);
            mapData = GenerateCorridors(mapData);
            RenderMap(mapData, tile);
        }
    }

}
