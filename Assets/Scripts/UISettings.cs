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
    private Dropdown dropdownCameraPositionOptions;

    public bool SetMinSpeed { get; set; }
    public bool SetMaxSpeed { get; set; }

    void Start()
    {
        textPlayerMinimumSpeed.text = sliderPlayerMinimumSpeed.value.ToString();
        textPlayerMaximumSpeed.text = sliderPlayerMaximumSpeed.value.ToString();

        for (int i = 0; i < Settings.Instance.CameraOptionsNumber; i++)
        {
            dropdownCameraPositionOptions.options[i].text = Settings.Instance.CameraOptions[i].Name;
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
        Settings.Instance.SetCameraOptions = true;
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

    public void SetCancel()
    {
        Settings.Instance.SetMaxSpeed = false;
        Settings.Instance.SetMinSpeed = false;
        Settings.Instance.SetCameraOptions = false;
    }
}
