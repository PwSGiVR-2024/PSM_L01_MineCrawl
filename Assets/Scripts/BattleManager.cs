using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;

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
    [SerializeField] private SkillTooltip skillTooltip;

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
    [SerializeField] private Button waitButton;
    [SerializeField] private GameObject blockerPanel;
    private Queue<CharacterInstance> turnOrderQueue;
    private CharacterInstance currentCharacter; // kto teraz wykonuje turę

    private CharacterInstance player;
    private CharacterInstance enemy;
    private bool isAnimating = false;
    [SerializeField] private GameObject popupTextPrefab;
    public void ShowDamagePopup(int amount, bool isHealing, Transform aboveTransform)
    {
        Vector3 spawnPos = aboveTransform.position + new Vector3(0, 2f, 0); // nad głową
        GameObject popupGO = Instantiate(popupTextPrefab, spawnPos, Quaternion.identity);
        DamagePopup popup = popupGO.GetComponent<DamagePopup>();

        Color color = isHealing ? Color.green : Color.red;
        string text = isHealing ? $"+{amount}" : $"-{amount}";

        popup.Setup(text, color);
    }

    void Start()
    {
        SetAnimationLock(true);
        player = BattleTransferData.playerInstance;
        enemy = BattleTransferData.enemyInstance;
        blockerPanel.SetActive(false);
        player.PrepareForBattle();
        enemy.PrepareForBattle();

        SpawnSprites(player, enemy);
        UpdateUI();
        waitButton.onClick.AddListener(WaitTurn);

        if (hurtScreen != null)
            hurtScreen.color = new Color(1, 0, 0, 0);

        InitializeTurnOrder();
        StartCoroutine(ProcessNextTurn());
    }
    void InitializeTurnOrder()
    {
        List<CharacterInstance> allCharacters = new List<CharacterInstance> { player, enemy };
        // Sortuj malejąco po Agility
        allCharacters.Sort((a, b) =>
            b.Stats.GetStatValue(CharacterStats.StatType.Agility).CompareTo(
            a.Stats.GetStatValue(CharacterStats.StatType.Agility)));

        turnOrderQueue = new Queue<CharacterInstance>(allCharacters);
    }
    private IEnumerator ProcessNextTurn()
    {
        SetAnimationLock(true);
        // Czekaj, jeśli trwa animacja
        

        if (turnOrderQueue.Count == 0)
            InitializeTurnOrder();

        currentCharacter = turnOrderQueue.Dequeue();

        if (currentCharacter == player)
        {
            // Aktywuj UI, aby gracz mógł wykonać akcję
            blockerPanel.SetActive(false);
            waitButton.interactable = true;
            LogManager.Instance.Log("Your turn.");

            SetAnimationLock(false);
        }
        else
        {
            // Tura przeciwnika - blokuj UI i wykonaj akcję AI
            blockerPanel.SetActive(true);
            waitButton.interactable = false;
            LogManager.Instance.Log("Enemy turn.");
            yield return StartCoroutine(EnemyTurn());

            if (enemy.Stats.IsDead)
            {
                StartCoroutine(EndBattle(true));
                yield break;
            }

            if (player.Stats.IsDead)
            {
                StartCoroutine(EndBattle(false));
                yield break;
            }

            // Po zakończeniu tury przeciwnika dodaj go z powrotem do kolejki
            turnOrderQueue.Enqueue(currentCharacter);
            StartCoroutine(ProcessNextTurn());
        }
    }


    void SetAnimationLock(bool state)
    {
        isAnimating = state;
        blockerPanel.SetActive(state);
    }

    void SpawnSprites(CharacterInstance player, CharacterInstance enemy)
    {
        GameObject playerVisual = Instantiate(characterVisualPrefab, playerSpawnPoint.position, Quaternion.identity, playerSpawnPoint);
        playerVisual.GetComponent<SpriteRenderer>().sprite = player.Race.BattleSprite;

        GameObject enemyVisual = Instantiate(characterVisualPrefab, enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);
        enemyVisual.GetComponent<SpriteRenderer>().sprite = enemy.Race.BattleSprite;
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

    private IEnumerator PlaySkillEffect(GameObject effectPrefab, Transform userTransform, Transform targetTransform, ParticleEffectType effectType)
    {
        if (effectPrefab == null) yield break;

        ;
        GameObject effectInstance;

        switch (effectType)
        {
            case ParticleEffectType.Projectile:
                Vector3 direction = (targetTransform.position - userTransform.position).normalized;
                Vector3 startPos = userTransform.position + direction * 2.5f + new Vector3(0, 0, -1f);

                bool isPlayer = userTransform == playerSpawnPoint;

                if (isPlayer && direction.x < 0)
                {
                    float yDiff = Mathf.Abs(targetTransform.position.y - userTransform.position.y);
                    float threshold = 0.1f;

                    if (yDiff > threshold)
                    {
                        startPos += new Vector3(0.3f, 0, 0);
                    }
                }

                Quaternion rotation = Quaternion.LookRotation(direction);
                effectInstance = Instantiate(effectPrefab, startPos, rotation);
                break;

            case ParticleEffectType.OnTarget:
                effectInstance = Instantiate(effectPrefab, targetTransform.position, Quaternion.identity);
                break;

            case ParticleEffectType.SelfCast:
                effectInstance = Instantiate(effectPrefab, userTransform.position, Quaternion.identity);
                break;

            default:
                effectInstance = Instantiate(effectPrefab, userTransform.position, Quaternion.identity);
                break;
        }

        ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            while (ps.IsAlive(true)) yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        Destroy(effectInstance);
        ;
    }

    public void PlayerAttack(SkillsSO skill)
    {
        if (isAnimating) return;
        skillTooltip.Hide();

        if (player.Stats.GetStatValue(CharacterStats.StatType.Mana) < skill.ManaCost)
        {
            LogManager.Instance.Log("Not enough mana!");
            return;
        }

        ICharacter target = skill.selfUse ? player : enemy;
        StartCoroutine(PerformPlayerAttack(skill, target));
        SetAnimationLock(true); 
    }

    private IEnumerator PerformPlayerAttack(SkillsSO skill, ICharacter target)
    {
        ;

        Transform fromTransform = playerSpawnPoint;
        Transform toTransform = skill.selfUse ? playerSpawnPoint : enemySpawnPoint;
        int hpBefore = target.Stats.GetStatValue(CharacterStats.StatType.CurrentHP);
        player.UseSkill(skill, target);
        int hpAfter = target.Stats.GetStatValue(CharacterStats.StatType.CurrentHP);
        bool isHealing = skill.Damage < 0 || skill.Effects.Contains(SkillEffectType.Heal);
        bool isLifeSteal =skill.Vampire > 0 ? true : false;
        int amount = Mathf.Abs(hpAfter-hpBefore); 
        ShowDamagePopup(amount, isHealing, skill.selfUse ? playerSpawnPoint : enemySpawnPoint);
        if (isLifeSteal)
        {
            amount = Mathf.CeilToInt(Mathf.Abs(amount * skill.Vampire));
            ShowDamagePopup(amount, true, playerSpawnPoint);
        }
        UpdateUI();

        yield return StartCoroutine(PlaySkillEffect(skill.VisualEffectPrefab, fromTransform, toTransform, skill.EffectType));
        Color flashColor = skill.Damage <= 0 ? Color.green : Color.red;
        Transform flashTarget = skill.selfUse ? playerSpawnPoint : enemySpawnPoint;
        yield return StartCoroutine(FlashDamage(flashTarget, flashColor));
        RegenerateBasic(player);




        if (!skill.selfUse && enemy.Stats.IsDead)
        {
            StartCoroutine(EndBattle(true));
            yield break;
        }

        ;

        turnOrderQueue.Enqueue(player);
        StartCoroutine(ProcessNextTurn());
    }


    public void WaitTurn()
    {
        if (isAnimating) return;
        ;

        int hpRegen = Mathf.CeilToInt(player.Stats.GetStatValue(CharacterStats.StatType.MaxHP) * 0.05f);
        int manaRegen = Mathf.CeilToInt(player.Stats.GetStatValue(CharacterStats.StatType.MaxMana) * 0.15f);

        player.Stats.ChangeStat(CharacterStats.StatType.CurrentHP, hpRegen);
        player.Stats.ChangeStat(CharacterStats.StatType.Mana, manaRegen);

        LogManager.Instance.Log($"You wait... +{hpRegen} HP, +{manaRegen} Mana");
        UpdateUI();

        ;

        turnOrderQueue.Enqueue(player);
        StartCoroutine(ProcessNextTurn());
    }


    IEnumerator EnemyTurn()
    {
        ;
        yield return new WaitForSeconds(0.8f);

        List<SkillsSO> usableSkills = new List<SkillsSO>();
        foreach (var skill in enemy.Class.startingSkills)
        {
            if (enemy.Stats.GetStatValue(CharacterStats.StatType.Mana) >= skill.ManaCost)
                usableSkills.Add(skill);
        }

        if (usableSkills.Count == 0)
        {
            int hpRegen = Mathf.CeilToInt(enemy.Stats.GetStatValue(CharacterStats.StatType.MaxHP) * 0.05f);
            int manaRegen = Mathf.CeilToInt(enemy.Stats.GetStatValue(CharacterStats.StatType.MaxMana) * 0.2f);
            enemy.Stats.ChangeStat(CharacterStats.StatType.CurrentHP, hpRegen);
            enemy.Stats.ChangeStat(CharacterStats.StatType.Mana, manaRegen);

            LogManager.Instance.Log($"Enemy is resting... +{hpRegen} HP, +{manaRegen} Mana");
            UpdateUI();
            yield break;
        }

        float playerHpPct = (float)player.Stats.GetStatValue(CharacterStats.StatType.CurrentHP) / player.Stats.GetStatValue(CharacterStats.StatType.MaxHP);
        float enemyHpPct = (float)enemy.Stats.GetStatValue(CharacterStats.StatType.CurrentHP) / enemy.Stats.GetStatValue(CharacterStats.StatType.MaxHP);

        SkillsSO chosenSkill = null;
        float bestScore = float.MinValue;

        foreach (var skill in usableSkills)
        {
            float score = 0f;
            if (skill.selfUse && skill.Damage < 0)
            {
                if (enemyHpPct < 0.5f)
                    score += 20f * (1f - enemyHpPct);
            }
            if (!skill.selfUse && skill.Damage > 0)
            {
                score += 10f;
                if (playerHpPct < 0.3f)
                    score += 20f;
                score += skill.Damage * 0.5f;
            }
            score += Random.Range(0f, 5f);
            if (score > bestScore)
            {
                bestScore = score;
                chosenSkill = skill;
            }
        }

        if (chosenSkill != null)
        {
            ICharacter target = chosenSkill.selfUse ? enemy : player;
            Transform fromTransform = enemySpawnPoint;
            Transform toTransform = chosenSkill.selfUse ? enemySpawnPoint : playerSpawnPoint;
            int hpBefore = target.Stats.GetStatValue(CharacterStats.StatType.CurrentHP);
            enemy.UseSkill(chosenSkill, target);
            int hpAfter = target.Stats.GetStatValue(CharacterStats.StatType.CurrentHP);
            bool isHealing = chosenSkill.Damage < 0 || chosenSkill.Effects.Contains(SkillEffectType.Heal);
            bool isLifeSteal = chosenSkill.Vampire > 0 ? true : false;
            int amount = Mathf.Abs(hpAfter - hpBefore);
            ShowDamagePopup(amount, isHealing, chosenSkill.selfUse ? enemySpawnPoint : playerSpawnPoint);
            if (isLifeSteal)
            {
                amount = Mathf.CeilToInt(Mathf.Abs(amount * chosenSkill.Vampire));
                ShowDamagePopup(amount, true, enemySpawnPoint);
            }
            
            yield return StartCoroutine(PlaySkillEffect(chosenSkill.VisualEffectPrefab, fromTransform, toTransform, chosenSkill.EffectType));

           
            RegenerateBasic(enemy);

            //LogManager.Instance.Log($"Enemy used {chosenSkill.SkillName}.");

            Color flashColor = chosenSkill.Damage < 0 ? Color.green : Color.red;
            Transform flashTarget = chosenSkill.selfUse ? enemySpawnPoint : playerSpawnPoint;

            StartCoroutine(FlashDamage(flashTarget, flashColor));
            UpdateUI();

            if (!chosenSkill.selfUse && player.Stats.IsDead)
            {
                StartCoroutine(EndBattle(false)); // lub false

                yield break;
            }
        }
    }

    void RegenerateBasic(CharacterInstance character)
    {
        int manaRegen = Mathf.CeilToInt(character.Stats.GetStatValue(CharacterStats.StatType.MaxMana) * 0.05f);
        character.Stats.ChangeStat(CharacterStats.StatType.Mana, manaRegen);
    }

    IEnumerator FlashDamage(Transform target, Color color)
    {
        SpriteRenderer sr = target.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = color;
            yield return new WaitForSeconds(0.2f);
            sr.color = original;
        }

        if (hurtScreen != null && target == playerSpawnPoint && color == Color.red)
        {
            yield return StartCoroutine(FlashHurtScreen());
        }
    }

    IEnumerator FlashHurtScreen()
    {
        float fadeIn = 0.1f;
        float fadeOut = 0.3f;
        Color c = hurtScreen.color;

        for (float t = 0; t < fadeIn; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, 0.4f, t / fadeIn);
            hurtScreen.color = c;
            yield return null;
        }

        for (float t = 0; t < fadeOut; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0.4f, 0, t / fadeOut);
            hurtScreen.color = c;
            yield return null;
        }

        c.a = 0;
        hurtScreen.color = c;
    }
    private IEnumerator EndBattle(bool playerWon)
    {
        LogManager.Instance.Log(playerWon ? "You won the battle!" : "You lost the battle...");

        

        yield return new WaitForSeconds(1.5f);

        if (playerWon)
        {
            int expReward = 5 + 2 * 2; // przykładowa nagroda
            player.GainExp(expReward);
            player.OnEnemyDefeated();
            SceneManager.LoadScene(BattleTransferData.previousSceneName);
        }
        else
        {
            SceneManager.LoadScene("Main menu");
        }
    }
}
