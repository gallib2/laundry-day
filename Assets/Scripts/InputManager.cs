using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private static bool useButtons;
    private static bool debuggingOnPC;
    [SerializeField] private GameObject inputButtons;

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
        inputButtons.SetActive(useButtons);//TODO: Might be more appropriate to move this to InputButtonsManager
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

        return inputType;
    }
}

public enum InputType
{
    NONE = 0, LEFT = 1, RIGHT = 2, UP =3,
}