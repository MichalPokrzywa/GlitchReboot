using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] Toggle inverseMouseYToggle;

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider gamepadSensitivitySlider;

    [SerializeField] TMP_Text volumeValue;
    [SerializeField] TMP_Text mouseSensitivityText;
    [SerializeField] TMP_Text gamepadSensitivityText;

    void Start()
    {
        inverseMouseYToggle?.onValueChanged.AddListener(ToggleInverseMouseY);
        volumeSlider?.onValueChanged.AddListener(ChangeVolume);
        mouseSensitivitySlider?.onValueChanged.AddListener(ChangeMouseSensitivity);
        gamepadSensitivitySlider?.onValueChanged.AddListener(ChangeGamepadSensitivity);

        volumeSlider.value = DependencyManager.audioManager.soundsVolume * 100f;
        mouseSensitivitySlider.value = InputManager.Instance.mouseSensitivity * 100f;
        gamepadSensitivitySlider.value = InputManager.Instance.gamepadSensitivity * 100f;
        inverseMouseYToggle.isOn = InputManager.Instance.invertInputY;

        volumeValue.text = volumeSlider.value.ToString("F0");
        mouseSensitivityText.text = mouseSensitivitySlider.value.ToString("F0");
        gamepadSensitivityText.text = gamepadSensitivitySlider.value.ToString("F0");
    }

    void ChangeVolume(float value)
    {
        DependencyManager.audioManager.SetMusicVolume(value / 100f);
        DependencyManager.audioManager.SetSoundVolume(value / 100f);
        volumeValue.text = volumeSlider.value.ToString("F0");
    }

    void ChangeMouseSensitivity(float value)
    {
        float defaultSensitivity = InputManager.Instance.mouseSensitivity;
        value = Mathf.Clamp(value / mouseSensitivitySlider.maxValue, mouseSensitivitySlider.minValue, 1f);
        InputManager.Instance.mouseSensitivity = value;
        mouseSensitivityText.text = mouseSensitivitySlider.value.ToString("F0");
    }

    void ChangeGamepadSensitivity(float value)
    {
        float defaultSensitivity = InputManager.Instance.gamepadSensitivity;
        value = Mathf.Clamp(value / gamepadSensitivitySlider.maxValue, gamepadSensitivitySlider.minValue, 1f);
        InputManager.Instance.gamepadSensitivity = value;
        gamepadSensitivityText.text = gamepadSensitivitySlider.value.ToString("F0");
    }

    void ToggleInverseMouseY(bool isOn)
    {
        InputManager.Instance.invertInputY = isOn;
    }
}
