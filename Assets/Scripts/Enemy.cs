using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public string enemyID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(enemyID))
            enemyID = gameObject.name;

        CharacterHolder holder = GetComponent<CharacterHolder>();
        if (holder != null && holder.characterInstance != null)
        {
            int floor = 5; // Zak³adamy sta³¹ wartoœæ albo pobran¹ np. z DungeonManager.Instance.GetCurrentFloor()

            var stats = holder.characterInstance.Stats;
            stats.ChangeStat(CharacterStats.StatType.MaxHP, floor * 5);
            stats.ChangeStat(CharacterStats.StatType.CurrentHP, floor * 5);
            stats.ChangeStat(CharacterStats.StatType.Attack, floor * 2);
            stats.ChangeStat(CharacterStats.StatType.Defense, floor);

            Debug.Log($"[Enemy] {name} skalowany do piêtra {floor}. HP: {stats.GetStatValue(CharacterStats.StatType.MaxHP)}");
        }
        else
        {
            Debug.LogWarning("Brak CharacterHolder lub characterInstance – nie mo¿na skalowaæ wroga.");
        }
    }

    public void Battle()
    {
        CharacterHolder enemyHolder = GetComponent<CharacterHolder>();
        if (enemyHolder == null || enemyHolder.characterInstance == null)
        {
            Debug.LogError("Brakuje komponentu CharacterHolder lub instancji postaci u przeciwnika!");
            return;
        }

        // Przekazanie instancji przeciwnika do statycznego obiektu
        BattleTransferData.enemyInstance = enemyHolder.characterInstance;
        BattleTransferData.defeatedEnemyID = enemyID;

        // Gracz
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Nie znaleziono gracza!");
            return;
        }

        CharacterHolder playerHolder = player.GetComponent<CharacterHolder>();
        if (playerHolder == null || playerHolder.characterInstance == null)
        {
            Debug.LogError("Gracz nie ma poprawnie przypisanej instancji postaci!");
            return;
        }

        BattleTransferData.playerInstance = playerHolder.characterInstance;
        BattleTransferData.previousSceneName = SceneManager.GetActiveScene().name;
        BattleTransferData.playerPosition = player.transform.position;
        BattleTransferData.cameFromBattle = true;

        SceneManager.LoadScene("BattleScene");
    }
}
