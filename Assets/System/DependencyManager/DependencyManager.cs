using UnityEngine;

public class DependencyManager : MonoBehaviour
{
    public static AudioManager audioManager;
    public static DataManager dataManager = new DataManager();
    //public static SaveManager saveManager;
    public static SceneLoader sceneLoader; 


    [SerializeField] Scene initiallyLoadedScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        audioManager = GetComponentInChildren<AudioManager>();
        sceneLoader = GetComponentInChildren<SceneLoader>();

        SetResolution();

        sceneLoader.LoadScene(initiallyLoadedScene);
    }

    void SetResolution()
    {
        int maxWidth = Screen.currentResolution.width;
        int maxHeight = Screen.currentResolution.height;

        // Czy urz�dzenie obs�uguje rozdzielczo�� 2560 x 1440
        if (maxWidth >= 2560 && maxHeight >= 1440)
        {
            Screen.SetResolution(2560, 1440, true);
        }
        else
        {
            // Ustaw rozdzielczo�� na Full HD (1920 x 1080)
            Screen.SetResolution(1920, 1080, true);
            Debug.LogWarning("Rozdzielczo�� 2560x1440 nie jest obs�ugiwana. Ustawiono Full HD (1920x1080).");
        }
    }
}