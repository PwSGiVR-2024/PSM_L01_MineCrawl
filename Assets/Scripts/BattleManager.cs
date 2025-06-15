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
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI playerMPText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHPText;
    [SerializeField] private TextMeshProUGUI enemyMPText;
    [SerializeField] private GameObject characterVisualPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Button[] skillButtons;

    private CharacterInstance player;
    private CharacterInstance enemy;

    void Start()
    {
        player = BattleTransferData.playerInstance;
        enemy = BattleTransferData.enemyInstance;

        SpawnSprites(player, enemy);
        UpdateUI();
        SetupSkillButtons();
    }

    void SpawnSprites(CharacterInstance player, CharacterInstance enemy)
    {
        GameObject playerVisual = Instantiate(characterVisualPrefab, playerSpawnPoint.position, Quaternion.identity);
        playerVisual.GetComponent<SpriteRenderer>().sprite = player.Race.BattleSprite;

        GameObject enemyVisual = Instantiate(characterVisualPrefab, enemySpawnPoint.position, Quaternion.identity);
        enemyVisual.GetComponent<SpriteRenderer>().sprite = enemy.Race.BattleSprite;
    }

    void UpdateUI()
    {
        playerNameText.text = player.Name;
        playerHPText.text = $"HP: {player.Stats.GetStatValue(CharacterStats.StatType.CurrentHP)}/{player.Stats.GetStatValue(CharacterStats.StatType.MaxHP)}";
        playerMPText.text = $"MP: {player.Stats.GetStatValue(CharacterStats.StatType.Mana)}/{player.Stats.GetStatValue(CharacterStats.StatType.MaxMana)}";

        enemyNameText.text = enemy.Name;
        enemyHPText.text = $"HP: {enemy.Stats.GetStatValue(CharacterStats.StatType.CurrentHP)}/{enemy.Stats.GetStatValue(CharacterStats.StatType.MaxHP)}";
        enemyMPText.text = $"MP: {enemy.Stats.GetStatValue(CharacterStats.StatType.Mana)}/{enemy.Stats.GetStatValue(CharacterStats.StatType.MaxMana)}";
    }

    void SetupSkillButtons()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < player.Class.startingSkills.Count)
            {
                int index = i;
                skillButtons[i].gameObject.SetActive(true);

                TextMeshProUGUI tmpText = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                    tmpText.text = player.Class.startingSkills[i].SkillName;

                skillButtons[i].onClick.AddListener(() => PlayerAttack(player.Class.startingSkills[index]));
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(SkillsSO skill)
    {
        if (player.Stats.GetStatValue(CharacterStats.StatType.Mana) < skill.ManaCost)
        {
            Debug.Log("Za ma³o many!");
            return;
        }

        player.UseSkill(skill, enemy);
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
        yield return new WaitForSeconds(1f);
        ISkill enemySkill = enemy.Class.startingSkills[0];
        enemy.UseSkill(enemySkill, player);

        UpdateUI();

        if (player.Stats.IsDead)
        {
            EndBattle(false);
        }
    }

    void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Wygra³eœ walkê!" : "Przegra³eœ...");

        if (playerWon)
        {
            int baseExp = 5;
            int floor = 2; // lub DungeonManager.Instance.GetCurrentFloor()
            int expReward = baseExp + floor * 2;

            player.GainExp(expReward);
        }

        SceneManager.LoadScene(BattleTransferData.previousSceneName);
    }
}
