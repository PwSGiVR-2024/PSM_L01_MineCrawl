using Assets.Scripts.CreateRoom;
using UnityEngine;
using static Assets.Scripts.CreateRoom.CreateRoom;

public class FloorChange : MonoBehaviour
{
    public GameObject nextFloorAltarPrefab;

    private CreateRoom floorData;
    private Room altarSpawnRoom;
    private Vector2 altarSpawnPoint;

    private void Start()
    {
        floorData = GetComponent<CreateRoom>();
        altarSpawnRoom = floorData.GetRandomRoom();
        altarSpawnPoint = altarSpawnRoom.rootCoords;
        altarSpawnPoint.x += altarSpawnRoom.width / 2;
        altarSpawnPoint.y += altarSpawnRoom.height / 2;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameObject.FindGameObjectWithTag("Finish") == null)
            {
                Instantiate(nextFloorAltarPrefab, altarSpawnPoint, Quaternion.identity);
            }
            else
            {
                Debug.Log("Prefab już istnieje na scenie.");
            }
        }
    }
}
