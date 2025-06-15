using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] SettingsMenu menu;
    public static void StartGame()
    {
        Debug.Log("Wystartowa³eœ grê");
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void RandomGame()
    {
        Debug.Log("Random game started");

        CharacterBuilder builder = FindFirstObjectByType<CharacterBuilder>();
        if (builder == null)
        {
            Debug.LogError("Nie znaleziono obiektu z CharacterBuilder na scenie!");
            return;
        }

        builder.RandomCharacter(builder.availableRaces, builder.availableClasses);
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void Settings()
    {
        print("Open settings");
        menu.OpenSettings();
    }
    public void Volume()
    {
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().MuteMusic();
    }

}
