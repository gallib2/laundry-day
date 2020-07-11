using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField]
    private Slider sliderPlayerMinimumZSpeed;
    [SerializeField]
    private Text textPlayerMinimumZSpeed;
    [SerializeField]
    private Slider sliderPlayerMaximumZSpeed;
    [SerializeField]
    private Text textPlayerMaximumZSpeed;
    [SerializeField] private Slider playerXSpeedSlider;
    [SerializeField] private Text playerXSpeedText;
    [SerializeField]
    private Slider sliderPlayerJumpForce;
    [SerializeField]
    private Text textPlayerJumpForce;
    [SerializeField]
    private Dropdown dropdownCameraPositionOptions;
    [SerializeField] Toggle forbidSwitchingLanesWhileAirborneToggle;

 [SerializeField] private Text livesAtStartText;
    [SerializeField] private Slider livesAtStartSlider;

    public bool SetMinSpeed { get; set; }
    public bool SetMaxSpeed { get; set; }

    private void Start()
    {
        textPlayerMinimumZSpeed.text = sliderPlayerMinimumZSpeed.value.ToString();
        textPlayerMaximumZSpeed.text = sliderPlayerMaximumZSpeed.value.ToString();
        playerXSpeedText.text = playerXSpeedSlider.value.ToString();

        textPlayerJumpForce.text = sliderPlayerJumpForce.value.ToString();
        livesAtStartText.text = livesAtStartSlider.value.ToString();

        dropdownCameraPositionOptions.options.Clear();
        for (int i = 0; i < Settings.Instance.CameraOptionsNumber; i++)
        {
            dropdownCameraPositionOptions.options.Add(new Dropdown.OptionData(Settings.Instance.CameraOptions[i].Name));
        }
    }

    public void SetChanges()
    {
        bool canSetSpeedValues = sliderPlayerMinimumZSpeed.value <= sliderPlayerMaximumZSpeed.value;
        
        if(canSetSpeedValues)
        {
            Settings.Instance.PlayerMinimumSpeed = (int)sliderPlayerMinimumZSpeed.value;
            Settings.Instance.PlayerMaximumSpeed = (int)sliderPlayerMaximumZSpeed.value;

        }
        else
        {
            // min value cant be bigger then the max. so if min < max the min is set for both
            Settings.Instance.PlayerMinimumSpeed = (int)sliderPlayerMinimumZSpeed.value;
            Settings.Instance.PlayerMaximumSpeed = (int)sliderPlayerMinimumZSpeed.value;
        }

        Settings.Instance.PlayerXSpeed = (int)playerXSpeedSlider.value;

        int index = dropdownCameraPositionOptions.value;
        Settings.Instance.ChosenCameraOption = Settings.Instance.CameraOptions[index];
        Settings.Instance.IsSetCameraOptions = true;

        Settings.Instance.PlayeJumpForce = sliderPlayerJumpForce.value;
        Settings.Instance.LivesAtStart = (int)livesAtStartSlider.value;

        Settings.Instance.ForbidSwitchingLanesWhileAirborne = forbidSwitchingLanesWhileAirborneToggle.isOn;
    }

    public void SetForbidSwitchingLanesWhileAirborne()
    {
        Settings.Instance.ForbidSwitchingLanesWhileAirborneIsSet = true;
    }

    public void SetLivesAtStart()
    {
        livesAtStartText.text = livesAtStartSlider.value.ToString();
        Settings.Instance.LivesAtStartIsSet = true;
    }

    public void SetPlayerMinimumSpeed()
    {
        textPlayerMinimumZSpeed.text = sliderPlayerMinimumZSpeed.value.ToString();
        Settings.Instance.SetMinSpeed = true;
    }

    public void SetPlayerMaximumSpeed()
    {
        textPlayerMaximumZSpeed.text = sliderPlayerMaximumZSpeed.value.ToString();
        Settings.Instance.SetMaxSpeed = true;
    }

    public void SetPlayerXSpeed()
    {
        playerXSpeedText.text = playerXSpeedSlider.value.ToString();
        Settings.Instance.PlayerXSpeedIsSet = true;
    }

    public void SetPlayerJumpForce()
    {
        textPlayerJumpForce.text = sliderPlayerJumpForce.value.ToString();
        Settings.Instance.IsSetJumpForce = true;
    }


    public void SetCancel()
    {
        Settings.Instance.SetMaxSpeed = false;
        Settings.Instance.SetMinSpeed = false;
        Settings.Instance.IsSetCameraOptions = false;
    }
}
