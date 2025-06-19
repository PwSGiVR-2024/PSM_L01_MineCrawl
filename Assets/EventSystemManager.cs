using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private void Start()
    {
        var eventSystems = FindObjectsOfType<EventSystem>();
        Debug.Log($"Liczba EventSystemów: {eventSystems.Length}");
        foreach (var es in eventSystems)
        {
            Debug.Log($"EventSystem: {es.gameObject.name}, Active: {es.gameObject.activeSelf}");
        }
    }
    void Awake()
    {
        var eventSystems = FindObjectsOfType<EventSystem>();

        foreach (var es in eventSystems)
        {
            if (es.gameObject.name != "EventManager")
            {
                Debug.Log($"Usuwam EventSystem: {es.gameObject.name}");
                Destroy(es.gameObject);
            }
            else
            {
                Debug.Log($"Zostawiam EventSystem: {es.gameObject.name}");
                DontDestroyOnLoad(es.gameObject);
            }
        }
    }



}
