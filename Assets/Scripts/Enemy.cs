using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    // Przechwyæ wroga
    public string enemyID;
    void Awake()
    {
        if (string.IsNullOrEmpty(enemyID))
            enemyID = gameObject.name; // fallback
    }

    public void Battle()
    {

        
        CharacterHolder enemyHolder = GetComponent<CharacterHolder>();
        if (enemyHolder == null)
        {
            Debug.LogError("Brakuje komponentu CharacterHolder u przeciwnika!");
            return;
        }
        BattleTransferData.enemyData = enemyHolder.characterData;
        BattleTransferData.defeatedEnemyID = enemyID;
        // ZnajdŸ gracza
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Nie znaleziono gracza!");
            return;
        }

        CharacterHolder playerHolder = player.GetComponent<CharacterHolder>();
        if (playerHolder == null)
        {
            Debug.LogError("Gracz nie ma komponentu CharacterHolder!");
            return;
        }

        BattleTransferData.playerData = playerHolder.characterData;
        BattleTransferData.previousSceneName = SceneManager.GetActiveScene().name;
        BattleTransferData.playerPosition = player.transform.position;
        BattleTransferData.cameFromBattle = true;

        SceneManager.LoadScene("BattleScene");
    }
}
