using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public Tilemap walkableTilemap;
    [SerializeField] public Grid grid;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private AudioSource audioSource;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        targetPosition = transform.position;
        if (PlayerCamera == null)
            PlayerCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            HandleInput();
        }
    }

    private void LateUpdate()
    {
        Vector3 cameraTarget = new Vector3(transform.position.x, transform.position.y, PlayerCamera.transform.position.z);
        PlayerCamera.transform.position = Vector3.Lerp(PlayerCamera.transform.position, cameraTarget, 10f * Time.deltaTime);
    }

    private void HandleInput()
    {
        int x = 0, y = 0;

        // Pojedyncze naciœniêcie = jeden ruch
        if (Input.GetKeyDown(KeyCode.W)) y += 1;
        if (Input.GetKeyDown(KeyCode.S)) y -= 1;
        if (Input.GetKeyDown(KeyCode.A)) x -= 1;
        if (Input.GetKeyDown(KeyCode.D)) x += 1;

        if (x != 0 || y != 0)
        {
            Vector3Int currentPos = grid.WorldToCell(transform.position);
            Vector3Int moveDir = new Vector3Int(x, y, 0);
            Vector3Int newPos = currentPos + moveDir;

            // Obracanie sprite w lewo/prawo
            if (spriteRenderer != null && x != 0)
                spriteRenderer.flipX = (x < 0);

            if (IsEnemyAtPosition(newPos, out GameObject enemy))
            {
                Debug.Log("Spotkano przeciwnika: " + enemy.name);
                enemy.GetComponent<Enemy>().Battle();
                return;
            }

            if (IsWalkable(newPos))
            {
                MoveTo(newPos);
            }
        }
    }

    private void MoveTo(Vector3Int gridPos)
    {
        targetPosition = grid.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        isMoving = true;

        if (footstepSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(footstepSound, 0.5f); // dŸwiêk 1s
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

    private bool IsEnemyAtPosition(Vector3Int gridPos, out GameObject enemy)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 0.1f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                enemy = hit.gameObject;
                return true;
            }
        }

        enemy = null;
        return false;
    }
}