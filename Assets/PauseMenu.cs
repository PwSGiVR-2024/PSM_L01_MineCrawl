using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button quitButton;
    [SerializeField] GameObject settingsWindow;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed!");
            if (!settingsWindow.active)
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // zatrzymuje gr�
        isPaused = true;
    }

    void QuitGame()
    {
        Time.timeScale = 1f; // przywr�� gr� przed wyj�ciem


        BattleTransferData.playerInstance = null;


        SceneManager.LoadScene("Main menu");
    }
}
