using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PausePanel : Panel
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Volume depthOfField;
    [SerializeField] FirstPersonController firstPersonController;

    bool EnsurePlayerRef()
    {
        if (firstPersonController == null)
            firstPersonController = FindObjectOfType<FirstPersonController>();

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

    void OnDisable()
    {
        ResetState();
    }

    public override void Close(Action onComplete = null)
    {
        base.Close();

        // stop the game time
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (EnsurePlayerRef())
        {
            firstPersonController.lockCursor = true;
            firstPersonController.StartMovement();
        }
        if (EnsureCameraRef())
            depthOfField.enabled = false;
    }

    public override void Open(Action onComplete = null)
    {
        base.Open(() => Time.timeScale = 0f);

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

    public void ReturnToMenu()
    {
        PanelManager.Instance.ReturnToMenu();
    }
}
