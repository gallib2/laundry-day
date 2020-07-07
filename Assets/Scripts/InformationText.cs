using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InformationText : Singleton<InformationText>
{
    [SerializeField] private Text clothingTypeRequiredText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text washedItemsText;
    [SerializeField] private Text mileageText;


    public void UpdateText
        (string clothingTypeRequired = null, string lives = null, string washedItems = null, string mileage = null)
    {
        if (clothingTypeRequired != null)
        {
            clothingTypeRequiredText.text = "Clothing type required: " +clothingTypeRequired;
        }
        if (lives != null)
        {
            livesText.text = "Lives: " + lives;
        }
        if (washedItems != null)
        {
            washedItemsText.text = "Washed Items: " + washedItems;
        }
        if (mileage != null)
        {
            mileageText.text = "Distance Traveled: " + mileage;
        }
    }
}
