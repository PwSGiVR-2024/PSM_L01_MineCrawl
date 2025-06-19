// CharacterInstance.cs
using System.Collections;
using TMPro;
using Unity.VisualScripting;
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
    public static int currentHighestFloor;
    public static int enemiesDefeated;
    private int baseExpRequired = 100;
    private float expMultiplier = 1.5f;
    public bool IsPlayerControlled { get; private set; }
    public void SetAsPlayer() => IsPlayerControlled = true;

    public int CurrentExp => currentExp;

    public CharacterInstance(CharacterSO template)
    {
        characterTemplate = template;
        level = FloorChanger.GetHighestFloor;
        currentExp = 0;
        stats = new CharacterStats();

        InitializeStats();
    }
    static public int Score => CalculateScore();

    private static int CalculateScore()
    {
        // Score roœnie liniowo z iloœci¹ pokonanych przeciwników i piêtrem.
        return FloorChanger.GetHighestFloor * 100 + enemiesDefeated * 50;
    }
    public static void ClearScore()
    {
        enemiesDefeated = 0;
        
    }
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        LogManager.Instance.Log($"{Name} defeated an enemy! Total kills: {enemiesDefeated}");
        
        //LogManager.Instance.scoreText.text = $"Score: {Score}";
    }
    public void SetCurrentFloor(int floor)
    {
        currentHighestFloor = floor;


    }
    public void PrepareForBattle()
    {
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, stats.GetStatValue(CharacterStats.StatType.MaxHP));
        stats.ChangeStat(CharacterStats.StatType.Mana, stats.GetStatValue(CharacterStats.StatType.MaxMana));
    }

    private void InitializeStats()
    {
        stats = new CharacterStats();

        if (Race != null) stats.Add(Race.baseStats);
        if (Class != null) stats.Add(Class.baseStats);
        stats.Add(characterTemplate.baseStats);
        // Skalowanie statystyk z poziomem
        // Skalowanie statystyk przeciwnika przez losowoœæ
        if (characterTemplate != null)
        {
            int floor = FloorChanger.GetHighestFloor;

            int extraStatsToAdd = Random.Range(1, 4); // 1 do 3 statystyk
            var statTypes = System.Enum.GetValues(typeof(CharacterStats.StatType));

            for (int i = 0; i < extraStatsToAdd; i++)
            {
                CharacterStats.StatType randomStat = (CharacterStats.StatType)statTypes.GetValue(Random.Range(0, statTypes.Length));
                int bonus = Random.Range(1, 4) + floor / 2; // Skalowanie z piêtrem
                stats.ChangeStat(randomStat, bonus);

                if (IsPlayerControlled && LogManager.Instance != null)
                    LogManager.Instance.Log($"[DEBUG] Added +{bonus} to {randomStat} due to floor scaling.");


            }
        }

        int endurance = stats.GetStatValue(CharacterStats.StatType.Endurance);
        int intelligence = stats.GetStatValue(CharacterStats.StatType.Intelligence);

        int maxHP = endurance * Race.hpPerEndurance;
        int maxMana = intelligence * Race.manaPerIntelligence;

        stats.ChangeStat(CharacterStats.StatType.MaxHP, maxHP);
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, maxHP);
        stats.ChangeStat(CharacterStats.StatType.MaxMana, maxMana);
        stats.ChangeStat(CharacterStats.StatType.Mana, maxMana);
    }
    public void TakeDamage(int amount)
    {
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, -amount);

        int currentHP = stats.GetStatValue(CharacterStats.StatType.CurrentHP);
        int maxHP = stats.GetStatValue(CharacterStats.StatType.MaxHP);

        if (currentHP > maxHP)
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, maxHP - currentHP);

        //LogManager.Instance.Log($"{Name} took {amount} damage. HP: {Mathf.Max(currentHP, 0)}/{maxHP}");

        if (currentHP <= 0)
        {
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, 0);
            LogManager.Instance.Log($"{Name} has been defeated.");
        }
    }

    public void SpentMana(int amount)
    {
        stats.ChangeStat(CharacterStats.StatType.Mana, -amount);
        int currentMana = stats.GetStatValue(CharacterStats.StatType.Mana);

        if (currentMana < 0)
        {
            int overflow = -currentMana;
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, -overflow);
            stats.ChangeStat(CharacterStats.StatType.Mana, -overflow);
            LogManager.Instance.Log($"{Name} ran out of mana and lost {overflow} HP!");
        }

        int maxMana = stats.GetStatValue(CharacterStats.StatType.MaxMana);
        if (currentMana > maxMana)
        {
            stats.ChangeStat(CharacterStats.StatType.Mana, maxMana - currentMana);
        }
    }

    public void UseSkill(ISkill skill, ICharacter target)
    {
        skill.Activate(this, target);
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        LogManager.Instance.Log($"Gained {amount} EXP. Current: {currentExp}/{GetExpForNextLevel()}");
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
        LogManager.Instance.Log($"LEVEL UP! {Name} reached level {Level}!");
        if (characterTemplate != null)
        {
            int floor = FloorChanger.GetHighestFloor;

            int extraStatsToAdd = Random.Range(1, 4); // 1 do 3 statystyk
            var statTypes = System.Enum.GetValues(typeof(CharacterStats.StatType));

            for (int i = 0; i < extraStatsToAdd; i++)
            {
                CharacterStats.StatType randomStat = (CharacterStats.StatType)statTypes.GetValue(Random.Range(0, statTypes.Length));
                int bonus = Random.Range(1, 4) + floor / 2; // Skalowanie z piêtrem
                stats.ChangeStat(randomStat, bonus);

                if (IsPlayerControlled && LogManager.Instance != null)
                    CharacterHolder.LogStatGainWithDelay( randomStat, bonus);

            }
        }
    }

    public void RestoreStat(CharacterStats.StatType statType, int amount)
    {
        stats.ChangeStat(statType, amount);

        int current = stats.GetStatValue(statType);
        int max = statType switch
        {
            CharacterStats.StatType.CurrentHP => stats.GetStatValue(CharacterStats.StatType.MaxHP),
            CharacterStats.StatType.Mana => stats.GetStatValue(CharacterStats.StatType.MaxMana),
            _ => int.MaxValue
        };

        if (current > max)
        {
            stats.ChangeStat(statType, max - current);
        }
    }
}
