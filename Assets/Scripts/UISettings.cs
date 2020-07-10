using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField]
    private Slider sliderPlayerMinimumSpeed;
    [SerializeField]
    private Text textPlayerMinimumSpeed;
    [SerializeField]
    private Slider sliderPlayerMaximumSpeed;
    [SerializeField]
    private Text textPlayerMaximumSpeed;
    [SerializeField]
    private Slider sliderPlayerJumpForce;
    [SerializeField]
    private Text textPlayerJumpForce;
    [SerializeField]
    private Dropdown dropdownCameraPositionOptions;

    [SerializeField] private Text livesAtStartText;
    [SerializeField] private Slider livesAtStartSlider;

    public bool SetMinSpeed { get; set; }
    public bool SetMaxSpeed { get; set; }

    private void Start()
    {
        textPlayerMinimumSpeed.text = sliderPlayerMinimumSpeed.value.ToString();
        textPlayerMaximumSpeed.text = sliderPlayerMaximumSpeed.value.ToString();
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
        bool canSetSpeedValues = sliderPlayerMinimumSpeed.value <= sliderPlayerMaximumSpeed.value;
        
        if(canSetSpeedValues)
        {
            Settings.Instance.PlayerMinimumSpeed = (int)sliderPlayerMinimumSpeed.value;
            Settings.Instance.PlayerMaximumSpeed = (int)sliderPlayerMaximumSpeed.value;
        }
        else
        {
            // min value cant be bigger then the max. so if min < max the min is set for both
            Settings.Instance.PlayerMinimumSpeed = (int)sliderPlayerMinimumSpeed.value;
            Settings.Instance.PlayerMaximumSpeed = (int)sliderPlayerMinimumSpeed.value;
        }

        int index = dropdownCameraPositionOptions.value;
        Settings.Instance.ChosenCameraOption = Settings.Instance.CameraOptions[index];
        Settings.Instance.IsSetCameraOptions = true;

        Settings.Instance.PlayeJumpForce = sliderPlayerJumpForce.value;
        Settings.Instance.LivesAtStart = (int)livesAtStartSlider.value;
    }

    public void SetLivesAtStart()
    {
        livesAtStartText.text = livesAtStartSlider.value.ToString();
        Settings.Instance.LivesAtStartIsSet = true;
    }

    public void SetPlayerMinimumSpeed()
    {
        textPlayerMinimumSpeed.text = sliderPlayerMinimumSpeed.value.ToString();
        Settings.Instance.SetMinSpeed = true;
    }

    public void SetPlayerMaximumSpeed()
    {
        textPlayerMaximumSpeed.text = sliderPlayerMaximumSpeed.value.ToString();
        Settings.Instance.SetMaxSpeed = true;
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
