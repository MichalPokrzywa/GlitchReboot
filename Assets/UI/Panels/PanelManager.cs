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

    const float tipDisplayDuration = 5f;

    Coroutine closeTipCoroutine;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        ResetFields();
    }

    void OnSceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
    {
        ResetPanels();
        StopAllCoroutines();
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        ResetFields();
        ResetPanels();
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

    void ResetPanels()
    {
        shownTips?.Clear();
        pausePanel?.ResetState();
    }

    void ResetFields()
    {
        if (tipsPanel == null)
            tipsPanel = FindObjectOfType<TipsPanel>();

        if (pausePanel == null)
            pausePanel = FindObjectOfType<PausePanel>();
    }

    IEnumerator CloseTip()
    {
        yield return new WaitForSeconds(tipDisplayDuration);
        tipsPanel?.Close();
    }
}
