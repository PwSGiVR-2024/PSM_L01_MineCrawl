using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterClass", menuName = "RPG/CharacterClass")]
public class CharacterClassSO : ScriptableObject
{
    public string className;
    public CharacterStats baseStats;
    public List<SkillsSO> startingSkills;

}
