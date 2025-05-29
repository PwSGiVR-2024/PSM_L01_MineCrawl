using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Tilemap walkableTilemap;
    [SerializeField] Grid grid;
    [SerializeField] Camera PlayerCamera ;
    [SerializeField] private AudioClip footstepSound;
    private AudioSource audioSource;



    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
        PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
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
    void LateUpdate()
    {
        Vector3 cameraTargetPosition = new Vector3(transform.position.x, transform.position.y, PlayerCamera.transform.position.z);
        PlayerCamera.transform.position = Vector3.Lerp(PlayerCamera.transform.position, cameraTargetPosition, 10f * Time.deltaTime);
    }

    void HandleInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.down;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

        if (direction != Vector3.zero)
        {
            direction = direction.normalized;

            Vector3Int currentGridPos = grid.WorldToCell(transform.position);
            Vector3Int moveDirInt = Vector3Int.RoundToInt(direction);
            Vector3Int newPosition = currentGridPos + moveDirInt;
            GameObject enemy;
            if (IsEnemyAtPosition(newPosition, out enemy))
            {
                Debug.Log("Spotkano przeciwnika: " + enemy.name);
                // tutaj mo¿esz np. za³adowaæ scenê walki:
                // SceneManager.LoadScene("BattleScene");
                // lub wywo³aæ jak¹œ metodê:
                enemy.GetComponent<Enemy>().Battle();
                return;
            }

            //Próbujemy ukoœnie
            if (IsWalkable(newPosition))
            {
                MoveTo(newPosition);
            }
            else
            {
                //Spróbuj oœ X
                Vector3Int xMove = new Vector3Int(moveDirInt.x, 0, 0);
                Vector3Int xTarget = currentGridPos + xMove;

                if (moveDirInt.x != 0 && IsWalkable(xTarget))
                {
                    MoveTo(xTarget);
                }
                //Spróbuj oœ Y
                else
                {
                    Vector3Int yMove = new Vector3Int(0, moveDirInt.y, 0);
                    Vector3Int yTarget = currentGridPos + yMove;

                    if (moveDirInt.y != 0 && IsWalkable(yTarget))
                    {
                        MoveTo(yTarget);
                    }
                }
            }
        }
    }
    void MoveTo(Vector3Int gridPos)
    {
        targetPosition = grid.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        isMoving = true;

        if (footstepSound != null)
            audioSource.PlayOneShot(footstepSound);
    }


    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
           
            isMoving = false;
        }
    }

    bool IsWalkable(Vector3Int position)
    {
        TileBase tile = walkableTilemap.GetTile(position);
        return tile != null; 
    }
    bool IsEnemyAtPosition(Vector3Int gridPos, out GameObject enemy)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0); // œrodek kafelka
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
