using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotsUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] imgsScreenshots;
    [SerializeField] private Dropdown dropdownCameraPositionOptions;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetImageWithDropDownValue();
    }

    public void SetImageWithDropDownValue()
    {
        int index = dropdownCameraPositionOptions.value;
        bool isValid = index >= 0 && index < imgsScreenshots.Length;
        if (isValid)
        {
            image.sprite = imgsScreenshots[index];
        }
    }
}
