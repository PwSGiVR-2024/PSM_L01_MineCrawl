using System.Collections.Generic;
using UnityEngine;

public enum SkillCategory
{
    Physical,
    Magic,
    Item
}

public enum SkillEffectType
{
    Damage,
    Heal,
    ManaRegen,
    Buff,
    Debuff
}
public enum ParticleEffectType
{
    
    OnTarget,
    Projectile,
    SelfCast
}
[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skills")]
public class SkillsSO : ScriptableObject, ISkill
{
    public enum damageType { Fire, Water, Earth, Air, Holy, Dark, Physical }

    [SerializeField] private string skillName;
    [SerializeField] private string description;
    [SerializeField] private SkillCategory category;
    [SerializeField] private damageType damagetype;
    [SerializeField] private bool useOnSelf;

    [Header("Costs")]
    [SerializeField] private int manaCost;
    [SerializeField] private int hpCost;

    [Header("Effects")]
    [SerializeField] private List<SkillEffectType> effects = new List<SkillEffectType>();
    [SerializeField] private int damage;
    [SerializeField] private int healingAmount;
    [SerializeField] private int manaRegenAmount;
    [SerializeField] private float lifeSteal;

    [Header("Visual")]
    [SerializeField] private GameObject visualEffectPrefab;
    public GameObject VisualEffectPrefab => visualEffectPrefab;
    [SerializeField] private ParticleEffectType particleEffectType;
    public ParticleEffectType EffectType => particleEffectType;
    public string SkillName => skillName;
    public string Description => description;
    public SkillCategory Category => category;
    public int ManaCost => manaCost;
    public int Damage => damage;
    public bool selfUse => useOnSelf;
    public List<SkillEffectType> Effects => effects;
 


    public virtual void Activate(ICharacter user, ICharacter target)
    {
        Debug.Log($"{user.Name} używa {skillName} na {target.Name}");

        user.SpentMana(manaCost);
        user.TakeDamage(hpCost);

        foreach (var effect in effects)
        {
            switch (effect)
            {
                case SkillEffectType.Damage:
                    target.TakeDamage(damage);
                    break;
                case SkillEffectType.Heal:
                    target.TakeDamage(-healingAmount);
                    break;
                case SkillEffectType.ManaRegen:
                    target.Stats.ChangeStat(CharacterStats.StatType.Mana, manaRegenAmount);
                    break;
                // Tu można dodać Buffy, Debuffy, itd. ale to może kiedyś jak będzie czas :P
            }
        }

        if (lifeSteal > 0)
        {
            int stolen = LifeSteal(damage, lifeSteal);
            user.TakeDamage(-stolen);
            Debug.Log($"{user.Name} ukradł {stolen} HP");
        }
    }

    public int LifeSteal(int baseDamage, float percent)
    {
        return Mathf.CeilToInt(baseDamage * percent);
    }
}
