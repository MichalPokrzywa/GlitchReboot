using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Scene currentScene;
    public LoadingCanvas loadingCanvas;
    public UnityEvent sceneLoaded;
    public void LoadScene(Scene scene)
    {
        currentScene = scene;
        StartCoroutine(PlayTransitionAndLoad());
    }

    public void LoadSceneAdditive(Scene scene)
    {
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

    private IEnumerator PlayTransitionAndLoad()
    {
        yield return new WaitUntil(() => TVCloseRenderFeature.Instance != null);

        TVCloseRenderFeature.Instance.PlayCloseEffect(2f, () =>
        {
            loadingCanvas.ShowLoadingCanvas();
            StartCoroutine(LoadSceneAsyncWithUI());
        });
    }

    private IEnumerator LoadSceneAsyncWithUI()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)currentScene);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log($"Loading progress: {progress}");
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        sceneLoaded.Invoke();

        loadingCanvas.HideLoadingCanvas();
        TVCloseRenderFeature.Instance.PlayOpenEffect(2f);
    }

}

public enum Scene
{
    Initialization = 0,
    TestLevel = 1,
    MainMenu = 2,
    Tutorial = 3,
    Level2 = 4,
    Level3Ghost = 5,
    level4Chess = 6,

    //Add another scene like this ^
}