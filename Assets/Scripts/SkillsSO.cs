using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skills")]
public class SkillsSO : ScriptableObject, ISkill
{
    public enum damageType {Fire, Water, Earth, Air, Holy, Dark }
    [SerializeField] private string skillName;
    [SerializeField] private int manaCost;
    [SerializeField] private int damage;
    [SerializeField] private damageType damagetype;
    public string SkillName => skillName;
    public int ManaCost => manaCost;

    public virtual void Activate(ICharacter user, ICharacter target) //do zmiany
    {
        Debug.Log($"{user.Name} u¿ywa {skillName} na {target.Name} zadaj¹c {damage} typu {damagetype} (koszt many: {manaCost})");
    }
}
