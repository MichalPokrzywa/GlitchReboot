using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject creditsPanel;

    bool creditsPanelToggle = false;
    bool settingsPanelToggle = false;

    void Start()
    {
        RegisterToEvents();
        PanelManager.Instance.UpdateEventSystemSelectedObject(playButton.gameObject);
    }

    void OnDestroy()
    {
        UnregisterFromEvents();
    }

    void Play()
    {
        DependencyManager.sceneLoader.LoadScene(Scene.Tutorial);
    }

    void ShowSettingsPanel()
    {
        // Toggle the settings panel visibility
        settingsPanelToggle = !settingsPanelToggle;
        creditsPanelToggle = false;

        settingsPanel?.SetActive(settingsPanelToggle);
        creditsPanel?.SetActive(creditsPanelToggle);
    }

    void ShowCreditsPanel()
    {
        // Toggle the credits panel visibility
        creditsPanelToggle = !creditsPanelToggle;
        settingsPanelToggle = false;

        settingsPanel?.SetActive(settingsPanelToggle);
        creditsPanel?.SetActive(creditsPanelToggle);
    }

    void ExitGame()
    {
        Application.Quit();
        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    void RegisterToEvents()
    {
        if (playButton != null)
            playButton.onClick.AddListener(Play);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettingsPanel);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCreditsPanel);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }


    void UnregisterFromEvents()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(Play);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(ShowSettingsPanel);

        if (creditsButton != null)
            creditsButton.onClick.RemoveListener(ShowCreditsPanel);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(ExitGame);
    }

}
