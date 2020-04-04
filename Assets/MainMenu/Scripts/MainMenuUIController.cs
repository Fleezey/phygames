using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuUIController : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
