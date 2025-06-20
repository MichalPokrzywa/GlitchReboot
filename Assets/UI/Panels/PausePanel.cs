using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [SerializeField] Button controlsButton;
    [SerializeField] Button returnToMenuButton;
    [SerializeField] Camera mainCamera;
    [SerializeField] Volume depthOfField;
    [SerializeField] FirstPersonController firstPersonController;

    void Awake()
    {
        if (controlsButton != null)
        {
            controlsButton.onClick.AddListener(ControlsPanel);
            firstItemToSelect = controlsButton.gameObject;
        }

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenu);

        onPanelOpen += RestoreTime;
        onPanelClose += StopTime;
    }

    void OnDestroy()
    {
        if (controlsButton != null)
            controlsButton.onClick.RemoveListener(ControlsPanel);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.RemoveListener(ReturnToMenu);

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

    void ControlsPanel()
    {

    }
}
