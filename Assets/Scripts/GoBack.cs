using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBack : MonoBehaviour
{
public void Back(int index)
    {
        SceneManager.LoadScene(sceneBuildIndex: index);
    }
}
