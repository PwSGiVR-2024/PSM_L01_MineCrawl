using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePalette", menuName = "Scriptable Objects/TilePalette")]
public class TilePalette : ScriptableObject
{
    public Tile[] tiles;
    private void OnEnable()
    {
        tiles = Resources.LoadAll<Tile>("Tile Pallete/TP Grass");
    }
}
