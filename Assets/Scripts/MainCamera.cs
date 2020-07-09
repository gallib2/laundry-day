using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offset;
    private float xPosition = 0f;
    private bool toFollowPlayerOnX;

    void Start()
    {
        if (Settings.Instance.IsSetCameraOptions)
        {
            toFollowPlayerOnX = Settings.Instance.ChosenCameraOption.ToFollowOnX;
            Camera.main.fieldOfView = Settings.Instance.ChosenCameraOption.FieldOfView;
            offset = Settings.Instance.ChosenCameraOption.Offset;
        }
        else
        {
            offset = new Vector3(0.0f, 5.0f, -20f);
            Camera.main.fieldOfView = 65;
        }
    }

    void Update()
    {
        xPosition = toFollowPlayerOnX ? target.position.x : 0;
        transform.position = new Vector3(xPosition, target.position.y, target.position.z) + offset;
    }
}
