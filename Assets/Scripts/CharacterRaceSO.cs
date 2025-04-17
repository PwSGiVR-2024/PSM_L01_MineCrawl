using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterRace", menuName = "RPG/CharacterRace")]
public class CharacterRaceSO : ScriptableObject
{
    public string raceName;
    public CharacterStats baseStats;
    public List<SkillsSO> startingSkills;
    public int hpPerEndurance = 10;
    public int manaPerIntelligence = 5;

}
