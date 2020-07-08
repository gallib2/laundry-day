using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private static ClothingType currentClothingTypeRequired;
    public static ClothingType CurrentClothingTypeRequired
    {
        get { return currentClothingTypeRequired; }
    }
    private static ClothingType nextClothingTypeRequired;
    public static event Action<ClothingType, ClothingType> OnClothingTypeRequiredChanged;

    [SerializeField] private float minimumSecondsBetweenClothingTypeChanges;
    [SerializeField] private float maximumSecondsBetweenClothingTypeChanges;
    private float nextClothingTypeChangeScheduled;
    public static event Action<float> OnClothingTypeChangeWarning;

    [SerializeField] private int warningOfClothingTypeChangeDuration;


    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        InitialiseClothingTypeRequired();
    }

    private void DetermineNextClothingTypeChangeSchedule()
    {
        nextClothingTypeChangeScheduled = Time.time + 
            (UnityEngine.Random.Range(minimumSecondsBetweenClothingTypeChanges, maximumSecondsBetweenClothingTypeChanges));
    }

    private void InitialiseClothingTypeRequired()
    {
        currentClothingTypeRequired = (ClothingType)
            UnityEngine.Random.Range(0, (int)ClothingType.LENGTH);

        bool newColthingTypeIsDifferentToOldOne = false;
        while (!newColthingTypeIsDifferentToOldOne)
        {
            ClothingType newClothingType = (ClothingType)
               UnityEngine.Random.Range(0, (int)ClothingType.LENGTH);
            if (newClothingType != currentClothingTypeRequired)
            {
                nextClothingTypeRequired = newClothingType;
                newColthingTypeIsDifferentToOldOne = true;
            }
        }

        DetermineNextClothingTypeChangeSchedule();

        OnClothingTypeRequiredChanged(currentClothingTypeRequired, nextClothingTypeRequired);
    }

    private void ChangeClothingTypeRequired()
    {
        bool newColthingTypeIsDifferentToOldOne = false;
        while (!newColthingTypeIsDifferentToOldOne)
        {
            ClothingType newClothingType = (ClothingType)
               UnityEngine.Random.Range(0, (int)ClothingType.LENGTH);
            if(newClothingType != nextClothingTypeRequired)
            {
                currentClothingTypeRequired = nextClothingTypeRequired;
                nextClothingTypeRequired = newClothingType;
                newColthingTypeIsDifferentToOldOne = true;
            }
        }
        
       /* Invoke("ChangeClothingTypeRequired",
            Random.Range(minimumSecondsBetweenClothingTypeChanges, maximumSecondsBetweenClothingTypeChanges));*/
        OnClothingTypeRequiredChanged(currentClothingTypeRequired, nextClothingTypeRequired);
    }

    private void Update()
    {
        float time = Time.time;
        float timeDifference = nextClothingTypeChangeScheduled - time;
        if (timeDifference < warningOfClothingTypeChangeDuration)
        {
            OnClothingTypeChangeWarning(timeDifference);
        }
        if (time >= nextClothingTypeChangeScheduled)
        {
            ChangeClothingTypeRequired();
            DetermineNextClothingTypeChangeSchedule();
        }
    }
}
