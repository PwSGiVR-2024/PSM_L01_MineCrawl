// CharacterCreationUI.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CharacterBuilder : MonoBehaviour
{
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown classDropdown;
    private CharacterCreationData characterDataTemplate;


    public CharacterRaceSO[] availableRaces;
    public CharacterClassSO[] availableClasses;

    public TMP_InputField nameField;
    public CharacterHolder characterHolderPrefab;
    public Transform spawnPoint;
    public Image characterPreviewImage;
    [SerializeField] bool Random = false;
    void Start()
    {
        var original = Resources.Load<CharacterCreationData>("Spells/Player_1");
        if (original == null)
        {
            Debug.LogError("Nie udało się załadować CharacterCreationData z Resources.");
            return;
        }

        // Tworzymy kopię instancji w pamięci — unika problemów z modyfikacją assetu
        characterDataTemplate = Instantiate(original);
        BattleTransferData.playerInstance = null;

        if (characterDataTemplate.selectedClass == null && characterDataTemplate.selectedRace == null)
            Debug.Log("wyczyszczono");

        (availableRaces, availableClasses) = CheckAvailable();

        if (!Random)
        {
            SetupChoices();
            raceDropdown.onValueChanged.AddListener(delegate { UpdateCharacterPreview(); });
        }
    }

    public (CharacterRaceSO[], CharacterClassSO[]) CheckAvailable()
    {
        availableRaces = Resources.LoadAll<CharacterRaceSO>("races");
        availableClasses = Resources.LoadAll<CharacterClassSO>("classes");
        return (availableRaces, availableClasses);
    }
    public void RandomCharacter(CharacterRaceSO[] availableRaces, CharacterClassSO[] availableClasses)
    {
        

        if (availableRaces == null || availableRaces.Length == 0)
        {
            Debug.LogError("Brak dostępnych ras!");
            return;
        }

        if (availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogError("Brak dostępnych klas!");
            return;
        }

        CharacterClassSO klasa = availableClasses[UnityEngine.Random.Range(0, availableClasses.Length)];
        CharacterRaceSO rasa = availableRaces[UnityEngine.Random.Range(0, availableRaces.Length)];

        Debug.Log("klasa: " + klasa.name + ", rasa: " + rasa.name);

        characterDataTemplate.selectedRace = rasa;
        characterDataTemplate.selectedClass = klasa;
        characterDataTemplate.name = "Andrzej";
        LogManager.persistentLogLines.Clear();
        LogManager.hasShownIntro = false;
        CharacterInstance.ClearScore();
        BattleTransferData.characterData = characterDataTemplate;
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
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
        LogManager.persistentLogLines.Clear();
        LogManager.hasShownIntro = false;
        CharacterInstance.ClearScore();
        UnityEngine.SceneManagement.SceneManager.LoadScene(2); // przejdź do gry
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
