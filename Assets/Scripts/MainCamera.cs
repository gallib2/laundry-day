using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    void Start()
    {
        if (Settings.Instance.IsSetCameraOptions)
        {
            transform.localPosition = Settings.Instance.ChosenCameraOption.Position;
            transform.localRotation = Settings.Instance.ChosenCameraOption.Rotation;
        }
    }
}
