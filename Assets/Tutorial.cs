using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);



    }
    public void clear()
    {
        GameObject.Destroy(gameObject);
    }
}
