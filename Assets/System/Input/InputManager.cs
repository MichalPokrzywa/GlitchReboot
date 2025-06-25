using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.DocumentationSortingAttribute;

public class InputManager : Singleton<InputManager>
{
    public enum DeviceType
    {
        KeyboardMouse,
        Gamepad
    }

    public enum CursorVisibilityRequestSource
    {
        GAMEPAD,
        PAUSE,
        MENU,
        TABLET
    }

    public DeviceType CurrentDevice { get; private set; }
    public InputSystem_Actions CurrentControls => currentControls;
    public Action<DeviceType> onControlsChanged;
    public Action<float> OnSensitivityChanged;

    public float mouseSensitivity;
    public float gamepadSensitivity;
    public bool invertInputY = false;

    PlayerInput playerInput;
    InputSystem_Actions currentControls;
    List<bool?> cursorVisibilityRequests = new List<bool?>();

    bool isCursorVisible = false;

    const string GamepadScheme = "Gamepad";
    const float defaultMouseSensitivity = 0.4f;
    const float defaultGamepadSensitivity = 2f;
    const bool defaultInvertInputY = false;

    void Awake()
    {
        foreach (var item in Enum.GetNames(typeof(CursorVisibilityRequestSource)))
        {
            cursorVisibilityRequests.Add(null);
        }

        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += OnControlsChanged;

        CurrentDevice = playerInput.currentControlScheme == GamepadScheme
            ? DeviceType.Gamepad
            : DeviceType.KeyboardMouse;

        RestoreDefaultSettings();
        Cursor.lockState = CursorLockMode.Locked;

        currentControls = new InputSystem_Actions();
        currentControls.Enable();
    }

    #region ControlGetters
    public float GetMoveHorizontal() => currentControls.Player.Move.ReadValue<Vector2>().x;
    public float GetMoveVertical() => currentControls.Player.Move.ReadValue<Vector2>().y;
    public float GetLookHorizontal()
    {
        var rawInput = currentControls.Player.Look.ReadValue<Vector2>().x;
        return CurrentDevice == DeviceType.Gamepad ? rawInput * gamepadSensitivity : rawInput * mouseSensitivity;
    }
    public float GetLookVertical()
    {
        var rawInput = currentControls.Player.Look.ReadValue<Vector2>().y;
        var value = CurrentDevice == DeviceType.Gamepad ? rawInput * gamepadSensitivity : rawInput * mouseSensitivity;
        return value * (invertInputY ? -1 : 1);
    }
    public bool IsCrouchHeld() => currentControls.Player.Crouch.IsPressed();
    public bool IsSprintHeld() => currentControls.Player.Sprint.IsPressed();
    public bool IsZoomHeld() => currentControls.Player.Zoom.IsPressed();
    public bool IsJumpPressed() => currentControls.Player.Jump.WasPerformedThisFrame();
    public bool IsFirePressed() => currentControls.Player.Fire.WasPerformedThisFrame();
    public bool IsInteractPressed() => currentControls.Player.Interact.WasPerformedThisFrame();
    public bool IsInteractWithTabletPressed() => currentControls.Player.TabletInteract.WasPerformedThisFrame();
    public bool IsPausePressed() => currentControls.Player.Pause.WasPerformedThisFrame();
    public bool IsSpawnMarkerPressed() => currentControls.Player.SpawnMarker.WasPerformedThisFrame();
    public bool IsNextMarkerPressed() => currentControls.Player.NextMarker.WasPerformedThisFrame();
    public bool IsPreviousMarkerPressed() => currentControls.Player.PrevMarker.WasPerformedThisFrame();
    #endregion

    public string GetBinding(InputAction action)
    {
        if (action == null)
            return string.Empty;

        string scheme = playerInput.currentControlScheme;

        return action.GetBindingDisplayString(
            bindingMask: InputBinding.MaskByGroup(scheme),
            options: InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
    }

    public InputAction GetMarkerAction(int index)
    {
        switch (index)
        {
            case 0: return currentControls.Player.Marker1;
            case 1: return currentControls.Player.Marker2;
            case 2: return currentControls.Player.Marker3;
            case 3: return currentControls.Player.Marker4;
            case 4: return currentControls.Player.Marker5;
            case 5: return currentControls.Player.Marker6;
            case 6: return currentControls.Player.Marker7;
            case 7: return currentControls.Player.Marker8;
            case 8: return currentControls.Player.Marker9;
            default: return null;
        }
    }

    public float GetCurrentCursorSensitivity()
    {
        if (CurrentDevice == DeviceType.Gamepad)
            return gamepadSensitivity;
        else
            return mouseSensitivity;
    }

    public void CursorVisibilityState(CursorVisibilityRequestSource source, bool? state)
    {
        isCursorVisible = false;
        cursorVisibilityRequests[(int)source] = state;

        foreach (var item in cursorVisibilityRequests)
        {
            if (item == true)
            {
                isCursorVisible = true;
                break;
            }
            if (item == false)
            {
                break;
            }
        }

        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }

    void RestoreDefaultSettings()
    {
        mouseSensitivity = defaultMouseSensitivity;
        gamepadSensitivity = defaultGamepadSensitivity;
        invertInputY = defaultInvertInputY;
    }

    void OnControlsChanged(PlayerInput playerInput)
    {
        CurrentDevice = playerInput.currentControlScheme == GamepadScheme
            ? DeviceType.Gamepad
            : DeviceType.KeyboardMouse;

        onControlsChanged?.Invoke(CurrentDevice);

        if (CurrentDevice == DeviceType.Gamepad)
            CursorVisibilityState(CursorVisibilityRequestSource.GAMEPAD, false);
        else
            CursorVisibilityState(CursorVisibilityRequestSource.GAMEPAD, null);
    }
}