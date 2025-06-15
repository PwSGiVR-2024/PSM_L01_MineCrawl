using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneRestoreManager : MonoBehaviour
{
    public CharacterCreationData characterDataTemplate; // Powinno zawieraæ selectedRace, selectedClass, playerName itd.
    public CharacterHolder characterPrefab;
    public Transform spawnPoint;
    [SerializeField] private Grid walk;
    [SerializeField] private Tilemap walkable;
    void PrintAllStats(CharacterInstance player)
    {
        foreach (CharacterStats.StatType stat in Enum.GetValues(typeof(CharacterStats.StatType)))
        {
            int value = player.Stats.GetStatValue(stat);
            Debug.Log($"{stat}: {value}");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintAllStats(BattleTransferData.playerInstance);
        }
    }
    void Start()
    {
        if (BattleTransferData.playerInstance == null && characterDataTemplate.selectedRace != null && characterDataTemplate.selectedClass != null)
        {
            // Tworzenie nowej postaci jak dot¹d
            CharacterSO newCharacterSO = ScriptableObject.CreateInstance<CharacterSO>();
            newCharacterSO.race = characterDataTemplate.selectedRace;
            newCharacterSO.characterClass = characterDataTemplate.selectedClass;
            newCharacterSO.characterName = characterDataTemplate.name;

            CharacterInstance newCharacterInstance = new CharacterInstance(newCharacterSO);

            CharacterHolder character = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
            character.characterInstance = newCharacterInstance;

            SetupMovementAndSprite(character);

            BattleTransferData.playerInstance = newCharacterInstance;
        }
        else if (BattleTransferData.playerInstance != null)
        {
            // Jeœli mamy ju¿ playerInstance (po powrocie z walki), tworzymy prefab i ustawiamy dane
            CharacterHolder character = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
            character.characterInstance = BattleTransferData.playerInstance;

            SetupMovementAndSprite(character);
        }

        // Logika przy powrocie z walki (przemieszczenie, usuniêcie wrogów)
        if (BattleTransferData.cameFromBattle)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = BattleTransferData.playerPosition;

            if (!string.IsNullOrEmpty(BattleTransferData.defeatedEnemyID))
            {
                GameObject defeatedEnemy = GameObject.Find(BattleTransferData.defeatedEnemyID);
                if (defeatedEnemy != null)
                    Destroy(defeatedEnemy);

                BattleTransferData.defeatedEnemyID = null;
            }

            BattleTransferData.cameFromBattle = false;
        }
    }

    private void SetupMovementAndSprite(CharacterHolder character)
    {
        Movement script = character.GetComponent<Movement>();
        script.grid = walk;
        script.walkableTilemap = walkable;

        SpriteRenderer sr = character.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = character.characterInstance.Race.battleSprite;
        }
    }

}
