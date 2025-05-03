using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Tilemap walkableTilemap;
    public Grid grid;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
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

    void HandleInput()
    {
        Vector3Int direction = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.S)) direction = Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.A)) direction = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.D)) direction = Vector3Int.right;

        if (direction != Vector3Int.zero)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);
            Vector3Int newPosition = gridPosition + direction;

            if (IsWalkable(newPosition))
            {
                targetPosition = grid.CellToWorld(newPosition) + new Vector3(0.5f, 0.5f, 0); // centrowanie
                isMoving = true;
            }
        }
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
}
