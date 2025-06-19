using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCreationData", menuName = "Game/CharacterCreationData")]
public class CharacterCreationData : ScriptableObject
{
    public string name;
    public CharacterRaceSO selectedRace;
    public CharacterClassSO selectedClass;

    public void Clear()
    {
        selectedRace = null;
        selectedClass = null;
    }
}
