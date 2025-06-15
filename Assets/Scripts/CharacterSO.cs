using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "RPG/Character")]
public class CharacterSO : ScriptableObject, ICharacter
{
    [SerializeField] public string characterName;
    [SerializeField] private int level;
    [SerializeField] private CharacterStats stats;
    [SerializeField] public CharacterRaceSO race;
    [SerializeField] public CharacterClassSO characterClass;

    public string Name => characterName;
    public int Level => level;
    public CharacterStats Stats => stats;
    public CharacterRaceSO Race => race;
    public CharacterClassSO Class => characterClass;

    public void TakeDamage(int amount)
    {
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, -amount);
        Debug.Log($"{Name} otrzyma³ {amount} obra¿eñ. Aktualne HP: {stats.GetStatValue(CharacterStats.StatType.CurrentHP)}"); 
        if (stats.GetStatValue(CharacterStats.StatType.CurrentHP) <= 0)
        {
            Debug.Log($"{Name} umiera.");
            // œmieræ postaci...
        }
        if(stats.GetStatValue(CharacterStats.StatType.CurrentHP ) > stats.GetStatValue(CharacterStats.StatType.MaxHP))
        {
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, -stats.GetStatValue(CharacterStats.StatType.CurrentHP));
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, stats.GetStatValue(CharacterStats.StatType.MaxHP));
        }
    }


    public void UseSkill(ISkill skill, ICharacter target)
    {
        skill.Activate(this, target);
    }

    public void GainExperience(int amount)
    {
        // PLACEHOLDER
        Debug.Log($"{Name} zyska³ {amount} expa.");
    }
    public void InitializeStats()
    {
        stats = new CharacterStats();

        if (race != null) stats.Add(race.baseStats);
        if (characterClass != null) stats.Add(characterClass.baseStats);

        int endurance = stats.GetStatValue(CharacterStats.StatType.Endurance);
        int intelligence = stats.GetStatValue(CharacterStats.StatType.Intelligence);

        stats.ChangeStat(CharacterStats.StatType.MaxHP, endurance * race.hpPerEndurance);
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, endurance * race.hpPerEndurance);
        stats.ChangeStat(CharacterStats.StatType.MaxMana, intelligence * race.manaPerIntelligence);
        stats.ChangeStat(CharacterStats.StatType.Mana, intelligence * race.manaPerIntelligence);
    }

    public void SpentMana(int amount)
    {
        stats.ChangeStat(CharacterStats.StatType.Mana, -amount);
        if (stats.GetStatValue(CharacterStats.StatType.Mana) <= 0)
        {
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, 0 + stats.GetStatValue(CharacterStats.StatType.Mana));
            stats.ChangeStat(CharacterStats.StatType.Mana, 0);
        }
        if (stats.GetStatValue(CharacterStats.StatType.Mana) > stats.GetStatValue(CharacterStats.StatType.MaxMana))
        {
            stats.ChangeStat(CharacterStats.StatType.Mana, stats.GetStatValue(CharacterStats.StatType.MaxMana));
        }
    }
}
