using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerStatsDisplay : MonoBehaviour
{
    public GameObject scrollView;
    public Transform contentParent;
    public GameObject statTextPrefab;    // Prefab z TextMeshProUGUI

    public CharacterHolder holder;
    private CharacterInstance player;

    private bool isVisible = false;
    private bool showSkillsInsteadOfStats = false;

    private void Start()
    {
        player = holder.characterInstance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            showSkillsInsteadOfStats = false;
            ToggleDisplay();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            showSkillsInsteadOfStats = true;
            ToggleDisplay();
        }
    }

    void ToggleDisplay()
    {
        isVisible = !isVisible;
        scrollView.SetActive(isVisible);

        if (isVisible)
        {
            if (showSkillsInsteadOfStats)
                ShowPlayerSkills();
            else
                ShowPlayerStats();
        }
        else
        {
            ClearDisplay();
        }
    }

    void ShowPlayerStats()
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerInstance is null!");
            return;
        }

        ClearDisplay();

        AddText($"Name: {player.Name}");
        AddText($"Level: {player.Level}");
        AddText($"EXP: {player.CurrentExp}/{player.GetExpForNextLevel()}");

        foreach (CharacterStats.StatType stat in System.Enum.GetValues(typeof(CharacterStats.StatType)))
        {
            int val = player.Stats.GetStatValue(stat);
            AddText($"{stat}: {val}");
        }

        AddText($"Race: {player.Race.name}");
        AddText($"Class: {player.Class.name}");
    }

    void ShowPlayerSkills()
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerInstance is null!");
            return;
        }

        ClearDisplay();

        List<SkillsSO> skills = new();
        if (player.Class != null) skills.AddRange(player.Class.startingSkills);
        if (player.Race != null) skills.AddRange(player.Race.startingSkills);

        foreach (var skill in skills)
        {
            AddText($"<b>{skill.SkillName}</b>\n<color=#FFF>{skill.Description}</color>");
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
