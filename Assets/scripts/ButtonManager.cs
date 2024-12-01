using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Metoda do wy³¹czania gry
    public void QuitGame()
    {
        Debug.Log("Zamykanie gry...");
        Application.Quit();
    }

    // Metoda do przechodzenia na inn¹ scenê
    public void LoadScene(string sceneName)
    {
        Debug.Log($"£adowanie sceny: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}

