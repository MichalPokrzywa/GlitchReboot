using UnityEngine;

public class DisableLogsInBuild : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void DisableLogs()
    {
        // Disable Debug.Log, Debug.LogWarning, and Debug.LogError in builds
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }
}
