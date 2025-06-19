using System.Collections;
using UnityEngine;

public class CharacterHolder : MonoBehaviour
{
    public static CharacterHolder Instance { get; private set; }

    public CharacterSO characterData;
    public CharacterInstance characterInstance;
    public bool gracz;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // albo Debug.LogError, jeœli nie chcesz niszczyæ
            return;
        }
        Instance = this;
    }


    private void Start()
    {
        Debug.Log("Start Called");
        InitializeCharacter();
    }

    public void InitializeCharacter()
    {
        if (characterInstance == null && characterData != null)
        {
            Debug.Log("Tworzê characterInstance");
            characterInstance = new CharacterInstance(characterData);
        }

        if (characterInstance != null)
        {
            characterInstance.SetAsPlayer();
            gracz = true;
            Debug.Log($"SetAsPlayer wywo³ane. IsPlayerControlled: {characterInstance.IsPlayerControlled}");
        }
    }



    public static void LogStatGainWithDelay(CharacterStats.StatType stat, int bonus)
    {
        if (Instance != null)
            Instance.StartCoroutine(Instance.DelayedLog($"[DEBUG] Added +{bonus} to {stat} due to floor scaling."));
    }

    private IEnumerator DelayedLog(string message)
    {
        yield return null;
        if (LogManager.Instance != null)
            LogManager.Instance.Log(message);
    }
}
