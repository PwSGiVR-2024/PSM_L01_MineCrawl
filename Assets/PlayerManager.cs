using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public CharacterInstance PlayerCharacter { get; private set; }

    [SerializeField] private CharacterSO characterTemplate;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tworzymy gracza na podstawie template
        PlayerCharacter = new CharacterInstance(characterTemplate);
    }
}
