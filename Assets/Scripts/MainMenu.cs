using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void TransitionToGame()
    {
        SceneManager.LoadScene("Scenes/Level1");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Scenes/Title");
    }
}
