using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    [SerializeField] private TMP_Text logText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int maxLines = 200;

    private static readonly Queue<string> persistentLogLines = new Queue<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

       UpdateLogText();
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
