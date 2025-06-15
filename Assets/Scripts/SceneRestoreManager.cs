using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneRestoreManager : MonoBehaviour
{
    public CharacterCreationData characterDataTemplate;
    public CharacterHolder characterPrefab;
    public Transform spawnPoint;
    [SerializeField] private Grid walk;
    [SerializeField] private Tilemap walkable;

    void Start()
    {
        CharacterHolder character = null;

        if (BattleTransferData.playerInstance == null &&
            characterDataTemplate.selectedRace != null &&
            characterDataTemplate.selectedClass != null)
        {
            var so = ScriptableObject.CreateInstance<CharacterSO>();
            so.race = characterDataTemplate.selectedRace;
            so.characterClass = characterDataTemplate.selectedClass;
            so.characterName = characterDataTemplate.name;

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
                    BattleTransferData.defeatedEnemyID = null;
                }

                BattleTransferData.cameFromBattle = false;
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
