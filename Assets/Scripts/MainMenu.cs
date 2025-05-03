using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Wystartowa³eœ grê");
        SceneManager.LoadScene (sceneBuildIndex:1);
    }
    public void RandomGame()
    {
        Debug.Log("Random game started");
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Settings()
    {
        print("Open settings");
    }
    public void Volume()
    {
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().MuteMusic();
    }

}
