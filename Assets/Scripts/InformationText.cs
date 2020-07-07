using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InformationText : Singleton<InformationText>
{
    [SerializeField] private Text clothingTypeRequiredText;
    [SerializeField] private Text livesText;

    public void UpdateText(string clothingTypeRequired = null, string lives = null )
    {
        if (clothingTypeRequired != null)
        {
            clothingTypeRequiredText.text = "Clothing type required: " +clothingTypeRequired;
        }
        if (lives != null)
        {
            livesText.text = "Lives:" + lives;
        }

    }
}
