using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //na przysz³oœæ gdy dodani bêd¹ przeciwnicy na mapie
    public CharacterRaceSO[] availableRaces;
    public CharacterClassSO[] availableClasses;
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    public void SpawnRandomEnemy()
    {
        CharacterBuilder builder = new CharacterBuilder();
        (availableRaces, availableClasses) = builder.CheckAvailable();
        CharacterInstance enemyInstance = EnemyFactory.CreateRandomEnemy(availableRaces, availableClasses);

        if (enemyInstance != null)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Enemy enemyComp = enemyGO.GetComponent<Enemy>();
            if (enemyComp != null)
            {
                enemyComp.Initialize(enemyInstance);
                Debug.Log($"Wylosowano wroga: {enemyInstance.Name}");
            }
            else
            {
                Debug.LogError("Prefab wroga nie ma komponentu Enemy!");
            }
        }
    }
}
