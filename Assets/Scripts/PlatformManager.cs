using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : Singleton<PlatformManager>
{
    [SerializeField] private Platform[] platformPrefabs;
    [SerializeField] private Material[] platformMaterials;
    [SerializeField] private Doors doors;
    [SerializeField] private int platformsFromBehindAtInitialisation = 1;
    
    private List<Platform> platforms;
    private int materialsIndex = 0;

   private float zPositionOffset = 0.0f;
    private const float Z_PLATFORM_SIZE = 100f;

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
        zPositionOffset = platformsFromBehindAtInitialisation * -Z_PLATFORM_SIZE;

        if (platforms == null)
        {
            platforms = new List<Platform>();
            for (int i = 0; i < platformPrefabs.Length; i++)
            {
                platforms.Add(Instantiate(platformPrefabs[i]));
            }
        }

        for (int i = 0; i < platforms.Count; i++)
        {
            MovePlatformToItsAppropriatePlace(platforms[i]);
        }
    }

    public void RecyclePlatform(Platform platform)
    {
        MovePlatformToItsAppropriatePlace(platform);
    }

    private void MovePlatformToItsAppropriatePlace(Platform platform)
    {
        platform.transform.position = new Vector3(0, 0, zPositionOffset);
        int newMaterialIndex = platformMaterials.Length - 1;
        // Adjusting the number slightly to make sure material changes happen in the right spots
        float platformNormalisedZ = platform.transform.position.z + 0.5f;
        for (int i = 1; i < InteractablesManager.ClothingItemsGenerationPoints.Length; i++)
        {
            if (platformNormalisedZ <= (float)InteractablesManager.ClothingItemsGenerationPoints[i].mileage)
            {
                newMaterialIndex = i - 1;
                break;
            }
        }
        if(newMaterialIndex!= materialsIndex)
        {
            materialsIndex = newMaterialIndex;
            doors.MoveToNewLocation(platform.transform.position);
        }
        Material material = platformMaterials[materialsIndex];

        platform.SetMaterial(material);
        zPositionOffset += Z_PLATFORM_SIZE;
    }
}
