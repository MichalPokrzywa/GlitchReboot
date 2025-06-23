using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InputManager;

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
    GameObject firstItemToSelect;
    List<Button> allButtons = new List<Button>();

    void Start()
    {
        playButton?.onClick.AddListener(Play);
        settingsButton?.onClick.AddListener(() => TogglePanel(settingsPanel));
        controlsButton?.onClick.AddListener(() => TogglePanel(controlsPanel));
        creditsButton?.onClick.AddListener(() => TogglePanel(creditsPanel));
        exitButton?.onClick.AddListener(ExitGame);

        allButtons = new List<Button>() { playButton, settingsButton, controlsButton, creditsButton, exitButton };

        firstItemToSelect = playButton.gameObject;
        UpdateEventSystemSelectedGO(InputManager.Instance.CurrentDevice);
        InputManager.Instance.onControlsChanged += UpdateEventSystemSelectedGO;
        InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.MENU, true);

        settingsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);
        creditsPanel?.SetActive(false);

        foreach (var button in allButtons)
            button.interactable = true;
    }

    void OnDestroy()
    {
        playButton?.onClick.RemoveListener(Play);
        settingsButton?.onClick.RemoveAllListeners();
        controlsButton?.onClick.RemoveAllListeners();
        creditsButton?.onClick.RemoveAllListeners();
        exitButton?.onClick.RemoveListener(ExitGame);

        if (InputManager.IsInstanceShuttingDown)
            InputManager.Instance.onControlsChanged -= UpdateEventSystemSelectedGO;
    }

    void Play()
    {
        DependencyManager.sceneLoader.LoadScene(Scene.Tutorial);
        InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.MENU, null);
        foreach (var button in allButtons)
            button.interactable = false;
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

    void UpdateEventSystemSelectedGO(InputManager.DeviceType deviceType)
    {
        if (deviceType == InputManager.DeviceType.Gamepad)
        {
            if (firstItemToSelect != null)
                EventSystem.current.SetSelectedGameObject(firstItemToSelect);
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
