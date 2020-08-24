using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : Singleton<InteractablesManager>
{
    private const int RANDOM_Z_ANGLE_MIN = -6;
    private const int RANDOM_Z_ANGLE_MAX = 6;

    [Serializable]
    private struct Chance
    {
        public int minimum;
        public int demand;
        public int maximum;

        public bool IsPositive()
        {
            return (UnityEngine.Random.Range(minimum, maximum + 1) >= demand);
        }
    }

    [SerializeField] private Chance lowestFloorEnforceChance;
    [SerializeField] private ClothingItem clothingItemsPreFab;
    [SerializeField] private Hanger hangerPreFab;
    [SerializeField] private ExtraLifeItem extraLifeItemPreFab;
    private static List<ClothingItem> clothingItemsPool;
    private static List<ExtraLifeItem> extraLifeItemsPool;
    private static List<Interactable> lentInteractables;

    private static List<Hanger> hangersPool;
    private static List<Hanger> lentHangers;

    private const int INITIAL_CLOTHING_ITEMS_POOL_SIZE = 32;
    private const int INITIAL_EXTRA_LIFE_ITEMS_POOL_SIZE = 12;
    private const int CLOTHING_ITEMS_POOL_EMERGENCY_BOOST = 16;
    private const int EXTRA_LIFE_ITEMS_POOL_EMERGENCY_BOOST = 2;
    private const int INITIAL_HANGERS_POOL_SIZE = 12;
    private const int HANGERS_POOL_EMERGENCY_BOOST = 6;

    List<Interactable> interactablesToBeSpawned;

    [SerializeField] Player player;
    [SerializeField] private float spawnDistanceFromPlayer;

    [SerializeField] private UInt32 minimumUnitsBetweenClothingItemSpawns;
    [SerializeField] private UInt32 maximumUnitsBetweenClothingItemSpawns;
    [SerializeField] private UInt32 minimumUnitsBetweenExtraLifeItemSpawns;
    [SerializeField] private UInt32 maximumUnitsBetweenExtraLifeItemSpawns;
    [SerializeField] int  firstClothingItemSpawnMileage;
    [SerializeField] int  firstExtraLifeItemSpawnMileage;
    private int nextClothingItemSpawn;
    private int nextExtraLifeItemSpawn;

    [SerializeField] private ClothingItemsGenerationPoint[] clothingItemsGenerationPoints;
    public static ClothingItemsGenerationPoint[] ClothingItemsGenerationPoints
    {
        get { return Instance.clothingItemsGenerationPoints; }
    }
    [Serializable]
    public struct ClothingItemsGenerationPoint
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
    private bool initialised = false;

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
        initialised = false;
        InitialisePools();
        InitialiseSpawnSpots();
        InitialiseInteractablesToBeSpawned();

        nextClothingItemSpawn = firstClothingItemSpawnMileage;
        nextExtraLifeItemSpawn = firstExtraLifeItemSpawnMileage;
        initialised = true;

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
            PopulateClothingItemsPool(INITIAL_CLOTHING_ITEMS_POOL_SIZE);
        }

        if (extraLifeItemsPool == null)
        {
            extraLifeItemsPool = new List<ExtraLifeItem>();
            PopulateExtraLifeItemsPool(INITIAL_EXTRA_LIFE_ITEMS_POOL_SIZE);
        }

        if (hangersPool == null)
        {
            hangersPool = new List<Hanger>();
            PopulateHangersPool(INITIAL_HANGERS_POOL_SIZE);
        }

        if (lentInteractables == null)
        {
            lentInteractables = new List<Interactable>();
        }

        if (lentHangers == null)
        {
            lentHangers = new List<Hanger>();
        }

        RecycleAllInteractables();
        RecycleAllHangers();
    }

    private void PopulateExtraLifeItemsPool(int ammout)
    {
        for (int i = 0; i < ammout; i++)
        {
            ExtraLifeItem newExtraLifeItem = Instantiate(extraLifeItemPreFab);
            newExtraLifeItem.gameObject.SetActive(false);
            extraLifeItemsPool.Add(newExtraLifeItem);
        }
    }

    private void PopulateClothingItemsPool(int ammout)
    {
        for (int i = 0; i < ammout; i++)
        {
            ClothingItem newClothingItem = Instantiate(clothingItemsPreFab);
            newClothingItem.gameObject.SetActive(false);
            clothingItemsPool.Add(newClothingItem);
        }
    }

    private void PopulateHangersPool(int ammout)
    {
        for (int i = 0; i < ammout; i++)
        {
            Hanger newHanger = Instantiate(hangerPreFab);
            newHanger.gameObject.SetActive(false);
            hangersPool.Add(newHanger);
        }
    }

    private void Update()
    {
        if ( !GameManager.GameIsPaused && initialised)
        {
            ManageSpawning();
        }
    }

    private void ManageSpawning()
    {
        UInt32 mileagePlusSpawnDistance = (Player.Instance.MileageInUnits + (UInt32)spawnDistanceFromPlayer );
        int nextClothingItemSpawnCopy = nextClothingItemSpawn;
        if (mileagePlusSpawnDistance >= nextClothingItemSpawn)
        {
            int maxClothingItems = 0;
            for (int i = 0; i < clothingItemsGenerationPoints.Length; i++)
            {
                if(mileagePlusSpawnDistance < clothingItemsGenerationPoints[i].mileage)
                {
                    break;
                }
                else
                {
                    maxClothingItems = clothingItemsGenerationPoints[i].maxClothingItemsGenerated;
                }
            }
            int numberOfClothingItemsToSpawn = 
                UnityEngine.Random.Range(1, maxClothingItems + 1);
            interactablesToBeSpawned.AddRange
                 (LendRandomClothingItems(numberOfClothingItemsToSpawn));
            nextClothingItemSpawn = (int)(nextClothingItemSpawn +
                   UnityEngine.Random.Range((int)minimumUnitsBetweenClothingItemSpawns, (int)maximumUnitsBetweenClothingItemSpawns));

            if (mileagePlusSpawnDistance >= nextExtraLifeItemSpawn)
            {
                interactablesToBeSpawned.Add(LendExtraLifeItem());
                nextExtraLifeItemSpawn = (int)(nextExtraLifeItemSpawn +
                      UnityEngine.Random.Range((int)minimumUnitsBetweenExtraLifeItemSpawns, (int)maximumUnitsBetweenExtraLifeItemSpawns));
            }
        }

        if(interactablesToBeSpawned.Count > 0)
        {

            float positionZ = nextClothingItemSpawnCopy;
            bool hangerNeeded = false;

            for (int i = 0; i < interactablesToBeSpawned.Count; i++)
            {
                bool freeSpotFound = false;
                int x = 0;
                int y = 0;
                while (!freeSpotFound)
                {
                    x = UnityEngine.Random.Range(0, spawnSpots.GetLength(0));
                    y = UnityEngine.Random.Range(0, spawnSpots.GetLength(1));
                    if(y != 0)
                    {
                        if (lowestFloorEnforceChance.IsPositive() && spawnSpots[x, 0] == SpawnSpot.FREE)
                        {
                            y = 0;
                        }
                        else
                        {
                            hangerNeeded = true;
                        }
                    }
                    if (spawnSpots[x, y] == SpawnSpot.FREE)
                    {
                        if(!(y>0&& interactablesToBeSpawned[i] is ExtraLifeItem))
                        {
                            freeSpotFound = true;

                        }
                    }
                }
                spawnSpots[x, y] = SpawnSpot.OCCUPIED;
                interactablesToBeSpawned[i].gameObject.SetActive(true);
                Vector2 laneXY = World.LanesXYs[x, y];
                interactablesToBeSpawned[i].transform.position = new Vector3(laneXY.x, laneXY.y, positionZ);
            }

            if (hangerNeeded)
            {
                Hanger hanger = LendAHanger();

                hanger.gameObject.SetActive(true);
                int midLane = World.NUMBER_OF_LANES/2;
                Vector2 laneXY = World.LanesXYs[midLane, midLane];
                hanger.transform.position = new Vector3(laneXY.x, laneXY.y, positionZ);
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
            if(clothingItemsPool != null)
            {
                PopulateClothingItemsPool(CLOTHING_ITEMS_POOL_EMERGENCY_BOOST);
            }
            else
            {
                return null;
            }
        }
        Interactable[] lentItems = new Interactable[amount];
        int clothingItemsPoolCount = clothingItemsPool.Count;
        for (int i = 0; i < lentItems.Length; i++)
        {
            int index = clothingItemsPoolCount -1 -i;
            Interactable interactable = clothingItemsPool[index];

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

            Quaternion angle = Quaternion.Euler(0, 0, UnityEngine.Random.Range(RANDOM_Z_ANGLE_MIN, RANDOM_Z_ANGLE_MAX));
            clothingItem.ChangeType(type, material, mesh, angle);
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
            if (extraLifeItemsPool != null)
            {
                PopulateExtraLifeItemsPool(EXTRA_LIFE_ITEMS_POOL_EMERGENCY_BOOST);
            }
            else
            {
                return null;
            }
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

    private Hanger LendAHanger()
    {
        if (hangersPool == null || hangersPool.Count <= 0)
        {
            if (hangersPool != null)
            {
                PopulateExtraLifeItemsPool(HANGERS_POOL_EMERGENCY_BOOST);
            }
            else
            {
                return null;
            }
        }
        int index = 0;
        Hanger lentItem = hangersPool[index];
        hangersPool.RemoveAt(index);

        lentHangers.Add(lentItem);
        return lentItem;
    }

    public static void RecycleHanger(Hanger hanger)
    {
        hanger.gameObject.SetActive(false);
        hangersPool.Add(hanger);

        lentHangers.Remove(hanger);
    }

    public static void RecycleAllHangers()
    {
        foreach (Hanger hanger in lentHangers)
        {
            hanger.gameObject.SetActive(false);
            hangersPool.Add(hanger);
        }
        lentHangers.Clear();
    }
    #endregion
}
