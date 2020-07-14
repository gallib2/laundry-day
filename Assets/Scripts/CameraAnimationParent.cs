using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationParent : MonoBehaviour
{
    [SerializeField] MainCamera mainCamera;
    public void ReleaseCamera()
    {
        mainCamera.Release();
    }
}
