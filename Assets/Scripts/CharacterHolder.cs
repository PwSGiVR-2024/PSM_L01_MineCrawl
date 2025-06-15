using UnityEngine;

public class CharacterHolder : MonoBehaviour
{
    public CharacterSO characterData;
    public CharacterInstance characterInstance;

    public void Start()
    {
        if (characterInstance == null && characterData != null)
        {
            characterInstance = new CharacterInstance(characterData);
        }
    }

}
