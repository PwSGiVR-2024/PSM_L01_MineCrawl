using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleStatsDisplay : MonoBehaviour
{
    public GameObject statsPanel;
    public Transform contentParent;
    public GameObject statTextPrefab;

    private CharacterInstance player => BattleTransferData.playerInstance;
    private CharacterInstance enemy => BattleTransferData.enemyInstance;

    private bool isVisible = false;
    private bool showEnemy = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isVisible = !isVisible;
            statsPanel.SetActive(isVisible);

            if (isVisible)
                ShowStats();
            else
                ClearDisplay();
        }

        if (isVisible && Input.GetKeyDown(KeyCode.Tab))
        {
            showEnemy = !showEnemy;
            ShowStats();
        }
    }

    void ShowStats()
    {
        ClearDisplay();
        CharacterInstance target = showEnemy ? enemy : player;
        if (target == null) return;

        string who = showEnemy ? "[ENEMY]" : "[PLAYER]";
        AddText($"{who}");
        AddText($"Name: {target.Name}");
        AddText($"Level: {target.Level}");
        AddText($"EXP: {target.CurrentExp}/{target.GetExpForNextLevel()}");
        AddText($"Race: {target.Race.name}");
        AddText($"Class: {target.Class.name}");

        foreach (CharacterStats.StatType stat in System.Enum.GetValues(typeof(CharacterStats.StatType)))
        {
            int val = target.Stats.GetStatValue(stat);
            AddText($"{stat}: {val}");
        }
    }

    void AddText(string text)
    {
        GameObject go = Instantiate(statTextPrefab, contentParent);
        TMP_Text tmp = go.GetComponent<TMP_Text>();
        if (tmp != null)
            tmp.text = text;
    }

    void ClearDisplay()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }
}
