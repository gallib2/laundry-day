using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TMPro.TextMeshProUGUI mileageText;
    [SerializeField] private TMPro.TextMeshProUGUI livesText;
    [SerializeField] private TMPro.TextMeshProUGUI washedItemsText;
    [SerializeField] private TMPro.TextMeshProUGUI clothingTypeChangeWarningText;
    [SerializeField] private Image currentClothingTypeRequiredImage;
    [SerializeField] private Image nextClothingTypeRequiredImage;

    [SerializeField] private Sprite[] currentClothingTypeRequiredSprites;
    [SerializeField] private Sprite[] nextClothingTypeRequiredSprites;


    private void OnEnable()
    {
        Player.OnMileageChanged += UpdateMileageText;
        Player.OnLivesChanged += UpdateLivesText;
        Player.OnWashedItemsChanged += UpdateWashedItemsText;
        GameManager.OnClothingTypeRequiredChanged += UpdateClothingTypeRequired;
        GameManager.OnClothingTypeChangeWarning += UpdateClothingTypeChangeWarningText;

    }

    private void OnDisable()
    {
        Player.OnMileageChanged -= UpdateMileageText;
        Player.OnLivesChanged -= UpdateLivesText;
        Player.OnWashedItemsChanged -= UpdateWashedItemsText;
        GameManager.OnClothingTypeRequiredChanged -= UpdateClothingTypeRequired;
        GameManager.OnClothingTypeChangeWarning -= UpdateClothingTypeChangeWarningText;

    }

    private void UpdateMileageText(System.UInt32 mileage)
    {
        mileageText.text = mileage.ToString();
    }

    private void UpdateLivesText(int lives)
    {
        livesText.text = lives.ToString();
    }

    private void UpdateWashedItemsText(System.UInt32 washedItems)
    {
        washedItemsText.text = washedItems.ToString();
    }

    private void UpdateClothingTypeRequired
        (ClothingType currentClothingTypeRequired, ClothingType nextClothingTypeRequired)
    {
        currentClothingTypeRequiredImage.sprite = 
            currentClothingTypeRequiredSprites[(int)currentClothingTypeRequired];
        nextClothingTypeRequiredImage.sprite =
            nextClothingTypeRequiredSprites[(int)nextClothingTypeRequired];

        clothingTypeChangeWarningText.text = "";

    }

    private void UpdateClothingTypeChangeWarningText(float time)
    {
        clothingTypeChangeWarningText.text = time.ToString("f1");
    }
}
