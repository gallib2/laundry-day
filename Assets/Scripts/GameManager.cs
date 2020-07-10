using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnGameOver;
    public static event Action OnRestart;
    public static event Action OnPause;
    public static event Action OnUnPause;

    public static bool GameIsOver
    {
        get; private set;
    }
    public static bool GameIsPaused
    {
        get; private set;
    }

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

    private void OnEnable()
    {
        OnRestart += Initialise;
        Player.OnLivesChanged += CheckPlayerLives;
    }

    private void OnDisable()
    {
        OnRestart -= Initialise;
        Player.OnLivesChanged -= CheckPlayerLives;
    }

    private void Start()
    {
        Initialise();
    }

    public void BackToMenu()
    {
        ScenesManager.LoadScene(Scene.MAIN_MENU);
    }

    public void Restart()
    {
        OnRestart();
    }

    public void PauseGame()
    {
        GameIsPaused = true;
        OnPause();
    }

    public void UnPauseGame()
    {
        GameIsPaused = false;
        OnUnPause();
    }

    private void Initialise()
    {
        GameIsOver = false;
        InitialiseClothingTypeRequired();
        UnPauseGame();
        SoundSettings.Instance.PlaySound(SoundNames.Background);
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

    private void CheckPlayerLives(int lives)
    {
        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameIsOver = true;
        SoundSettings.Instance.StopSound(SoundNames.Background);
        SoundSettings.Instance.PlaySound(SoundNames.Lose);
        OnGameOver();
    }

    private void Update()
    {
        if (!GameIsOver && !GameIsPaused)
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
}