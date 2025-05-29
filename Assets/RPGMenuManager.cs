using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static PlayerStats;
using System.Collections;

public class RPGMenuManager : MonoBehaviour
{
    IEnumerator InitializePlayer()
    {
        while (battleManager.PlayerCharacter == null)
            yield return null;

        playerSO = battleManager.PlayerCharacter;
        player = playerSO;

        if (player == null)
        {
            Debug.LogError("Player ICharacter is STILL null in RPGMenuManager!");
            yield break;
        }

        attackButton.onClick.AddListener(() => OpenSubMenu("Attack"));
        magicButton.onClick.AddListener(() => OpenSubMenu("Magic"));
        itemButton.onClick.AddListener(() => OpenSubMenu("Items"));
    }

    [Header("Reference to BattleManager")]
    [SerializeField] private BattleManager battleManager;

    private CharacterSO playerSO;
    private ICharacter player;

    public CharacterSO PlayerCharacter => playerSO;

    [Header("Menu Panels")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelSubMenu;
    [SerializeField] private Transform subMenuButtonContainer;

    [Header("Main Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button magicButton;
    [SerializeField] private Button itemButton;
    [SerializeField] private Button runButton;

    [Header("Submenu")]
    [SerializeField] private GameObject skillButtonPrefab;

    private List<string> currentOptions = new();

    void Start()
    {
        if (battleManager == null)
        {
            Debug.LogError("BattleManager reference not set in RPGMenuManager!");
            return;
        }

        StartCoroutine(InitializePlayer());
    }

    public void OpenSubMenu(string category)
    {
        if (player == null)
        {
            Debug.LogError("Player is null in OpenSubMenu");
            return;
        }

        SkillCategory skillCategory;

        switch (category)
        {
            case "Attack":
                skillCategory = SkillCategory.Physical;
                break;
            case "Magic":
                skillCategory = SkillCategory.Magic;
                break;
            case "Items":
                skillCategory = SkillCategory.Item;
                break;
            default:
                return;
        }

        List<SkillsSO> skills = GetAllCharacterSkills(player);
        List<SkillsSO> filteredSkills = skills.FindAll(s => s.Category == skillCategory);

        UpdateSubMenu(filteredSkills);
        panelMainMenu.SetActive(false);
        panelSubMenu.SetActive(true);
    }

    void UpdateSubMenu(List<SkillsSO> skills)
    {
        foreach (Transform child in subMenuButtonContainer)
            Destroy(child.gameObject);

        foreach (var skill in skills)
        {
            GameObject btnObj = Instantiate(skillButtonPrefab, subMenuButtonContainer);
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = skill.SkillName;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                SkillsSO capturedSkill = skill;
                btn.onClick.AddListener(() => UseSkill(capturedSkill));
            }
        }

        GameObject backBtnObj = Instantiate(skillButtonPrefab, subMenuButtonContainer);
        TextMeshProUGUI backTxt = backBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (backTxt != null) backTxt.text = "← Wróć";

        Button backBtn = backBtnObj.GetComponent<Button>();
        if (backBtn != null) backBtn.onClick.AddListener(BackToMain);
    }

    void UseSkill(SkillsSO skill)
    {
        battleManager.PlayerAttack(skill);
        BackToMain(); // po ataku wróć do menu głównego
    }


    void BackToMain()
    {
        panelSubMenu.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    public List<SkillsSO> GetAllCharacterSkills(ICharacter character)
    {
        List<SkillsSO> allSkills = new();

        if (character.Class != null)
            allSkills.AddRange(character.Class.startingSkills);

        if (character.Race != null)
            allSkills.AddRange(character.Race.startingSkills);

        return allSkills;
    }
}
