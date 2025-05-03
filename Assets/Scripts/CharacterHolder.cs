using UnityEngine;

public class CharacterHolder : MonoBehaviour
{
    public CharacterSO characterData; 
    public void Start()
    {
        characterData.InitializeStats();
    }
}