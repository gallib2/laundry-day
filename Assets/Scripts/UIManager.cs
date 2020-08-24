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
    [SerializeField] private TMPro.TextMeshProUGUI currentClothingTypeRequiredText;
    [SerializeField] private Image currentClothingTypeRequiredImage;
    [SerializeField] private Image nextClothingTypeRequiredImage;

    [SerializeField] private Sprite[] clothingTypeRequiredSprites;
    [SerializeField] private  GameObject gameOverPopUp;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject[] objectsToDisappearOnGameOver;

    [SerializeField] private GameObject[] objectsToHideDuringBeginingScreen;
    [SerializeField] private GameObject[] objectsToShowDuringBeginingScreen;


    private void OnEnable()
    {
        Player.OnMileageChanged += UpdateMileageText;
        Player.OnLivesChanged += UpdateLivesText;
        Player.OnWashedItemsChanged += UpdateWashedItemsText;
        GameManager.OnClothingTypeRequiredChanged += UpdateClothingTypeRequired;
        GameManager.OnClothingTypeChangeWarning += UpdateClothingTypeChangeWarningText;
        GameManager.OnGameStateChanged += ConformToNewGameState;
        GameManager.OnPause += ShowPauseMenu;
        GameManager.OnUnPause += HidePauseMenu;

    }

    private void OnDisable()
    {
        Player.OnMileageChanged -= UpdateMileageText;
        Player.OnLivesChanged -= UpdateLivesText;
        Player.OnWashedItemsChanged -= UpdateWashedItemsText;
        GameManager.OnClothingTypeRequiredChanged -= UpdateClothingTypeRequired;
        GameManager.OnClothingTypeChangeWarning -= UpdateClothingTypeChangeWarningText;
        GameManager.OnGameStateChanged -= ConformToNewGameState;
        GameManager.OnPause -= ShowPauseMenu;
        GameManager.OnUnPause -= HidePauseMenu;
    }

    private void ConformToNewGameState(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.GameOver:
                ShowGameOverPopUp();
                break;
            case GameManager.GameState.BeginingScreen:
                ShowBeginingUI();
                break;
            case GameManager.GameState.Intro:
                ShowInGameUI();
                break;
        }
    }

    private void UpdateMileageText(System.UInt32 mileage)
    {
        mileageText.text = mileage.ToString()+"M";
    }

    private void UpdateLivesText(int lives)
    {
        livesText.text = lives.ToString();
    }

    private void UpdateWashedItemsText(System.UInt32 washedItems, System.UInt32 washedItemsCombo )
    {
        washedItemsText.text = washedItems.ToString();
    }

    private void UpdateClothingTypeRequired
        (ClothingType currentClothingTypeRequired, ClothingType nextClothingTypeRequired)
    {
        currentClothingTypeRequiredImage.sprite =
            clothingTypeRequiredSprites[(int)currentClothingTypeRequired];
        currentClothingTypeRequiredImage.SetNativeSize();
        nextClothingTypeRequiredImage.sprite =
            clothingTypeRequiredSprites[(int)nextClothingTypeRequired];
        nextClothingTypeRequiredImage.SetNativeSize();

        currentClothingTypeRequiredText.text = currentClothingTypeRequired.ToString();
        clothingTypeChangeWarningText.text = "";

    }

    private void UpdateClothingTypeChangeWarningText(float time)
    {
        clothingTypeChangeWarningText.text = time.ToString("f1");
    }

    private void ShowBeginingUI()
    {
        for (int i = 0; i < objectsToHideDuringBeginingScreen.Length; i++)
        {
            objectsToHideDuringBeginingScreen[i].SetActive(false);
        }
        for (int i = 0; i < objectsToShowDuringBeginingScreen.Length; i++)
        {
            objectsToShowDuringBeginingScreen[i].SetActive(true);
        }
    }

    private void ShowInGameUI()
    {
        for (int i = 0; i < objectsToHideDuringBeginingScreen.Length; i++)
        {
            objectsToHideDuringBeginingScreen[i].SetActive(true);
        }
        for (int i = 0; i < objectsToShowDuringBeginingScreen.Length; i++)
        {
            objectsToShowDuringBeginingScreen[i].SetActive(false);
        }
       
        for (int i = 0; i < objectsToDisappearOnGameOver.Length; i++)
        {
            if (objectsToDisappearOnGameOver[i] != null)
            {
                objectsToDisappearOnGameOver[i].SetActive(true);

            }
        }
        gameOverPopUp.SetActive(false);
    }

    private void ShowGameOverPopUp()
    {
        for (int i = 0; i < objectsToDisappearOnGameOver.Length; i++)
        {
            objectsToDisappearOnGameOver[i].SetActive(false);
        }
        gameOverPopUp.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    private void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
    }
}
