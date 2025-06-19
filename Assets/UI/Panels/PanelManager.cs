using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TipsPanel;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] TipsPanel tipsPanel;
    [SerializeField] PausePanel pausePanel;

    HashSet<eTipType> shownTips = new HashSet<eTipType>();
    Coroutine closeTipCoroutine;

    const float tipDisplayDuration = 5f;

    void Awake()
    {
        if (tipsPanel == null)
            tipsPanel = FindObjectOfType<TipsPanel>();

        if (pausePanel == null)
            pausePanel = FindObjectOfType<PausePanel>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void Update()
    {
        if (InputManager.Instance.IsPausePressed() && DependencyManager.sceneLoader.currentScene != Scene.MainMenu)
        {
            pausePanel.TogglePanel();
        }
    }

    void OnSceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
    {
        ResetPanels();
        StopAllCoroutines();
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        ResetPanels();
        pausePanel.gameObject.SetActive(DependencyManager.sceneLoader.currentScene != Scene.MainMenu);
    }

    public void ShowTipOnce(eTipType tipType)
    {
        if (tipType == eTipType.None || shownTips.Contains(tipType))
            return;

        if (tipsPanel.isOpen)
        {
            StopCoroutine(closeTipCoroutine);
            closeTipCoroutine = null;
        }

        shownTips.Add(tipType);
        tipsPanel.Open();
        tipsPanel.SetText(tipType);
        closeTipCoroutine = StartCoroutine(CloseTip());
    }

    public void ReturnToMenu()
    {
        DependencyManager.sceneLoader.LoadScene(Scene.MainMenu);
        NarrativeSystem.Instance.ResetNarrative();
    }

    void ResetPanels()
    {
        tipsPanel?.Close();
        pausePanel?.Close();
        pausePanel?.ResetState();
    }

    IEnumerator CloseTip()
    {
        yield return new WaitForSeconds(tipDisplayDuration);
        tipsPanel?.Close();
    }
}
