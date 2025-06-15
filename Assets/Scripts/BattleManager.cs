using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public static class BattleTransferData
{
    public static CharacterInstance playerInstance;
    public static CharacterInstance enemyInstance;

    public static string defeatedEnemyID;
    public static string previousSceneName;
    public static Vector3 playerPosition;
    public static bool cameFromBattle = false;
}


public class BattleManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI playerMPText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHPText;
    [SerializeField] private TextMeshProUGUI enemyMPText;
    [SerializeField] private Image hurtScreen; 
    [SerializeField] private GameObject characterVisualPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Button[] skillButtons;

    private CharacterInstance player;
    private CharacterInstance enemy;
    private bool isAnimating = false;

    void Start()
    {
        player = BattleTransferData.playerInstance;
        enemy = BattleTransferData.enemyInstance;

        SpawnSprites(player, enemy);
        UpdateUI();
        SetupSkillButtons();

        if (hurtScreen != null)
            hurtScreen.color = new Color(1, 0, 0, 0); // Transparent
    }

    void SpawnSprites(CharacterInstance player, CharacterInstance enemy)
    {
        GameObject playerVisual = Instantiate(characterVisualPrefab, playerSpawnPoint.position, Quaternion.identity, playerSpawnPoint);
        playerVisual.GetComponent<SpriteRenderer>().sprite = player.Race.BattleSprite;

        GameObject enemyVisual = Instantiate(characterVisualPrefab, enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);
        enemyVisual.GetComponent<SpriteRenderer>().sprite = enemy.Race.BattleSprite;
    }

    void SetupSkillButtons()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < player.Class.startingSkills.Count)
            {
                int index = i;
                skillButtons[i].gameObject.SetActive(true);
                skillButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = player.Class.startingSkills[i].SkillName;

                skillButtons[i].onClick.AddListener(() => PlayerAttack(player.Class.startingSkills[index]));
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateUI()
    {
        playerNameText.text = player.Name;
        enemyNameText.text = enemy.Name;

        StartCoroutine(UpdateAllStatsAndUnblock());
    }

    IEnumerator UpdateAllStatsAndUnblock()
    {
        isAnimating = true;

        yield return StartCoroutine(UpdateStatText(playerHPText, player.Stats.GetStatValue(CharacterStats.StatType.CurrentHP), player.Stats.GetStatValue(CharacterStats.StatType.MaxHP)));
        yield return StartCoroutine(UpdateStatText(playerMPText, player.Stats.GetStatValue(CharacterStats.StatType.Mana), player.Stats.GetStatValue(CharacterStats.StatType.MaxMana)));
        yield return StartCoroutine(UpdateStatText(enemyHPText, enemy.Stats.GetStatValue(CharacterStats.StatType.CurrentHP), enemy.Stats.GetStatValue(CharacterStats.StatType.MaxHP)));
        yield return StartCoroutine(UpdateStatText(enemyMPText, enemy.Stats.GetStatValue(CharacterStats.StatType.Mana), enemy.Stats.GetStatValue(CharacterStats.StatType.MaxMana)));

        isAnimating = false;
    }

    IEnumerator UpdateStatText(TextMeshProUGUI textElement, int targetCurrent, int max)
    {
        float duration = 0.4f;
        float elapsed = 0f;

        string prefix = textElement.text.StartsWith("HP") ? "HP" : "MP";
        int displayedCurrent = ExtractCurrentValue(textElement.text);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            int interpolated = Mathf.RoundToInt(Mathf.Lerp(displayedCurrent, targetCurrent, t));
            textElement.text = $"{prefix}: {interpolated}/{max}";
            yield return null;
        }

        textElement.text = $"{prefix}: {targetCurrent}/{max}";
    }

    private int ExtractCurrentValue(string text)
    {
        if (text.Contains(":") && text.Contains("/"))
        {
            string[] parts = text.Split(':');
            if (parts.Length > 1)
            {
                string[] values = parts[1].Split('/');
                if (values.Length > 0 && int.TryParse(values[0].Trim(), out int value))
                    return value;
            }
        }

        return 0;
    }

    public void PlayerAttack(SkillsSO skill)
    {
        if (isAnimating) return;

        if (player.Stats.GetStatValue(CharacterStats.StatType.Mana) < skill.ManaCost)
        {
            Debug.Log("Za mało many!");
            return;
        }

        player.UseSkill(skill, enemy);
        StartCoroutine(FlashDamage(enemySpawnPoint)); // efekt trafienia

        UpdateUI();

        if (enemy.Stats.IsDead)
        {
            EndBattle(true);
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.8f);

        ISkill chosen = enemy.Class.startingSkills[Random.Range(0, enemy.Class.startingSkills.Count)];
        enemy.UseSkill(chosen, player);

        StartCoroutine(FlashDamage(playerSpawnPoint));
        UpdateUI();

        if (player.Stats.IsDead)
        {
            EndBattle(false);
        }
    }

    IEnumerator FlashDamage(Transform target)
    {
        // Zmiana koloru na czerwony
        SpriteRenderer sr = target.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            sr.color = original;
        }

        if (hurtScreen != null && target == playerSpawnPoint)
        {
            yield return StartCoroutine(FlashHurtScreen());
        }
    }

    IEnumerator FlashHurtScreen()
    {
        float fadeIn = 0.1f;
        float fadeOut = 0.3f;

        Color c = hurtScreen.color;

        // Fade in
        for (float t = 0; t < fadeIn; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, 0.4f, t / fadeIn);
            hurtScreen.color = c;
            yield return null;
        }

        // Fade out
        for (float t = 0; t < fadeOut; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0.4f, 0, t / fadeOut);
            hurtScreen.color = c;
            yield return null;
        }

        c.a = 0;
        hurtScreen.color = c;
    }

    void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Wygrałeś walkę!" : "Przegrałeś...");

        if (playerWon)
        {
            int expReward = 5 + 2 * 2;
            player.GainExp(expReward);
            SceneManager.LoadScene(BattleTransferData.previousSceneName);
        }
        else
        {
            SceneManager.LoadScene("Main menu"); 
        }
    }
}
