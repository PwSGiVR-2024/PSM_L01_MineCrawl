using System.Collections.Generic;
using UnityEngine;

public enum SkillCategory
{
    Physical,
    Magic,
    Item
}
public enum ScalingStatType
{
    None,
    Strength,
    Intelligence,
    Endurance,
    Agility,
    Luck,
    Charisma,
    Perception,
    Attack,
    Defense
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
    [Header("Scaling")]
    [SerializeField] private ScalingStatType scalingStat = ScalingStatType.None;

    public ScalingStatType ScalingStat => scalingStat;


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
        {
            user.SpentMana(manaCost);
            LogManager.Instance.Log($"{user.Name} spends {manaCost} mana. MP: {user.Stats.GetStatValue(CharacterStats.StatType.Mana)}/{user.Stats.GetStatValue(CharacterStats.StatType.MaxMana)}");
        }
        else if (hpCost > 0)
        {
            user.TakeDamage(hpCost);
            LogManager.Instance.Log($"{user.Name} sacrifices {hpCost} HP. HP:{user.Stats.GetStatValue(CharacterStats.StatType.CurrentHP)}/{user.Stats.GetStatValue(CharacterStats.StatType.MaxHP)}");
        }
        else
        {
            user.SpentMana(manaCost);
            user.TakeDamage(hpCost);
        }


        // Efekty najpierw, potem logi o efektach
        foreach (var effect in effects)
        {
            switch (effect)
            {
                case SkillEffectType.Damage:
                    
                    int targetDefense = target.Stats.GetStatValue(CharacterStats.StatType.Defense);
                    int baseAttack = user.Stats.GetStatValue(CharacterStats.StatType.Attack) + damage;
                    int scalingBonus = Mathf.RoundToInt(baseAttack * (baseAttack/100));
                    int finalDamage = Mathf.Max((scalingBonus + baseAttack) - targetDefense, 1);


                    target.TakeDamage(finalDamage);
                    LogManager.Instance.Log($"{target.Name} takes {finalDamage} damage. (Base: {damage}, Bonus: {scalingBonus}, Reduced: {targetDefense})");
                    if (lifeSteal > 0 && effects.Contains(SkillEffectType.Damage))
                    {
                        int stolen = LifeSteal(finalDamage, lifeSteal);
                        user.TakeDamage(-stolen);
                        LogManager.Instance.Log($"{user.Name} steals {stolen} HP from {target.Name}.");
                    }
                    break;

                case SkillEffectType.Heal:
                    int healBonus = Mathf.RoundToInt(GetScalingStatValue(user.Stats) * (healingAmount/100));
                    int totalHeal = healingAmount + healBonus;

                    target.TakeDamage(-totalHeal);
                    LogManager.Instance.Log($"{target.Name} is healed for {totalHeal} HP. (Base: {healingAmount}, Bonus: {healBonus})");
                    break;

                case SkillEffectType.ManaRegen:
                    int scalingManaBonus = Mathf.RoundToInt(GetScalingStatValue(user.Stats) * (manaRegenAmount/100));
                    int totalManaRegen = manaRegenAmount + scalingManaBonus;

                    target.Stats.ChangeStat(CharacterStats.StatType.Mana, totalManaRegen);
                    LogManager.Instance.Log($"{target.Name} regenerates {totalManaRegen} mana. (Base: {manaRegenAmount}, Bonus: {scalingManaBonus})");
                    break;

                case SkillEffectType.Buff:
                    LogManager.Instance.Log($"{target.Name} is empowered by a buff. (Effect TBD)"); //WIP
                    break;

                case SkillEffectType.Debuff:
                    LogManager.Instance.Log($"{target.Name} suffers a debuff. (Effect TBD)"); //WIP
                    break;
            }
        }


    }

    public int LifeSteal(int baseDamage, float percent)
    {
        return Mathf.CeilToInt(baseDamage * percent);
    }
    private int GetScalingStatValue(CharacterStats stats)
    {
        return scalingStat switch
        {
            ScalingStatType.Strength => stats.GetStatValue(CharacterStats.StatType.Strength),
            ScalingStatType.Intelligence => stats.GetStatValue(CharacterStats.StatType.Intelligence),
            ScalingStatType.Endurance => stats.GetStatValue(CharacterStats.StatType.Endurance),
            ScalingStatType.Agility => stats.GetStatValue(CharacterStats.StatType.Agility),
            ScalingStatType.Luck => stats.GetStatValue(CharacterStats.StatType.Luck),
            ScalingStatType.Charisma => stats.GetStatValue(CharacterStats.StatType.Charisma),
            ScalingStatType.Perception => stats.GetStatValue(CharacterStats.StatType.Perception),
            ScalingStatType.Attack => stats.GetStatValue(CharacterStats.StatType.Attack),
            ScalingStatType.Defense => stats.GetStatValue(CharacterStats.StatType.Defense),
            _ => 0
        };
    }

}
