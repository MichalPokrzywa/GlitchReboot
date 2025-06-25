using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [SerializeField] Button controlsButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button returnToMenuButton;
    [SerializeField] Button backButton;
    [SerializeField] Button restartScene;

    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] Camera mainCamera;
    [SerializeField] Volume depthOfField;
    [SerializeField] FirstPersonController firstPersonController;
    [SerializeField] Image raycastBlocker;

    GameObject activePanel;
    List<Button> allButtons = new List<Button>();

    void Awake()
    {
        controlsButton?.onClick.AddListener(() => TogglePanel(controlsPanel));
        settingsButton?.onClick.AddListener(() => TogglePanel(settingsPanel));
        returnToMenuButton?.onClick.AddListener(ReturnToMenu);
        backButton?.onClick.AddListener(() => TogglePanel(null));
        restartScene?.onClick.AddListener(RestartLevel);

        allButtons = new List<Button>() { controlsButton, settingsButton, returnToMenuButton, backButton, restartScene };

        firstItemToSelect = controlsButton.gameObject;

        onPanelOpen += RestoreTime;
        onPanelClose += StopTime;
    }

    void OnDestroy()
    {
        controlsButton?.onClick.RemoveAllListeners();
        settingsButton?.onClick.RemoveAllListeners();
        returnToMenuButton.onClick.RemoveListener(ReturnToMenu);
        backButton?.onClick.RemoveListener(() => TogglePanel(null));
        restartScene?.onClick.RemoveListener(RestartLevel);

        onPanelOpen -= RestoreTime;
        onPanelClose -= StopTime;
    }

    void OnDisable()
    {
        ResetState();
    }

    bool EnsurePlayerRef()
    {
        if (firstPersonController == null)
            firstPersonController = FindFirstObjectByType<FirstPersonController>();

        return firstPersonController != null;
    }

    bool EnsureCameraRef()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return false;

        if (depthOfField == null)
            mainCamera.TryGetComponent(out depthOfField);

        return depthOfField != null;
    }

    void RestoreTime()
    {
        Time.timeScale = 0f;
    }

    void StopTime()
    {
        Time.timeScale = 1f;
    }

    public override void Close()
    {
        base.Close();

        foreach (var button in allButtons)
            button.interactable = true;

        raycastBlocker.enabled = false;
        AudioListener.pause = false;

        InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.PAUSE, null);

        if (EnsurePlayerRef())
            firstPersonController.StartMovement();

        if (EnsureCameraRef())
            depthOfField.enabled = false;

        TogglePanel(null);
    }

    public override void Open()
    {
        base.Open();

        raycastBlocker.enabled = true;
        AudioListener.pause = true;

        InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.PAUSE, true);

        if (EnsurePlayerRef())
            firstPersonController.StopMovement();

        if (EnsureCameraRef())
            depthOfField.enabled = true;
    }

    public override void ResetState()
    {
        base.ResetState();
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (firstPersonController != null)
            firstPersonController.StartMovement();

        InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.PAUSE, null);

        if (depthOfField != null)
            depthOfField.enabled = false;

        foreach (var button in allButtons)
            button.interactable = true;
    }

    void ReturnToMenu()
    {
        PanelManager.Instance.ReturnToMenu();
        foreach (var button in allButtons)
            button.interactable = false;
    }

    void RestartLevel()
    {
        DependencyManager.sceneLoader.LoadScene(DependencyManager.sceneLoader.CurrentScene);
    }

    void TogglePanel(GameObject panelToShow)
    {
        bool shouldShow = (panelToShow != null && panelToShow != activePanel);
        settingsPanel?.SetActive(false);
        controlsPanel?.SetActive(false);

        if (shouldShow)
        {
            panelToShow.SetActive(true);
            activePanel = panelToShow;
            backButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        }
        else
        {
            activePanel = null;
            backButton.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(firstItemToSelect);
        }
    }
}
