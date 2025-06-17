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
        // Najpierw log użycia skilla
        LogManager.Instance.Log($"{user.Name} uses {skillName} on {target.Name}.");

        if (manaCost > 0)
            LogManager.Instance.Log($"{user.Name} spends {manaCost} mana.");
        if (hpCost > 0)
            LogManager.Instance.Log($"{user.Name} sacrifices {hpCost} HP.");

        user.SpentMana(manaCost);
        user.TakeDamage(hpCost);

        // Efekty najpierw, potem logi o efektach
        foreach (var effect in effects)
        {
            switch (effect)
            {
                case SkillEffectType.Damage:
                    target.TakeDamage(damage);
                    LogManager.Instance.Log($"{target.Name} takes {damage} {damagetype} damage.");
                    break;

                case SkillEffectType.Heal:
                    target.TakeDamage(-healingAmount);
                    LogManager.Instance.Log($"{target.Name} is healed for {healingAmount} HP.");
                    break;

                case SkillEffectType.ManaRegen:
                    target.Stats.ChangeStat(CharacterStats.StatType.Mana, manaRegenAmount);
                    LogManager.Instance.Log($"{target.Name} regenerates {manaRegenAmount} mana.");
                    break;

                case SkillEffectType.Buff:
                    LogManager.Instance.Log($"{target.Name} is empowered by a buff. (Effect TBD)");
                    break;

                case SkillEffectType.Debuff:
                    LogManager.Instance.Log($"{target.Name} suffers a debuff. (Effect TBD)");
                    break;
            }
        }

        if (lifeSteal > 0 && effects.Contains(SkillEffectType.Damage))
        {
            int stolen = LifeSteal(damage, lifeSteal);
            user.TakeDamage(-stolen);
            LogManager.Instance.Log($"{user.Name} steals {stolen} HP from {target.Name}.");
        }
    }

    public int LifeSteal(int baseDamage, float percent)
    {
        return Mathf.CeilToInt(baseDamage * percent);
    }
}
