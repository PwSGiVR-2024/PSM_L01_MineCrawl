using System.Collections.Generic;
using UnityEngine;
public enum SkillCategory
{
    Physical,
    Magic,
    Item
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skills")]
public class SkillsSO : ScriptableObject, ISkill
{
    public enum damageType { Fire, Water, Earth, Air, Holy, Dark, Physical }

    [SerializeField] private string skillName;
    [SerializeField] private int manaCost;
    [SerializeField] private int damage;
    [SerializeField] private damageType damagetype;

    [SerializeField] private SkillCategory category; // ← NOWE POLE

    public string SkillName => skillName;
    public int ManaCost => manaCost;
    public SkillCategory Category => category; // ← GETTER

    public virtual void Activate(ICharacter user, ICharacter target)
    {
        Debug.Log($"{user.Name} uzywa {skillName} na {target.Name} zadając {damage} typu {damagetype} (koszt many: {manaCost})");
        target.TakeDamage(damage );
    }
}

