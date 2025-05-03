using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum StatType //enum do trzymania statystyk, ��twiej b�dzie iterowa�
    {
        Si�a,
        Percepcja,
        Wytrzyma�o��,
        Charyzma,
        Inteligencja,
        Zr�czno��,
        Szcz�cie
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
        // Inicjalizacja statystyk, taka pr�bna
        maxHealth = 10 * GetStat(StatType.Wytrzyma�o��);
        currentHealth = maxHealth;
        maxMana = 10 * GetStat(StatType.Inteligencja);
        currentMana = maxMana;
        stats[StatType.Si�a] = 5;
        stats[StatType.Percepcja] = 5;
        stats[StatType.Wytrzyma�o��] = 5;
        stats[StatType.Charyzma] = 5;
        stats[StatType.Inteligencja] = 5;
        stats[StatType.Zr�czno��] = 5;
        stats[StatType.Szcz�cie] = 5;


        //inizjalizacja skilli, te� pr�bna
        skills[SkillType.Fireball] = false;
        skills[SkillType.SwordMastery] = true;
    }

    public float GetStat(StatType stat)
    {
        return stats.ContainsKey(stat) ? stats[stat] : 0; //je�li istnieje zwr��, jak nie to lipa
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
            Debug.Log("Odblokowano now� umiej�tno��: " + skill);
        }
    }


}