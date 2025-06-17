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

    private readonly Queue<string> persistentLogLines = new Queue<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

       // UpdateLogText();
    }

    public void Log(string message)
    {
        if (persistentLogLines.Count >= maxLines)
            persistentLogLines.Dequeue();

        persistentLogLines.Enqueue(message);
        //UpdateLogText();
    }

    //private void UpdateLogText()
    //{
    //    if (logText != null)
    //    {
    //        logText.text = string.Join("\n", persistentLogLines);

    //        if (scrollRect != null)
    //        {
    //            Canvas.ForceUpdateCanvases(); // wymusza layout UI
    //            scrollRect.verticalNormalizedPosition = 1f; // scroll do do³u
    //        }
    //    }
    //}
}
