using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "RPG/Character")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public int baseLevel = 1;
    public CharacterStats baseStats;
    public CharacterRaceSO race;
    public CharacterClassSO characterClass;
}
