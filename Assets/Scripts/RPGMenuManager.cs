using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class RPGMenuManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    private CharacterInstance player;
    [SerializeField] private SkillTooltip skillTooltip;

    [Header("Menu Panels")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelSubMenu;
    [SerializeField] private Transform subMenuButtonContainer;

    [Header("Main Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button magicButton;
    [SerializeField] private Button itemButton;

    [Header("Submenu")]
    [SerializeField] private GameObject skillButtonPrefab;

    private void Start()
    {
        if (battleManager == null)
        {
            Debug.LogError("BattleManager reference not set!");
            return;
        }
        StartCoroutine(InitializePlayer());
    }

    IEnumerator InitializePlayer()
    {
        while (BattleTransferData.playerInstance == null)
            yield return null;

        player = BattleTransferData.playerInstance;

        attackButton.onClick.AddListener(() => OpenSubMenu(SkillCategory.Physical));
        magicButton.onClick.AddListener(() => OpenSubMenu(SkillCategory.Magic));
        itemButton.onClick.AddListener(() => OpenSubMenu(SkillCategory.Item));
    }

    void OpenSubMenu(SkillCategory category)
    {
        List<SkillsSO> filteredSkills = GetAllCharacterSkills(player).FindAll(s => s.Category == category);
        UpdateSubMenu(filteredSkills);
        panelMainMenu.SetActive(false);
        panelSubMenu.SetActive(true);
    }

    void UpdateSubMenu(List<SkillsSO> skills)
    {
        // Usuń stare przyciski
        foreach (Transform child in subMenuButtonContainer)
            Destroy(child.gameObject);

        // Dodaj przyciski umiejętności
        foreach (var skill in skills)
        {
            GameObject btnObj = Instantiate(skillButtonPrefab, subMenuButtonContainer);
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = skill.SkillName;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => UseSkill(skill));
            }

            // Dodaj komponent obsługi tooltipu i przypisz skill + tooltip
            SkillTooltipHandler tooltipHandler = btnObj.GetComponent<SkillTooltipHandler>();
            if (tooltipHandler == null)
                tooltipHandler = btnObj.AddComponent<SkillTooltipHandler>();

            tooltipHandler.skill = skill;
            tooltipHandler.skillTooltip = skillTooltip;
        }

        // Dodaj przycisk "← Wróć"
        GameObject backBtnObj = Instantiate(skillButtonPrefab, subMenuButtonContainer);
        TextMeshProUGUI backTxt = backBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (backTxt != null) backTxt.text = "← Wróć";

        Button backBtn = backBtnObj.GetComponent<Button>();
        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(BackToMain);
        }
    }


    void UseSkill(SkillsSO skill)
    {
        battleManager.PlayerAttack(skill);
        BackToMain();
    }

    void BackToMain()
    {
        panelSubMenu.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    public List<SkillsSO> GetAllCharacterSkills(ICharacter character)
    {
        List<SkillsSO> allSkills = new();
        if (character.Class != null) allSkills.AddRange(character.Class.startingSkills);
        if (character.Race != null) allSkills.AddRange(character.Race.startingSkills);
        return allSkills;
    }
}
