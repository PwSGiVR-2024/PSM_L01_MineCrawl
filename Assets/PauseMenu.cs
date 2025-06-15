using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button quitButton;

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
            if (isPaused)
                Resume();
            else
                Pause();
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
        Time.timeScale = 0f; // zatrzymuje grê
        isPaused = true;
    }

    void QuitGame()
    {
        Time.timeScale = 1f; // przywróæ grê przed wyjœciem


        BattleTransferData.playerInstance = null;

        //// Jeœli masz dostêp do CharacterCreationData i chcesz te¿ j¹ wyczyœciæ:
        //var creationData = FindObjectOfType<CharacterBuilder>()?.characterDataTemplate;
        //if (creationData != null)
        //{
        //    creationData.Clear();
        //}

        SceneManager.LoadScene("Main menu");
    }
}
