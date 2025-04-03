using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] ParticleSystem miningEffect;
    public void StartGame()
    {
        Debug.Log("Wystartowa³eœ grê");
        //SceneManager.LoadScene (sceneBuildIndex:/*Put the number here*/);
    }
    public void RandomGame()
    {
        Debug.Log("Random game started");
    }
    public void HighScore()
    {
        print("wypisz highscore");
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
    private void Update()
    {
        //Input.mousePosition;
    }
}
