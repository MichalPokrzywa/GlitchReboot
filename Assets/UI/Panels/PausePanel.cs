using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PausePanel : Panel
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Volume depthOfField;
    [SerializeField] FirstPersonController firstPersonController;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (depthOfField == null)
        {
            if (!mainCamera.TryGetComponent<Volume>(out depthOfField))
            {
                Debug.LogError("Depth of Field Volume component not found on the main camera.");
            }
        }

        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
            if (firstPersonController == null)
            {
                Debug.LogError("FirstPersonController not found in the scene.");
            }
        }

        ResetState();
    }

    void OnDisable()
    {
        ResetState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    public override void Close(Action onComplete = null)
    {
        base.Close();
        // stop the game time
        Time.timeScale = 1f;
        AudioListener.pause = false;
        firstPersonController.lockCursor = true;
        firstPersonController.StartMovement();
        depthOfField.enabled = false;
    }

    public override void Open(Action onComplete = null)
    {
        base.Open(() => Time.timeScale = 0f);
        firstPersonController.lockCursor = false;
        firstPersonController.StopMovement();
        AudioListener.pause = true;
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
        // fix? scene loader?
        UnityEngine.SceneManagement.SceneManager.LoadScene(Common.MainMenuSceneName);
    }

    public void OpenSettings()
    {

    }
}
