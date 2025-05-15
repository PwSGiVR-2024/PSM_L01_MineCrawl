using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public void StartBattle()
    {
        Debug.Log("Walka si� zaczyna!");

        CharacterHolder enemyHolder = GetComponent<CharacterHolder>();
        if (enemyHolder == null)
        {
            Debug.LogError("Brakuje komponentu CharacterHolder u przeciwnika!");
            return;
        }

        BattleTransferData.enemyData = enemyHolder.characterData;

        // Zak�adamy, �e gracz ma sw�j GameObject z CharacterHolder i tag "Player"
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

        SceneManager.LoadScene("BattleScene");
    }
}
