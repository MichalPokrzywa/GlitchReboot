using UnityEngine;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject creditsPanel;

    bool creditsPanelToggle = false;
    bool settingsPanelToggle = false;

    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Common.TutorialSceneName);
    }

    public void ShowSettingsPanel()
    {
        // Toggle the settings panel visibility
        settingsPanelToggle = !settingsPanelToggle;
        creditsPanelToggle = false;

        settingsPanel?.SetActive(settingsPanelToggle);
        creditsPanel?.SetActive(creditsPanelToggle);
    }

    public void ShowCreditsPanel()
    {
        // Toggle the credits panel visibility
        creditsPanelToggle = !creditsPanelToggle;
        settingsPanelToggle = false;

        settingsPanel?.SetActive(settingsPanelToggle);
        creditsPanel?.SetActive(creditsPanelToggle);
    }

    public void ExitGame()
    {
        Application.Quit();
        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
