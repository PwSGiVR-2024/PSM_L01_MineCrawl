using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStatsDisplay : MonoBehaviour
{
    public GameObject scrollView;        // ca³y ScrollView (do w³¹czania/wy³¹czania)
    public Transform contentParent;      // Content w ScrollView (tam bêd¹ teksty)
    public GameObject statTextPrefab;    // prefab TMP Text

    public CharacterHolder holder;     // twoja instancja gracza
    public CharacterInstance player;
    private void Start()
    {
        player = holder.characterInstance;
    }
    private bool isVisible = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isVisible = !isVisible;
            scrollView.SetActive(isVisible);

            if (isVisible)
                ShowPlayerStats();
            else
                ClearStats();
        }
    }

    void ShowPlayerStats()
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerInstance is null!");
            return;
        }

        ClearStats();

        // Przyk³ad wypisywania nazw i wartoœci statów
        AddStatText($"Name: {player.Name}");
        AddStatText($"Level: {player.Level}");
        AddStatText($"EXP: {player.CurrentExp}/{player.GetExpForNextLevel()}"); // jeœli masz publiczny dostêp lub metodê

        var stats = player.Stats;

        foreach (CharacterStats.StatType stat in System.Enum.GetValues(typeof(CharacterStats.StatType)))
        {
            int val = stats.GetStatValue(stat);
            AddStatText($"{stat}: {val}");
        }

        // Przyk³adowo mo¿esz te¿ wyœwietliæ rasê i klasê
        AddStatText($"Race: {player.Race.name}");
        AddStatText($"Class: {player.Class.name}");
    }

    void AddStatText(string text)
    {
        Debug.Log("Adding stat: " + text);
        GameObject go = Instantiate(statTextPrefab, contentParent);
        TMP_Text tmp = go.GetComponent<TMP_Text>();
        if (tmp != null)
            tmp.text = text;
        else
            Debug.LogWarning("Prefab statTextPrefab nie ma TMP_Text");
    }


    void ClearStats()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
