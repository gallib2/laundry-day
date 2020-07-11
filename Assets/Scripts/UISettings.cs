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

    private void Start()
    {     
        InitialiseUIElements();
    }

    private void InitialiseUIElements()
    {
        dropdownCameraPositionOptions.options.Clear();
        for (int i = 0; i < Settings.Instance.CameraOptionsNumber; i++)
        {
            dropdownCameraPositionOptions.options.Add
                (new Dropdown.OptionData(Settings.Instance.CameraOptions[i].Name));
        }

        ForceUIElementsToShowCurrentSettingsValues();
    }

    public void ForceUIElementsToShowCurrentSettingsValues()
    {
        Settings.SettingsBlock settingsBlock = Settings.Instance.CurrentBlock;
        ForceUIElementsToShowSettingsValues(ref settingsBlock);

    }

    public void ForceUIElementsToShowDefaultValues()
    {
        Settings.SettingsBlock settingsBlock = Settings.Instance.DefaultBlock;
        ForceUIElementsToShowSettingsValues(ref settingsBlock);
    }

    public void ForceUIElementsToShowSettingsValues(ref Settings.SettingsBlock settingsBlock)
    {
        sliderPlayerMinimumZSpeed.value = settingsBlock.playerMinimumZSpeed;
        sliderPlayerMaximumZSpeed.value = settingsBlock.playerMaximumZSpeed;
        playerXSpeedSlider.value = settingsBlock.playerXSpeed;
        sliderPlayerJumpForce.value = settingsBlock.playerJumpForce;
        livesAtStartSlider.value = settingsBlock.livesAtStart;

        forbidSwitchingLanesWhileAirborneToggle.isOn = settingsBlock.forbidSwitchingLanesWhileAirborne;

        dropdownCameraPositionOptions.value = settingsBlock.cameraOptionsIndex;

        UpdateTexts();

    }


    public void SetChangesAccordingToUI()
    {
        bool canSetSpeedValues = sliderPlayerMinimumZSpeed.value <= sliderPlayerMaximumZSpeed.value;
        Settings.SettingsBlock newSettingsBlock;
        if(canSetSpeedValues)
        {
            newSettingsBlock.playerMinimumZSpeed = (int)sliderPlayerMinimumZSpeed.value;
            newSettingsBlock.playerMaximumZSpeed = (int)sliderPlayerMaximumZSpeed.value;
        }
        else
        {
            // min value cant be bigger then the max. so if min < max the min is set for both
            newSettingsBlock.playerMinimumZSpeed = (int)sliderPlayerMinimumZSpeed.value;
            newSettingsBlock.playerMaximumZSpeed = (int)sliderPlayerMinimumZSpeed.value;
        }

        newSettingsBlock.playerXSpeed = (int)playerXSpeedSlider.value;

        int index = dropdownCameraPositionOptions.value;
        newSettingsBlock.cameraOptionsIndex = index;

        newSettingsBlock.playerJumpForce = sliderPlayerJumpForce.value;
        newSettingsBlock.livesAtStart = (int)livesAtStartSlider.value;

        newSettingsBlock.forbidSwitchingLanesWhileAirborne = forbidSwitchingLanesWhileAirborneToggle.isOn;


        Settings.Instance.CurrentBlock = newSettingsBlock;
    }

    public void UpdateTexts()
    {
        textPlayerMinimumZSpeed.text = sliderPlayerMinimumZSpeed.value.ToString();

        textPlayerMaximumZSpeed.text = sliderPlayerMaximumZSpeed.value.ToString();

        playerXSpeedText.text = playerXSpeedSlider.value.ToString();

        textPlayerJumpForce.text = sliderPlayerJumpForce.value.ToString();

        livesAtStartText.text = livesAtStartSlider.value.ToString();
    }

    /*public void SetCancel()
    {
        Settings.Instance.SetMaxSpeed = false;
        Settings.Instance.SetMinSpeed = false;
        Settings.Instance.IsSetCameraOptions = false;
    }*/
}
