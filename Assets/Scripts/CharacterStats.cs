using System;
using UnityEngine;
using static PlayerStats;

[System.Serializable]


public class CharacterStats 
{
    public enum StatType
    {
        MaxHP,
        CurrentHP,
        Strength,
        Perception,
        Endurance,
        Charisma,
        Intelligence,
        Agility,
        Luck,
        Attack,
        Defense,
        Mana,
        MaxMana
    }
    
    [SerializeField] private int MaxHP;
    [SerializeField] private int CurrentHP;
    [SerializeField] private int Strength;
    [SerializeField] private int Perception;
    [SerializeField] private int Endurance;
    [SerializeField] private int Charisma;
    [SerializeField] private int Intelligence;
    [SerializeField] private int Agility;
    [SerializeField] private int Luck;
    [SerializeField] private int Attack;
    [SerializeField] private int Defense;

    [SerializeField] private int Mana;
    [SerializeField] private int MaxMana;
    public bool IsDead => CurrentHP <= 0;

    public void ChangeStat(StatType stat, int amount)
    {
        switch (stat)
        {
            case StatType.MaxHP: MaxHP += amount; break;
            case StatType.CurrentHP: CurrentHP += amount; break;
            case StatType.Strength: Strength += amount; break;
            case StatType.Perception: Perception += amount; break;
            case StatType.Endurance: Endurance += amount; break;
            case StatType.Charisma: Charisma += amount; break;
            case StatType.Intelligence: Intelligence += amount; break;
            case StatType.Agility: Agility += amount; break;
            case StatType.Luck: Luck += amount; break;
            case StatType.Attack: Attack += amount; break;
            case StatType.Defense: Defense += amount; break;
            case StatType.Mana: Mana += amount; break;
            case StatType.MaxMana: MaxMana += amount; break;
            default: Debug.LogWarning("Nieznany stat: " + stat); break;
        }
    }
    public int GetStatValue(StatType stat)
    {
        return stat switch
        {
            StatType.MaxHP => MaxHP,
            StatType.CurrentHP => CurrentHP,
            StatType.Strength => Strength,
            StatType.Perception => Perception,
            StatType.Endurance => Endurance,
            StatType.Charisma => Charisma,
            StatType.Intelligence => Intelligence,
            StatType.Agility => Agility,
            StatType.Luck => Luck,
            StatType.Attack => Attack,
            StatType.Defense => Defense,
            StatType.Mana => Mana,
            StatType.MaxMana => MaxMana,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };


    }

    public void Add(CharacterStats other)
    {
        if (other == null)
        {
            Debug.LogWarning("CharacterStats.Add: other is null, skipping.");
            return;
        }

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            ChangeStat(stat, other.GetStatValue(stat));
        }
    }


}
