using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePalette", menuName = "Scriptable Objects/TilePalette")]
public class TilePallete : ScriptableObject
{
    public TileBase[] tiles;
    private void OnEnable()
    {
        //tiles = Resources.LoadAll<Tile>("Tile Pallete/TP Grass");
    }
}
