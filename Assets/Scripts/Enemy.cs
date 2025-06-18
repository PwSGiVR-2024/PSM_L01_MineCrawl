using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public string enemyID;
    public CharacterInstance characterInstance;

    public void Initialize(CharacterInstance instance)
    {
        characterInstance = instance;
        if (string.IsNullOrEmpty(enemyID))
            enemyID = characterInstance.Name;

        ScaleStatsByFloor(FloorChanger.GetHighestFloor); // Przyk³ad skalowania statystyk

        // Mo¿esz tutaj ustawiæ np. UI lub inne rzeczy powi¹zane z przeciwnikiem
    }

    private void ScaleStatsByFloor(int floor)
    {
        var stats = characterInstance.Stats;
        stats.ChangeStat(CharacterStats.StatType.MaxHP, floor * 5);
        stats.ChangeStat(CharacterStats.StatType.CurrentHP, floor * 5);
        stats.ChangeStat(CharacterStats.StatType.Attack, floor * 2);
        stats.ChangeStat(CharacterStats.StatType.Defense, floor);
    }

    public void Battle()
    {
        if (characterInstance == null)
        {
            Debug.LogError("Brakuje instancji postaci wroga!");
            return;
        }

        BattleTransferData.enemyInstance = characterInstance;
        BattleTransferData.defeatedEnemyID = enemyID;

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
