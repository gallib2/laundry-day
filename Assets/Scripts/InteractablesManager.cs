using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : Singleton<InteractablesManager>
{
    [SerializeField] private ClothingItem clothingItemsPreFab;
    [SerializeField] private ExtraLifeItem extraLifeItemPreFab;
    private static List< ClothingItem> clothingItemsPool;
    private static List<ExtraLifeItem> extraLifeItemsPool;
    private static List<Interactable> lentInteractables;

    List<Interactable> interactablesToBeSpawned;

    [SerializeField] Player player;
    [SerializeField] private float spawnDistanceFromPlayer;

    [SerializeField] private UInt32 minimumUnitsBetweenClothingItemSpawns;
    [SerializeField] private UInt32 maximumUnitsBetweenClothingItemSpawns;
    [SerializeField] private UInt32 minimumUnitsBetweenExtraLifeItemSpawns;
    [SerializeField] private UInt32 maximumUnitsBetweenExtraLifeItemSpawns;
    private UInt32 nextClothingItemSpawn;
    private UInt32 nextExtraLifeItemSpawn;

    [SerializeField] private ClothingItemsGenerationPoint[] clothingItemsGenerationPoints;
    [Serializable]
    private struct ClothingItemsGenerationPoint
    {
        public UInt32 mileage;
        public int maxClothingItemsGenerated;
    }

    private SpawnSpot[,] spawnSpots;
    private enum SpawnSpot : byte
    {
        FREE = 0, OCCUPIED = 1,
    }

    [Serializable]
    private class ClothingTypeProperties
    {
        [SerializeField] private ClothingType clothingType; 
        [SerializeField] private Mesh[] meshes;
        [SerializeField] private Material[] materials;
        public ClothingType GetClothingType()
        {
            return clothingType;
        }

        public Mesh GetRandomMesh()
        {
            return meshes[UnityEngine.Random.Range(0, meshes.Length)];
        }
        public Material GetRandomMaterial()
        {
            return materials[UnityEngine.Random.Range(0, materials.Length)];
        }
    }

    [SerializeField] private ClothingTypeProperties[] clothingTypeProperties;

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
        Debug.Log("InteractablesManager Initialise");
        InitialisePools();
        InitialiseSpawnSpots();
        InitialiseInteractablesToBeSpawned();
        nextClothingItemSpawn = 0;
        nextExtraLifeItemSpawn = 0;
    }

    private void InitialiseInteractablesToBeSpawned()
    {
        if (interactablesToBeSpawned == null)
        {
            interactablesToBeSpawned = new List<Interactable>();
        }
        interactablesToBeSpawned.Clear();
    }

    private void InitialiseSpawnSpots()
    {
        if(spawnSpots == null)
        {
            spawnSpots = new SpawnSpot[World.NUMBER_OF_LANES, World.NUMBER_OF_VERTICAL_DIVISIONS_PER_LANE];
        }
        ClearSpawnSpots();
    }

    private void ClearSpawnSpots()
    {
        for (int x = 0; x < spawnSpots.GetLength(0); x++)
        {
            for (int y = 0; y < spawnSpots.GetLength(1); y++)
            {
                spawnSpots[x, y] = SpawnSpot.FREE;
            }
        }
    }

    private void InitialisePools()
    {
        if(clothingItemsPool == null)
        {
            clothingItemsPool = new List<ClothingItem>();
            //for (int i = 0; i < clothingItemsPreFabs.Length; i++)
            {
                for (int j = 0; j < 56; j++)
                {
                    ClothingItem newClothingItem = Instantiate(clothingItemsPreFab);
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

        if (lentInteractables == null)
        {
            lentInteractables = new List<Interactable>();
        }

        RecycleAllInteractables();
    }

    private void Update()
    {
        if (!GameManager.GameIsOver && !GameManager.GameIsPaused)
        {
            ManageSpawning();
        }
    }

    private void ManageSpawning()
    {
        UInt32 playerMileage = Player.Instance.MileageInUnits;
        if (playerMileage >= nextClothingItemSpawn)
        {
            int maxClothingItems = 0;
            for (int i = 0; i < clothingItemsGenerationPoints.Length; i++)
            {
                if(playerMileage < clothingItemsGenerationPoints[i].mileage)
                {
                    break;
                }
                else
                {
                    maxClothingItems = clothingItemsGenerationPoints[i].maxClothingItemsGenerated;
                }
            }
            int numberOfClothingItemsToSpawn = 
                UnityEngine.Random.Range(1, maxClothingItems + 1/*Added 1 cause Random.Range isn't inclusive*/);
            interactablesToBeSpawned.AddRange
                 (LendRandomClothingItems(numberOfClothingItemsToSpawn));
            nextClothingItemSpawn = (UInt32)(playerMileage +
                   UnityEngine.Random.Range((int)minimumUnitsBetweenClothingItemSpawns, (int)maximumUnitsBetweenClothingItemSpawns));
        }
        if (playerMileage >= nextExtraLifeItemSpawn)
        {
            interactablesToBeSpawned.Add(LendExtraLifeItem());
            nextExtraLifeItemSpawn = (UInt32)(playerMileage +
                  UnityEngine.Random.Range((int)minimumUnitsBetweenExtraLifeItemSpawns, (int)maximumUnitsBetweenExtraLifeItemSpawns));
        }

        if(interactablesToBeSpawned.Count > 0)
        {
            for (int i = 0; i < interactablesToBeSpawned.Count; i++)
            {
                bool freeSpotFound = false;
                int x = 0;
                int y = 0;
                while (!freeSpotFound)
                {
                    x = UnityEngine.Random.Range(0, spawnSpots.GetLength(0));
                    y = UnityEngine.Random.Range(0, spawnSpots.GetLength(1));
                    if (spawnSpots[x, y] == SpawnSpot.FREE)
                    {
                        freeSpotFound = true;
                    }
                }
                spawnSpots[x, y] = SpawnSpot.OCCUPIED;
                interactablesToBeSpawned[i].gameObject.SetActive(true);
                Vector2 laneXY = World.LanesXYs[x, y];
                float positionZ = player.transform.position.z + spawnDistanceFromPlayer;
                interactablesToBeSpawned[i].transform.position = new Vector3(laneXY.x, laneXY.y, positionZ);
            }

            ClearSpawnSpots();
            interactablesToBeSpawned.Clear();
        }      
    }

    #region Lending and Recycling:

    private Interactable[] LendRandomClothingItems(int amount)
    {
        if (clothingItemsPool == null || clothingItemsPool.Count < amount)
        {
            Debug.LogError("Pool's closed!");
            return null;
        }
        Interactable[] lentItems = new Interactable[amount];
        int clothingItemsPoolCount = clothingItemsPool.Count;
        for (int i = 0; i < lentItems.Length; i++)
        {
            int index = clothingItemsPoolCount -1 -i; //UnityEngine.Random.Range(0, clothingItemsPool.Count);
            Interactable interactable = clothingItemsPool[index];

            //TODO: change this:
            ClothingItem clothingItem = interactable as ClothingItem;
            ClothingType type = (ClothingType)UnityEngine.Random.Range(0, (int)ClothingType.LENGTH);
            Mesh mesh =null;
            Material material = null;

            for (int j = 0; j < clothingTypeProperties.Length; j++)
            {
                if(clothingTypeProperties[j].GetClothingType() == type)
                {
                    mesh = clothingTypeProperties[j].GetRandomMesh();
                    material = clothingTypeProperties[j].GetRandomMaterial();
                }
            }
            clothingItem.ChangeType(type, material, mesh);
            lentItems[i] = interactable;
        }

        clothingItemsPool.RemoveRange(clothingItemsPoolCount - amount, amount);

        lentInteractables.AddRange(lentItems);
        

        return lentItems;
    }

    private Interactable LendExtraLifeItem()
    {
        if (extraLifeItemsPool == null || extraLifeItemsPool.Count <= 0)
        {
            Debug.LogError("Pool's closed!");
            return null;
        }
        int index = 0;
        Interactable lentItem = extraLifeItemsPool[index];
        extraLifeItemsPool.RemoveAt(index);

        lentInteractables.Add(lentItem);
        return lentItem;
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

        lentInteractables.Remove(interactable);
    }

    public static void RecycleAllInteractables()
    {
        foreach (Interactable interactable in lentInteractables)
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

        lentInteractables.Clear();

    }
    #endregion
}
