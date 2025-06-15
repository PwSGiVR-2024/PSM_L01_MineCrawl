using UnityEngine;

public static class EnemyFactory
{
    //na przysz³oœæ gdy dodani bêd¹ przeciwnicy na mapie
    public static CharacterInstance CreateRandomEnemy(CharacterRaceSO[] availableRaces, CharacterClassSO[] availableClasses)
    {
        if (availableRaces == null || availableRaces.Length == 0 || availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogError("Brak dostêpnych ras lub klas dla wroga.");
            return null;
        }

        // Losowanie rasy i klasy
        CharacterRaceSO race = availableRaces[Random.Range(0, availableRaces.Length)];
        CharacterClassSO cls = availableClasses[Random.Range(0, availableClasses.Length)];

      
        CharacterSO enemyTemplate = ScriptableObject.CreateInstance<CharacterSO>();
        enemyTemplate.characterName = $"{race.raceName} {cls.className}";
        enemyTemplate.race = race;
        enemyTemplate.characterClass = cls;
        enemyTemplate.baseLevel = 1;
        enemyTemplate.baseStats = new CharacterStats(); 

        // Tworzymy instancjê postaci wroga
        CharacterInstance enemyInstance = new CharacterInstance(enemyTemplate);
        return enemyInstance;
    }
}
