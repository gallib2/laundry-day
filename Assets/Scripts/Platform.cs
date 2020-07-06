using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        PlatformManager.Instance?.RecyclePlatform(gameObject);
    }
}
