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

    const float VOLUME_MULTIPLIER = 50f;
    const float MOUSE_SENSITIVITY_MULTIPLIER = 50f;
    const float GAMEPAD_SENSITIVITY_MULTIPLIER = 10f;

    void Start()
    {
        inverseMouseYToggle?.onValueChanged.AddListener(ToggleInverseMouseY);
        volumeSlider?.onValueChanged.AddListener(ChangeVolume);
        mouseSensitivitySlider?.onValueChanged.AddListener(ChangeMouseSensitivity);
        gamepadSensitivitySlider?.onValueChanged.AddListener(ChangeGamepadSensitivity);

        volumeSlider.value = DependencyManager.audioManager.soundsVolume * VOLUME_MULTIPLIER;
        mouseSensitivitySlider.value = InputManager.Instance.mouseSensitivity * MOUSE_SENSITIVITY_MULTIPLIER;
        gamepadSensitivitySlider.value = InputManager.Instance.gamepadSensitivity * GAMEPAD_SENSITIVITY_MULTIPLIER;
        inverseMouseYToggle.isOn = InputManager.Instance.invertInputY;

        volumeValue.text = volumeSlider.value.ToString("F0");
        mouseSensitivityText.text = mouseSensitivitySlider.value.ToString("F0");
        gamepadSensitivityText.text = gamepadSensitivitySlider.value.ToString("F0");
    }

    void ChangeVolume(float value)
    {
        float normalized = value / 100f;
        DependencyManager.audioManager.SetMusicVolume(normalized);
        DependencyManager.audioManager.SetSoundVolume(normalized);
        volumeValue.text = volumeSlider.value.ToString("F0");
    }

    void ChangeMouseSensitivity(float sliderValue)
    {
        float sensitivity = sliderValue / MOUSE_SENSITIVITY_MULTIPLIER;
        InputManager.Instance.mouseSensitivity = sensitivity;
        mouseSensitivityText.text = sliderValue.ToString("F0");
    }

    void ChangeGamepadSensitivity(float sliderValue)
    {
        float sensitivity = sliderValue / GAMEPAD_SENSITIVITY_MULTIPLIER;
        InputManager.Instance.gamepadSensitivity = sensitivity;
        gamepadSensitivityText.text = sliderValue.ToString("F0");
    }

    void ToggleInverseMouseY(bool isOn)
    {
        InputManager.Instance.invertInputY = isOn;
    }
}
