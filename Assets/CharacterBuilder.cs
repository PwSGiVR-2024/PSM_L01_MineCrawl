// CharacterCreationUI.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCreationData", menuName = "Game/CharacterCreationData")]
public class CharacterCreationData : ScriptableObject
{
    public string name;
    public CharacterRaceSO selectedRace;
    public CharacterClassSO selectedClass;

    public void Clear()
    {
        selectedRace = null;
        selectedClass = null;
    }
}

public class CharacterBuilder : MonoBehaviour
{
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown classDropdown;
    public CharacterCreationData characterDataTemplate;

    public CharacterRaceSO[] availableRaces;
    public CharacterClassSO[] availableClasses;

    public TMP_InputField nameField;
    public CharacterHolder characterHolderPrefab;
    public Transform spawnPoint;
    public Image characterPreviewImage;
    void Start()
    {
        availableRaces = Resources.LoadAll<CharacterRaceSO>("races");
        availableClasses = Resources.LoadAll<CharacterClassSO>("classes");
        SetupChoices();

        raceDropdown.onValueChanged.AddListener(delegate { UpdateCharacterPreview(); });
    }


    public void CreateCharacter()
    {
        CharacterRaceSO selectedRace = availableRaces[raceDropdown.value];
        CharacterClassSO selectedClass = availableClasses[classDropdown.value];
        if (nameField.text!="")
        {
            characterDataTemplate.name = nameField.text;
        }
        else
        {
            characterDataTemplate.name = "Andrzej";
        }

        characterDataTemplate.selectedRace = selectedRace;
        characterDataTemplate.selectedClass = selectedClass;

        UnityEngine.SceneManagement.SceneManager.LoadScene(1); // przejdź do gry
    }

    public void SetupChoices()
    {
        raceDropdown.ClearOptions();
        classDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> raceOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var race in availableRaces)
        {
            raceOptions.Add(new TMP_Dropdown.OptionData(race.raceName));
        }

        List<TMP_Dropdown.OptionData> classOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var klasa in availableClasses)
        {
            classOptions.Add(new TMP_Dropdown.OptionData(klasa.className));
        }

        raceDropdown.AddOptions(raceOptions);
        classDropdown.AddOptions(classOptions);

        UpdateCharacterPreview();
    }

    void UpdateCharacterPreview()
    {
        if (availableRaces.Length > raceDropdown.value)
        {
            CharacterRaceSO selectedRace = availableRaces[raceDropdown.value];
            characterPreviewImage.sprite = selectedRace.battleSprite;
        }
    }

}
