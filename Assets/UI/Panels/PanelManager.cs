using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using static TipsPanel;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] TipsPanel tipsPanel;
    [SerializeField] PausePanel pausePanel;

    public bool canBePaused = true;

    HashSet<eTipType> shownTips = new HashSet<eTipType>();
    Coroutine closeTipCoroutine;
    EventSystem eventSystem;

    const float tipDisplayDuration = 5f;

    void Awake()
    {
        if (tipsPanel == null)
            tipsPanel = FindFirstObjectByType<TipsPanel>();

        if (pausePanel == null)
            pausePanel = FindFirstObjectByType<PausePanel>();

        RegisterToEvents();
    }

    void OnDisable()
    {
        UnregisterFromEvents();
    }

    void Update()
    {
        if (InputManager.Instance.IsPausePressed() && canBePaused
            && DependencyManager.sceneLoader.CurrentScene != Scene.MainMenu)
        {
            pausePanel.TogglePanel();

            var virtualMouse = FindFirstObjectByType<CustomVirtualMouseInput>(FindObjectsInactive.Include);
            if (pausePanel.isOpen && virtualMouse != null)
                virtualMouse.gameObject.SetActive(false);
            else if (!pausePanel.isOpen && virtualMouse != null)
                virtualMouse.gameObject.SetActive(true);
        }
    }

    public void ShowTipsOnce(params eTipType[] tips)
    {
        if (tips == null || tips.Length == 0)
            return;

        List<eTipType> newTips = new();
        foreach (var tip in tips)
        {
            if (tip != eTipType.None && !shownTips.Contains(tip))
            {
                newTips.Add(tip);
                shownTips.Add(tip);
            }
        }

        if (newTips.Count == 0)
            return;

        if (tipsPanel.isOpen && closeTipCoroutine != null)
        {
            StopCoroutine(closeTipCoroutine);
            closeTipCoroutine = null;
        }

        tipsPanel.Open();
        tipsPanel.SetTips(newTips.ToArray());

        closeTipCoroutine = StartCoroutine(CloseTip());
    }

    public void ReturnToMenu()
    {
        DependencyManager.sceneLoader.LoadScene(Scene.MainMenu);
    }

    void EnsureEventSystemRef()
    {
        if (eventSystem == null)
            eventSystem = FindFirstObjectByType<EventSystem>();
    }

    void OnSceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
    {
        ResetPanels();
        StopAllCoroutines();
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        ResetPanels();
        pausePanel.gameObject.SetActive(DependencyManager.sceneLoader.CurrentScene != Scene.MainMenu);
    }

    void RegisterToEvents()
    {
        if (pausePanel != null)
            pausePanel.onPanelOpen += OnPanelOpen;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void UnregisterFromEvents()
    {
        if (pausePanel != null)
            pausePanel.onPanelOpen -= OnPanelOpen;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void ResetPanels()
    {
        tipsPanel?.Close();
        pausePanel?.Close();
        pausePanel?.ResetState();
    }

    void OnPanelOpen()
    {
        EventSystem.current.SetSelectedGameObject(pausePanel.FirstItemToSelect);
    }

    IEnumerator CloseTip()
    {
        yield return new WaitForSeconds(tipDisplayDuration);
        tipsPanel?.Close();
    }
}
