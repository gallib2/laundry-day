using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private float xPosition = 0f;
    private float yPosition = 0f;
    private bool toFollowPlayerOnX;
    private bool followPlayerOnY;


    void Start()
    {
        CameraOption cameraOptions;
        if (Settings.Instance.IsSetCameraOptions)
        {
            cameraOptions = Settings.Instance.ChosenCameraOption;
        }
        else
        {
            cameraOptions = Settings.Instance.CameraOptions[5];
        }
    
         toFollowPlayerOnX = cameraOptions.ToFollowOnX;
        followPlayerOnY = cameraOptions.FollowOnY;
         Camera.main.fieldOfView = cameraOptions.FieldOfView;
         offset = cameraOptions.Offset;
         transform.rotation = cameraOptions.Angle;
        
       /* else
        {
            offset = new Vector3(0.0f, 5.0f, -20f);
            Camera.main.fieldOfView = 65;
        }*/
    }

    void Update()
    {
        xPosition = toFollowPlayerOnX ? target.position.x : 0;
        yPosition = followPlayerOnY ? target.position.y : 0;
        transform.position = new Vector3(xPosition, yPosition, target.position.z) + offset;
    }
}
