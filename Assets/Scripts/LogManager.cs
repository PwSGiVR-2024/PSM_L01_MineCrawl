using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;
    public TMP_Text scoreText;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int maxLines = 200;
    public static bool hasShownIntro = false;
    public static readonly Queue<string> persistentLogLines = new Queue<string>();
    private readonly string[] introMessages = new string[]
    {
        "Welcome to the world of pain (world of unfinished games and unfulfilled dev's hopes).",
        "Brace yourself... not for glory, but for bugs, balance issues, and burnout.",
        "This game is 90% ambition, 10% working features. Enjoy!",
        "You are the chosen one... probably because no one else showed up.",
        "Enter a world where dreams go to die — and so do you.",
        "Made with love, stress, and just a bit of spaghetti code.",
        "If you can read this, the UI didn't break. That's a good start."
    };
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (!hasShownIntro)
        {
            Log(GetRandomIntro());
            hasShownIntro = true;
        }
        UpdateLogText();
    }
    public string GetRandomIntro()
    {

        int index = Random.Range(0, introMessages.Length);
        return introMessages[index];
    }
    public void Log(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string fullMessage = $"[{timestamp}] {message}";

        if (persistentLogLines.Count >= maxLines)
            persistentLogLines.Dequeue();

        persistentLogLines.Enqueue(fullMessage);
        UpdateLogText();
    }


    private void UpdateLogText()
    {
        if (logText != null)
        {
            logText.text = string.Join("\n", persistentLogLines);

            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases(); // wymusza layout UI
                scrollRect.verticalNormalizedPosition = 0f; // scroll do do³u
            }
        }
    }
}
