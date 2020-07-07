using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : Singleton<InteractablesManager>
{
    [SerializeField] private ClothingItem[] clothingItemsPreFabs;
    [SerializeField] private ExtraLifeItem extraLifeItemPreFab;
    private static List< ClothingItem> clothingItemsPool;
    private static List<ExtraLifeItem> extraLifeItemsPool;

    [SerializeField] Player player;
    [SerializeField] private float spawnDistanceFromPlayer;

    [SerializeField] private UInt32 minimumUnitsBetweenClothingItemSpawns;
    [SerializeField] private UInt32 maximumUnitsBetweenClothingItemSpawns;
    [SerializeField] private UInt32 minimumUnitsBetweenExtraLifeItemSpawns;
    [SerializeField] private UInt32 maximumUnitsBetweenExtraLifeItemSpawns;
    private UInt32 nextClothingItemSpawn;
    private UInt32 nextExtraLifeItemSpawn;
    [SerializeField] private float lowSpawnY;
    [SerializeField] private float highSpawnY;

    private void Start()
    {
        Initialise();

    }

    private void Initialise()
    {
        InitialisePools();
       // SpawnClothingItem();
    }

    private void InitialisePools()
    {
        if(clothingItemsPool == null)
        {
            clothingItemsPool = new List<ClothingItem>();
            for (int i = 0; i < clothingItemsPreFabs.Length; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    ClothingItem newClothingItem = Instantiate(clothingItemsPreFabs[i]);
                    newClothingItem.gameObject.SetActive(false);
                    clothingItemsPool.Add(newClothingItem);
                }
            }
        }

        if (extraLifeItemsPool == null)
        {
            extraLifeItemsPool = new List<ExtraLifeItem>();
            for (int j = 0; j < 5; j++)
            {
                ExtraLifeItem newExtraLifeItem = Instantiate(extraLifeItemPreFab);
                newExtraLifeItem.gameObject.SetActive(false);
                extraLifeItemsPool.Add(newExtraLifeItem);
            }
        }

    }

    private void Update()
    {
        ManageSpawning();

    }

    private void ManageSpawning()
    {
        UInt32 playerMileage = Player.Instance.MileageInUnits;
        if (playerMileage >= nextClothingItemSpawn)
        {
            SpawnClothingItem(playerMileage);
        }
        if (playerMileage >= nextExtraLifeItemSpawn)
        {
            SpawnExtraLifeItem(playerMileage);
        }
    }

    private void SpawnClothingItem(UInt32 playerMileage)
    {
        if (clothingItemsPool ==null || clothingItemsPool.Count <= 0 )
        {
            Debug.LogError("Pool's closed!");
            return;
        }
        int index = UnityEngine.Random.Range(0, clothingItemsPool.Count);
        ClothingItem newClothingItem = clothingItemsPool[index];
        clothingItemsPool.RemoveAt(index);
        newClothingItem.gameObject.SetActive(true);
        float x = World.LanesXs[UnityEngine.Random.Range(0, World.LanesXs.Length)];//TODO: build lane slot system
        float y = UnityEngine.Random.Range(0, 4) == 3 ? highSpawnY : lowSpawnY; //HARDCODED
        float z = player.transform.position.z + spawnDistanceFromPlayer;
        newClothingItem.transform.position = new Vector3(x, y, z);

        nextClothingItemSpawn = (UInt32)(playerMileage +
             UnityEngine.Random.Range((int)minimumUnitsBetweenClothingItemSpawns, (int)maximumUnitsBetweenClothingItemSpawns));
    }

    private void SpawnExtraLifeItem(UInt32 playerMileage)
    {
        //TODO: merege this function with SpawnClothingItem
        if (extraLifeItemsPool == null || extraLifeItemsPool.Count <= 0)
        {
            Debug.LogError("Pool's closed!");
            return;
        }
        int index = 0;
        ExtraLifeItem newExtraLifeItem = extraLifeItemsPool[index];
        extraLifeItemsPool.RemoveAt(index);
        newExtraLifeItem.gameObject.SetActive(true);
        float x = World.LanesXs[UnityEngine.Random.Range(0, World.LanesXs.Length)];
        float y = UnityEngine.Random.Range(0, 4) == 3 ? highSpawnY : lowSpawnY; //HARDCODED
        float z = player.transform.position.z + spawnDistanceFromPlayer;
        newExtraLifeItem.transform.position = new Vector3(x, y, z);

        nextExtraLifeItemSpawn = (UInt32)(playerMileage +
             UnityEngine.Random.Range((int)minimumUnitsBetweenExtraLifeItemSpawns, (int)maximumUnitsBetweenExtraLifeItemSpawns));
    }

    public static void RecycleInteractable(Interactable interactable)
    {
        interactable.gameObject.SetActive(false);

        if (interactable is ClothingItem)
        {
            clothingItemsPool.Add(interactable as ClothingItem);
        }
        else if (interactable is ExtraLifeItem)
        {
            extraLifeItemsPool.Add(interactable as ExtraLifeItem);
        }


    }
}
