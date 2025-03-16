using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Scene currentScene;

    public void LoadScene(Scene scene)
    {
        currentScene = scene;

        //LSS_LoadingScreen.LoadScene(scene.ToString(), "Standard");
        SceneManager.LoadSceneAsync((int)currentScene);
    }

    public void LoadSceneAdditive(Scene scene)
    {
        //LSS_LoadingScreen.LoadSceneAdditive(scene.ToString(), "Standard");
        SceneManager.LoadSceneAsync((int)currentScene,LoadSceneMode.Additive);
    }

    public void UnloadSceneAdditive(Scene scene)
    {
        SceneManager.UnloadSceneAsync((int)scene);
    }
    public void UnloadSceneAdditive(int index)
    {
        SceneManager.UnloadSceneAsync(index);
    }

    public bool IsSceneLoaded(Scene scene)
    {
        // Pobieramy scenê na podstawie indeksu
        UnityEngine.SceneManagement.Scene unityScene = SceneManager.GetSceneByBuildIndex((int)scene);

        // Sprawdzamy, czy scena istnieje i czy jest za³adowana
        return unityScene.isLoaded;
    }
}

public enum Scene
{
    Initialization = 0,
    TestLevel = 1,
    MainMenu = 2,

    //Add another scene like this ^
}