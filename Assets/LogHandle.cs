using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogHandle : MonoBehaviour
{
    [SerializeField] private TMP_Text logTextSerialized;
    [SerializeField] private TMP_Text scoreTextSerialized;
    [SerializeField] private ScrollRect scrollRectSerialized;

    public static TMP_Text logText;
    public static TMP_Text scoreText;
    public static ScrollRect scrollRect;

    void Start()
    {
        logText = logTextSerialized;
        scoreText = scoreTextSerialized;
        scrollRect = scrollRectSerialized;

        LogManager.Instance.AttachUI(logText, scrollRect);
        LogManager.Instance.AttachScore(scoreText);
        if (!LogManager.hasShownIntro)
        {
            LogManager.Instance.Log(LogManager.Instance.GetRandomIntro());
            LogManager.hasShownIntro = true;
        }
    }
}
