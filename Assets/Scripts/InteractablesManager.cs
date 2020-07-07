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

    [SerializeField] private float minimumSecondsBetweenClothingItemSpawns;
    [SerializeField] private float maximumSecondsBetweenClothingItemSpawns;
    [SerializeField] private float minimumSecondsBetweenExtraLifeItemSpawns;
    [SerializeField] private float maximumSecondsBetweenExtraLifeItemSpawns;
    [SerializeField] private float lowSpawnY;
    [SerializeField] private float highSpawnY;

    private void Start()
    {
        InitialisePools();
        SpawnClothingItem();
        SpawnExtraLifeItem();

    }

    private void InitialisePools()
    {
        clothingItemsPool = new List<ClothingItem>();
        for (int i = 0; i < clothingItemsPreFabs.Length; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                ClothingItem newClothingItem = Instantiate(clothingItemsPreFabs[i]);
                newClothingItem.gameObject.SetActive(false);
                clothingItemsPool.Add(newClothingItem);
            }
        }

        extraLifeItemsPool = new List<ExtraLifeItem>();
        for (int j = 0; j < 5; j++)
        {
            ExtraLifeItem newExtraLifeItem = Instantiate(extraLifeItemPreFab);
            newExtraLifeItem.gameObject.SetActive(false);
            extraLifeItemsPool.Add(newExtraLifeItem);
        }
    }

    private void SpawnClothingItem()
    {
        if (clothingItemsPool ==null || clothingItemsPool.Count <= 0 )
        {
            Debug.LogError("Pool's closed!");
            return;
        }
        int index = Random.Range(0, clothingItemsPool.Count);
        ClothingItem newClothingItem = clothingItemsPool[index];
        clothingItemsPool.RemoveAt(index);
        newClothingItem.gameObject.SetActive(true);
        float x = World.LanesXs[Random.Range(0, World.LanesXs.Length)];
        float y = Random.Range(0, 4) == 3 ? highSpawnY : lowSpawnY; //HARDCODED
        float z = player.transform.position.z + spawnDistanceFromPlayer;
        newClothingItem.transform.position = new Vector3(x, y, z);

        Invoke("SpawnClothingItem",
            Random.Range(minimumSecondsBetweenClothingItemSpawns, maximumSecondsBetweenClothingItemSpawns));
    }

    private void SpawnExtraLifeItem()
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
        float x = World.LanesXs[Random.Range(0, World.LanesXs.Length)];
        float y = Random.Range(0, 4) == 3 ? highSpawnY : lowSpawnY; //HARDCODED
        float z = player.transform.position.z + spawnDistanceFromPlayer;
        newExtraLifeItem.transform.position = new Vector3(x, y, z);

        Invoke("SpawnExtraLifeItem",
            Random.Range(minimumSecondsBetweenExtraLifeItemSpawns, maximumSecondsBetweenExtraLifeItemSpawns));
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
