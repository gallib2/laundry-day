using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private static ClothingType clothingTypeRequired;
    public static ClothingType ClothingTypeRequired
    {
        get { return clothingTypeRequired; }
    }
    [SerializeField] private float minimumSecondsBetweenClothingTypeChanges;
    [SerializeField] private float maximumSecondsBetweenClothingTypeChanges;

    private void Start()
    {
        ChangeClothingTypeRequired();
    }

    private void ChangeClothingTypeRequired()
    {
        bool newColthingTypeIsDifferentToOldOne = false;
        while (!newColthingTypeIsDifferentToOldOne)
        {
            ClothingType newClothingType = (ClothingType)
            Random.Range(0, (int)ClothingType.LENGTH);
            if(newClothingType != clothingTypeRequired)
            {
                clothingTypeRequired = newClothingType;
                InformationText.Instance.UpdateText(newClothingType.ToString());
                newColthingTypeIsDifferentToOldOne = true;
            }
        }

        Invoke("ChangeClothingTypeRequired",
            Random.Range(minimumSecondsBetweenClothingTypeChanges, maximumSecondsBetweenClothingTypeChanges));

        Debug.Log("ClothingTypeRequired:" + ClothingTypeRequired.ToString()) ;
    }
}
