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
        if (characterDataTemplate.selectedRace != null && characterDataTemplate.selectedClass != null)
        {
            CharacterSO newCharacter = ScriptableObject.CreateInstance<CharacterSO>();
            newCharacter.race = characterDataTemplate.selectedRace;
            newCharacter.characterClass = characterDataTemplate.selectedClass;
            newCharacter.InitializeStats();
            newCharacter.characterName = characterDataTemplate.name;
            CharacterHolder character = Instantiate(characterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            character.characterData = newCharacter;
            Movement script = character.GetComponent<Movement>();
            script.grid = walk;
            script.walkableTilemap = walkable;

            SpriteRenderer sr = character.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = characterDataTemplate.selectedRace.battleSprite;
            }

            characterDataTemplate.Clear(); // opcjonalne czyszczenie
        }
        if (BattleTransferData.cameFromBattle)
        {

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = BattleTransferData.playerPosition;

            // Usuñ pokonanego przeciwnika
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
}
