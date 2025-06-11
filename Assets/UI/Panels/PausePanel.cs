using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class PausePanel : MonoBehaviour
{
    [SerializeField] RectTransform contentRect;
    [SerializeField] Camera mainCamera;
    [SerializeField] Volume depthOfField;
    [SerializeField] FirstPersonController firstPersonController;

    bool panelToggle = false;
    bool animationInProgress = false;

    const float scaleDuration = 0.25f;

    Tween tween;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !animationInProgress)
        {
            panelToggle = !panelToggle;

            if (panelToggle)
                OpenPanel();
            else
                ClosePanel();
        }
    }

    public void ClosePanel()
    {
        animationInProgress = true;
        panelToggle = false;
        // stop the game time
        Time.timeScale = 1f;
        AudioListener.pause = false;
        firstPersonController.lockCursor = true;
        firstPersonController.StartMovement();
        tween = contentRect.DOScale(Vector3.zero, scaleDuration).SetUpdate(true).OnComplete(() => animationInProgress = false);
        depthOfField.enabled = false;
    }

    void OpenPanel()
    {
        animationInProgress = true;
        panelToggle = true;
        firstPersonController.lockCursor = false;
        firstPersonController.StopMovement();
        AudioListener.pause = true;
        tween = contentRect.DOScale(new Vector3(1,1,1), scaleDuration).SetUpdate(true).OnComplete(() =>
        {
            Time.timeScale = 0f;
            animationInProgress = false;
        });

        depthOfField.enabled = true;
    }

    void ResetState()
    {
        panelToggle = false;
        animationInProgress = false;
        firstPersonController.lockCursor = true;
        firstPersonController.StartMovement();
        AudioListener.pause = false;
        depthOfField.enabled = false;
        Time.timeScale = 1f;
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Common.MainMenuSceneName);
    }

    public void OpenSettings()
    {

    }
}
