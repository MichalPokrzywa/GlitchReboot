using System;
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

    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] Camera mainCamera;
    [SerializeField] Volume depthOfField;
    [SerializeField] FirstPersonController firstPersonController;

    GameObject activePanel;

    void Awake()
    {
        controlsButton?.onClick.AddListener(() => TogglePanel(controlsPanel));
        settingsButton?.onClick.AddListener(() => TogglePanel(settingsPanel));
        returnToMenuButton?.onClick.AddListener(ReturnToMenu);
        backButton?.onClick.AddListener(() => TogglePanel(null));

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

        AudioListener.pause = false;

        if (EnsurePlayerRef())
        {
            firstPersonController.lockCursor = true;
            firstPersonController.StartMovement();
        }
        if (EnsureCameraRef())
            depthOfField.enabled = false;
    }

    public override void Open()
    {
        base.Open();

        AudioListener.pause = true;

        if (EnsurePlayerRef())
        {
            firstPersonController.lockCursor = false;
            firstPersonController.StopMovement();
        }
        if (EnsureCameraRef())
            depthOfField.enabled = true;
    }

    public override void ResetState()
    {
        base.ResetState();
        Time.timeScale = 1f;
        if (firstPersonController != null)
        {
            firstPersonController.lockCursor = true;
            firstPersonController.StartMovement();
        }
        AudioListener.pause = false;
        if (depthOfField != null)
            depthOfField.enabled = false;
    }

    void ReturnToMenu()
    {
        PanelManager.Instance.ReturnToMenu();
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
