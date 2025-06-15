using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    [SerializeField] private TMP_Text logText;
    [SerializeField] private int maxLines = 10;

    private readonly Queue<string> logLines = new Queue<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Log(string message)
    {
        if (logLines.Count >= maxLines)
            logLines.Dequeue();

        logLines.Enqueue(message);
        UpdateLogText();
    }

    private void UpdateLogText()
    {
        logText.text = string.Join("\n", logLines);
    }
}
