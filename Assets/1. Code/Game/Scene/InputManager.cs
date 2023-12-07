using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    public static GameInput MainInput { get; set; }

    public static bool MCQOptionRight => MainInput.MCQMapping.OptionRight.activeControl != null;
    public static bool MCQOptionRightDown => MainInput.MCQMapping.OptionRight.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionRight.activeControl as ButtonControl).wasPressedThisFrame;
    public static bool MCQOptionRightUp => MainInput.MCQMapping.OptionRight.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionRight.activeControl as ButtonControl).wasReleasedThisFrame;

    public static bool MCQOptionTop => MainInput.MCQMapping.OptionTop.activeControl != null;
    public static bool MCQOptionTopDown => MainInput.MCQMapping.OptionTop.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionTop.activeControl as ButtonControl).wasPressedThisFrame;
    public static bool MCQOptionTopUp => MainInput.MCQMapping.OptionTop.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionTop.activeControl as ButtonControl).wasReleasedThisFrame;

    public static bool MCQOptionLeft => MainInput.MCQMapping.OptionLeft.activeControl != null;
    public static bool MCQOptionLeftDown => MainInput.MCQMapping.OptionLeft.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionLeft.activeControl as ButtonControl).wasPressedThisFrame;
    public static bool MCQOptionLeftUp => MainInput.MCQMapping.OptionLeft.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionLeft.activeControl as ButtonControl).wasReleasedThisFrame;

    public static bool MCQOptionBottom => MainInput.MCQMapping.OptionBottom.activeControl != null;
    public static bool MCQOptionBottomDown => MainInput.MCQMapping.OptionBottom.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionBottom.activeControl as ButtonControl).wasPressedThisFrame;
    public static bool MCQOptionBottomUp => MainInput.MCQMapping.OptionBottom.activeControl as ButtonControl != null && (MainInput.MCQMapping.OptionBottom.activeControl as ButtonControl).wasReleasedThisFrame;

    public static bool GamepadConnected { get; private set; } = false;

    void Awake ()
    {
        MainInput = new GameInput();
        MainInput.Enable();
    }

    void Update()
    {
        // if(MainInput.MCQMapping.OptionRight.activeControl != null)
            // Debug.Log("MCQ right triggered");
        if(GamepadConnected != (Gamepad.current != null))
            GamepadConnected = (Gamepad.current != null);
        
        
    }
}
