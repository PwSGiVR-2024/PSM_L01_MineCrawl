using UnityEngine;

public class CharacterInstance : ICharacter
{
    public string Name => characterTemplate.characterName;
    public int Level => level;
    public CharacterStats Stats => stats;
    public CharacterRaceSO Race => characterTemplate.race;
    public CharacterClassSO Class => characterTemplate.characterClass;

    private CharacterSO characterTemplate;
    private int level;
    private int currentExp;
    private CharacterStats stats;

    public int CurrentExp => currentExp;
    private int baseExpRequired = 100;
    private float expMultiplier = 1.5f;

    public CharacterInstance(CharacterSO template)
    {
        characterTemplate = template;
        level = template.baseLevel;
        currentExp = 0;
        stats = new CharacterStats();

        InitializeStats();
    }

    private void InitializeStats()
    {
        stats = new CharacterStats();

        if (Race != null) stats.Add(Race.baseStats);
        if (Class != null) stats.Add(Class.baseStats);
        stats.Add(characterTemplate.baseStats);

        int endurance = stats.GetStatValue(CharacterStats.StatType.Endurance);
        int intelligence = stats.GetStatValue(CharacterStats.StatType.Intelligence);

        stats.ChangeStat(CharacterStats.StatType.MaxHP, endurance * Race.hpPerEndurance);
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, endurance * Race.hpPerEndurance);
        stats.ChangeStat(CharacterStats.StatType.MaxMana, intelligence * Race.manaPerIntelligence);
        stats.ChangeStat(CharacterStats.StatType.Mana, intelligence * Race.manaPerIntelligence);
    }

public void TakeDamage(int amount)
    {
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, -amount);
        Debug.Log($"{Name} otrzyma³ {amount} obra¿eñ. Aktualne HP: {stats.GetStatValue(CharacterStats.StatType.CurrentHP)}");

        if (stats.GetStatValue(CharacterStats.StatType.CurrentHP) <= 0)
        {
            Debug.Log($"{Name} umiera.");
        }

        if (stats.GetStatValue(CharacterStats.StatType.CurrentHP) > stats.GetStatValue(CharacterStats.StatType.MaxHP))
        {
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, stats.GetStatValue(CharacterStats.StatType.MaxHP) - stats.GetStatValue(CharacterStats.StatType.CurrentHP));
        }
    }

    public void UseSkill(ISkill skill, ICharacter target)
    {
        skill.Activate(this, target);
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"Zdobyto {amount} EXP. Aktualne: {currentExp}/{GetExpForNextLevel()}");
        CheckLevelUp();
    }

    public int GetExpForNextLevel()
    {
        return Mathf.FloorToInt(baseExpRequired * Mathf.Pow(expMultiplier, level - 1));
    }

    private void CheckLevelUp()
    {
        while (currentExp >= GetExpForNextLevel())
        {
            currentExp -= GetExpForNextLevel();
            level++;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Debug.Log($"Level UP! Nowy poziom: {Level}");
        stats.ChangeStat(CharacterStats.StatType.MaxHP, 10);
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, 10);
        stats.ChangeStat(CharacterStats.StatType.Strength, 2);
        stats.ChangeStat(CharacterStats.StatType.Defense, 2);
        stats.ChangeStat(CharacterStats.StatType.Agility, 1);
    }

    public void SpentMana(int amount)
    {
        stats.ChangeStat(CharacterStats.StatType.Mana, -amount);
        if (stats.GetStatValue(CharacterStats.StatType.Mana) <= 0)
        {
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, stats.GetStatValue(CharacterStats.StatType.Mana)); // ujemne mana odbiera HP
            stats.ChangeStat(CharacterStats.StatType.Mana, 0);
        }

        if (stats.GetStatValue(CharacterStats.StatType.Mana) > stats.GetStatValue(CharacterStats.StatType.MaxMana))
        {
            stats.ChangeStat(CharacterStats.StatType.Mana, stats.GetStatValue(CharacterStats.StatType.MaxMana) - stats.GetStatValue(CharacterStats.StatType.Mana));
        }
    }
}
