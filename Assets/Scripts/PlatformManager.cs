using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : Singleton<PlatformManager>
{
    [SerializeField] private GameObject[] platformPrefabs;
    private float zPositionOffset = 0.0f;

    private const float Z_PLATFORM_SIZE = 100.0f;

    void Start()
    {
        for (int i = 0; i < platformPrefabs.Length; i++)
        {
            Instantiate(platformPrefabs[i], new Vector3(0, 0, i * Z_PLATFORM_SIZE), Quaternion.identity);
            zPositionOffset += Z_PLATFORM_SIZE;
        }
    }

    public void RecyclePlatform(GameObject platform)
    {
        platform.transform.position = new Vector3(0, 0, zPositionOffset);
        zPositionOffset += Z_PLATFORM_SIZE;
    }
}
