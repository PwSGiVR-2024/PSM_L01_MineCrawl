using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public Tilemap walkableTilemap;
    [SerializeField] public Grid grid;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private CharacterRaceSO[] availableEnemyRaces;
    [SerializeField] private CharacterClassSO[] availableEnemyClasses;

    private AudioSource audioSource;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Vector2Int lastDirection;
    private float holdDelay = 0.15f;
    private float holdTimer = 0f;

    private float encounterChance = 0.3f; //0.03f

    private void Start()
    {
        targetPosition = transform.position;
        if (PlayerCamera == null)
            PlayerCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (availableEnemyRaces == null || availableEnemyRaces.Length == 0)
            availableEnemyRaces = Resources.LoadAll<CharacterRaceSO>("races");

        if (availableEnemyClasses == null || availableEnemyClasses.Length == 0)
            availableEnemyClasses = Resources.LoadAll<CharacterClassSO>("classes");
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
            return;
        }

        Vector2Int inputDir = Vector2Int.zero;
        if (Input.GetKey(KeyCode.W)) inputDir.y = 1;
        else if (Input.GetKey(KeyCode.S)) inputDir.y = -1;
        else if (Input.GetKey(KeyCode.A)) inputDir.x = -1;
        else if (Input.GetKey(KeyCode.D)) inputDir.x = 1;

        if (inputDir != Vector2Int.zero)
        {
            if (holdTimer <= 0f || inputDir != lastDirection)
            {
                TryMove(inputDir);
                lastDirection = inputDir;
                holdTimer = holdDelay;
            }
            else
            {
                holdTimer -= Time.deltaTime;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    private void LateUpdate()
    {
        Vector3 cameraTarget = new Vector3(transform.position.x, transform.position.y, PlayerCamera.transform.position.z);
        PlayerCamera.transform.position = Vector3.Lerp(PlayerCamera.transform.position, cameraTarget, 10f * Time.deltaTime);
    }

    private void TryMove(Vector2Int dir)
    {
        Vector3Int currentPos = grid.WorldToCell(transform.position);
        Vector3Int moveDir = new Vector3Int(dir.x, dir.y, 0);
        Vector3Int newPos = currentPos + moveDir;

        if (spriteRenderer != null && dir.x != 0)
            spriteRenderer.flipX = (dir.x < 0);

        //if (IsEnemyAtPosition(newPos, out GameObject enemy))
        //{
        //    Debug.Log("Spotkano przeciwnika: " + enemy.name);
        //    enemy.GetComponent<Enemy>().Battle();
        //    return;
        //}

        if (IsWalkable(newPos))
        {
            MoveTo(newPos);
            TryTriggerRandomEncounter();  // <-- Tu jest wywo³anie funkcji
        }
    }

    private void MoveTo(Vector3Int gridPos)
    {
        targetPosition = grid.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        isMoving = true;

        if (footstepSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(footstepSound, 0.5f);
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private bool IsWalkable(Vector3Int position)
    {
        TileBase tile = walkableTilemap.GetTile(position);
        return tile != null;
    }

    //private bool IsEnemyAtPosition(Vector3Int gridPos, out GameObject enemy)
    //{
    //    Vector3 worldPos = grid.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 0.1f);

    //    foreach (var hit in hits)
    //    {
    //        if (hit.CompareTag("Enemy"))
    //        {
    //            enemy = hit.gameObject;
    //            return true;
    //        }
    //    }

    //    enemy = null;
    //    return false;
    //}


    private void TryTriggerRandomEncounter()
    {
        if (Random.value < encounterChance && availableEnemyRaces.Length > 0 && availableEnemyClasses.Length > 0)
        {
            CharacterRaceSO race = availableEnemyRaces[Random.Range(0, availableEnemyRaces.Length)];
            CharacterClassSO cls = availableEnemyClasses[Random.Range(0, availableEnemyClasses.Length)];

            CharacterSO enemyTemplate = ScriptableObject.CreateInstance<CharacterSO>();
            enemyTemplate.characterName = $"{race.raceName} {cls.className}";
            enemyTemplate.race = race;
            enemyTemplate.characterClass = cls;
            enemyTemplate.baseLevel = 1;
            enemyTemplate.baseStats = new CharacterStats();

            CharacterInstance enemyInstance = new CharacterInstance(enemyTemplate);
            BattleTransferData.enemyInstance = enemyInstance;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterHolder playerHolder = player.GetComponent<CharacterHolder>();
                if (playerHolder != null)
                {
                    BattleTransferData.playerInstance = playerHolder.characterInstance;
                }
            }

            BattleTransferData.previousSceneName = SceneManager.GetActiveScene().name;
            BattleTransferData.playerPosition = transform.position;
            BattleTransferData.cameFromBattle = true;

            LogManager.Instance.Log($"A wild {enemyTemplate.characterName} lvl: {enemyTemplate.baseLevel} appears!");
            SceneManager.LoadScene("BattleScene");
        }

    }
}
