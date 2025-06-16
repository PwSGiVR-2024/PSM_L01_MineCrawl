using UnityEngine;

public class FinishAltar : MonoBehaviour
{
    private FloorChanger floorChanger;
    private bool playerInRange;
    private GameObject player;

    private void Start()
    {
        floorChanger = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorChanger>();
        playerInRange = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                floorChanger.ChangeFloor(1);
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                floorChanger.ChangeFloor(-1);
            }
        }
        
    }

}
