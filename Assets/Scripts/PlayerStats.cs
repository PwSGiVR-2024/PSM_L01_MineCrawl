using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum StatType //enum do trzymania statystyk, ³¹twiej bêdzie iterowaæ
    {
        Sila,
        Percepcja,
        Wytrzymalosc,
        Charyzma,
        Inteligencja,
        Zrecznosc,
        Szczescie
    }

    public enum SkillType
    {
        Fireball,
        Stealth,
        SwordMastery,
        SpearMastery,
        ShieldMastery,
        Lockpicking
    }


    public float maxHealth, maxMana;
    private float currentHealth, currentMana;

    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;

    public Dictionary<SkillType, bool> skills = new Dictionary<SkillType, bool>();
    public Dictionary<StatType, float> stats = new Dictionary<StatType, float>();

    void Start()
    {
        // Inicjalizacja statystyk, taka próbna
        maxHealth = 10 * GetStat(StatType.Wytrzymalosc);
        currentHealth = maxHealth;
        maxMana = 10 * GetStat(StatType.Inteligencja);
        currentMana = maxMana;
        stats[StatType.Sila] = 5;
        stats[StatType.Percepcja] = 5;
        stats[StatType.Wytrzymalosc] = 5;
        stats[StatType.Charyzma] = 5;
        stats[StatType.Inteligencja] = 5;
        stats[StatType.Zrecznosc] = 5;
        stats[StatType.Szczescie] = 5;


        //inizjalizacja skilli, te¿ próbna
        skills[SkillType.Fireball] = false;
        skills[SkillType.SwordMastery] = true;
    }

    public float GetStat(StatType stat)
    {
        return stats.ContainsKey(stat) ? stats[stat] : 0; //jeœli istnieje zwróæ, jak nie to lipa
    }

    public void ModifyStat(StatType stat, float amount)
    {
        if (stats.ContainsKey(stat))
        {
            stats[stat] += amount;
        }
    }
    public void GainExperience(int amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);
        Debug.Log("Level Up! Nowy poziom: " + level);
    }


    public void UnlockSkill(SkillType skill)
    {
        if (!skills.ContainsKey(skill))
        {
            skills[skill] = true;
            Debug.Log("Odblokowano now¹ umiejêtnoœæ: " + skill);
        }
    }


}