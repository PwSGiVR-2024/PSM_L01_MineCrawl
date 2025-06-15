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
    [SerializeField] private int hpCost;
    [SerializeField] private float lifeSteal;
    [SerializeField] private damageType damagetype;

    [SerializeField] private SkillCategory category; // ← NOWE POLE

    public string SkillName => skillName;
    public int ManaCost => manaCost;
    public SkillCategory Category => category; // ← GETTER

    public virtual void Activate(ICharacter user, ICharacter target)
    {
        Debug.Log($"{user.Name} uzywa {skillName} na {target.Name} zadając {damage} typu {damagetype} (koszt many: {manaCost})");

        user.SpentMana(manaCost);
        user.TakeDamage(hpCost);
        target.TakeDamage(damage );
        if (lifeSteal>0)
        {
            int stolen = LifeSteal(target.Stats.GetStatValue(CharacterStats.StatType.CurrentHP), lifeSteal);
            user.TakeDamage((int)-stolen);
            Debug.Log(user.Name + " stolen" + stolen);
        }
    }
    public virtual int LifeSteal(int hp, float percent)
    {
        return Mathf.CeilToInt(hp * percent);
    }
}

