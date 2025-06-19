using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneRestoreManager : MonoBehaviour
{
    private CharacterCreationData characterDataTemplate;
    public CharacterHolder characterPrefab;
    public Transform spawnPoint;
    [SerializeField] private Grid walk;
    [SerializeField] private Tilemap walkable;

    void Start()
    {
        CharacterHolder character = null;
        if (BattleTransferData.characterData == null)
        {
            Debug.LogError("Brak danych postaci! Wróæ do ekranu tworzenia.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
            return;
        }

        if (BattleTransferData.playerInstance == null &&
            BattleTransferData.characterData != null &&
            BattleTransferData.characterData.selectedRace != null &&
            BattleTransferData.characterData.selectedClass != null)
        {
            var so = ScriptableObject.CreateInstance<CharacterSO>();
            so.race = BattleTransferData.characterData.selectedRace;
            so.characterClass = BattleTransferData.characterData.selectedClass;
            so.characterName = BattleTransferData.characterData.name;

            CharacterInstance instance = new CharacterInstance(so);
            character = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
            character.characterInstance = instance;

            BattleTransferData.playerInstance = instance;
        }
        else if (BattleTransferData.playerInstance != null)
        {
            character = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
            character.characterInstance = BattleTransferData.playerInstance;
        }

        if (character != null)
        {
            SetupMovementAndSprite(character);

            if (BattleTransferData.cameFromBattle)
            {
                character.transform.position = BattleTransferData.playerPosition;

                if (!string.IsNullOrEmpty(BattleTransferData.defeatedEnemyID))
                {
                    GameObject defeated = GameObject.Find(BattleTransferData.defeatedEnemyID);
                    if (defeated != null) Destroy(defeated);
                }

                BattleTransferData.ResetAfterBattle();
            }
        }
    }


    private void SetupMovementAndSprite(CharacterHolder character)
    {
        var move = character.GetComponent<Movement>();
        move.grid = walk;
        move.walkableTilemap = walkable;

        var sr = character.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sprite = character.characterInstance.Race.battleSprite;
    }
}
