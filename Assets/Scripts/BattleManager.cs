using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public static class BattleTransferData
{
    public static CharacterSO playerData;
    public static CharacterSO enemyData;
}
public class BattleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHPText;

    [SerializeField] private GameObject characterVisualPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;

    [SerializeField] private Button[] skillButtons;

    private CharacterSO playerSO;
    private CharacterSO enemySO;

    private ICharacter player;
    private ICharacter enemy;

    public CharacterSO PlayerCharacter => playerSO; // publiczne do odczytu

    void Start()
    {
        playerSO = Instantiate(BattleTransferData.playerData);
        enemySO = Instantiate(BattleTransferData.enemyData);

        playerSO.InitializeStats();
        enemySO.InitializeStats();

        player = playerSO;
        enemy = enemySO;

        SpawnSprites(playerSO, enemySO);
        UpdateUI();
        SetupSkillButtons();
    }

    void SpawnSprites(CharacterSO playerSO, CharacterSO enemySO)
    {
        GameObject playerVisual = Instantiate(characterVisualPrefab, playerSpawnPoint.position, Quaternion.identity);
        playerVisual.GetComponent<SpriteRenderer>().sprite = playerSO.BattleSprite;

        GameObject enemyVisual = Instantiate(characterVisualPrefab, enemySpawnPoint.position, Quaternion.identity);
        enemyVisual.GetComponent<SpriteRenderer>().sprite = enemySO.BattleSprite;
    }

    void UpdateUI()
    {
        playerNameText.text = player.Name;
        playerHPText.text = $"HP: {player.Stats.GetStatValue(CharacterStats.StatType.CurrentHP)}/{player.Stats.GetStatValue(CharacterStats.StatType.MaxHP)}";

        enemyNameText.text = enemy.Name;
        enemyHPText.text = $"HP: {enemy.Stats.GetStatValue(CharacterStats.StatType.CurrentHP)}/{enemy.Stats.GetStatValue(CharacterStats.StatType.MaxHP)}";
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

    void PlayerAttack(ISkill skill)
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
        // Tutaj mo¿esz za³adowaæ inn¹ scenê lub pokazaæ ekran koñca walki
    }
}
