using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationBootstrap : MonoBehaviour
{
    const string SceneName = "Initialization";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
            var candidate = SceneManager.GetSceneAt(sceneIndex);
            if (candidate.name == SceneName)
                return;
        }

        Debug.Log($"Loading {SceneName} scene.");
        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }
}
