using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button controlsButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject creditsPanel;

    GameObject activePanel;

    void Start()
    {
        playButton?.onClick.AddListener(Play);
        settingsButton?.onClick.AddListener(() => TogglePanel(settingsPanel));
        controlsButton?.onClick.AddListener(() => TogglePanel(controlsPanel));
        creditsButton?.onClick.AddListener(() => TogglePanel(creditsPanel));
        exitButton?.onClick.AddListener(ExitGame);

        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    void OnDestroy()
    {
        playButton?.onClick.RemoveListener(Play);
        settingsButton?.onClick.RemoveAllListeners();
        controlsButton?.onClick.RemoveAllListeners();
        creditsButton?.onClick.RemoveAllListeners();
        exitButton?.onClick.RemoveListener(ExitGame);
    }

    void Play()
    {
        DependencyManager.sceneLoader.LoadScene(Scene.Tutorial);
    }

    void TogglePanel(GameObject panelToShow)
    {
        if (panelToShow == null) return;

        bool shouldShow = panelToShow != activePanel;
        settingsPanel?.SetActive(false);
        creditsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);

        if (shouldShow)
        {
            panelToShow.SetActive(true);
            activePanel = panelToShow;
        }
        else
        {
            activePanel = null;
        }
    }

    void ExitGame()
    {
        Application.Quit();
        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
