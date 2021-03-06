﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private GameObject inputButtons;

    private static bool useButtons;
    private static bool debuggingOnPC;
    private static bool swapLeftAndRight;


    private void OnEnable()
    {
        GameManager.OnRestart += Initialise;
    }

    private void OnDisable()
    {
        GameManager.OnRestart -= Initialise;
    }

    private void Initialise()
    {
        useButtons = Settings.Instance.CurrentBlock.useInputButtons;
        inputButtons.SetActive(useButtons);
        swapLeftAndRight = Settings.Instance.ChosenCameraOption.SwapLeftAndRight;

        debuggingOnPC = (SystemInfo.deviceType != DeviceType.Handheld);
    }

    public static InputType GetInput()
    {
        InputType inputType = InputType.NONE;
        if (useButtons)
        {
            inputType = InputButtonsManager.Instance.GetInput();
        }
        else
        {
            if (MobileInput.Instance.SwipeLeft)
            {
                inputType = InputType.LEFT;
            }
            else if (MobileInput.Instance.SwipeRight)
            {
                inputType = InputType.RIGHT;
            }
            else if (MobileInput.Instance.SwipeUp)
            {
                inputType = InputType.UP;
            }
        }

        if (debuggingOnPC)
        {
            if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                inputType = InputType.RIGHT;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                inputType = InputType.LEFT;
            }
       else if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                inputType = InputType.UP;
            }

        }

        if (swapLeftAndRight)
        {
            if(inputType == InputType.RIGHT)
            {
                inputType = InputType.LEFT;
            }
            else if (inputType == InputType.LEFT)
            {
                inputType = InputType.RIGHT;
            }
        }
        return inputType;
    }
}

public enum InputType
{
    NONE = 0, LEFT = 1, RIGHT = 2, UP =3,
}