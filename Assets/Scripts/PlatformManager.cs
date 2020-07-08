using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : Singleton<PlatformManager>
{
    [SerializeField] private GameObject[] platformPrefabs;
    private List<GameObject> platforms;

    private float zPositionOffset = 0.0f;

    private const float Z_PLATFORM_SIZE = 100.0f;

    private void OnEnable()
    {
        GameManager.OnRestart += Initialise;
    }

    private void OnDisable()
    {
        GameManager.OnRestart -= Initialise;
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        zPositionOffset = 0f;

        if (platforms == null)
        {
            platforms = new List<GameObject>();
            for (int i = 0; i < platformPrefabs.Length; i++)
            {
                platforms.Add(Instantiate(platformPrefabs[i]));
            }
        }

        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].transform.position = new Vector3(0, 0, i * Z_PLATFORM_SIZE);
        }
        zPositionOffset += Z_PLATFORM_SIZE * platforms.Count;
    }

    public void RecyclePlatform(GameObject platform)
    {
        platform.transform.position = new Vector3(0, 0, zPositionOffset);
        zPositionOffset += Z_PLATFORM_SIZE;
    }
}
