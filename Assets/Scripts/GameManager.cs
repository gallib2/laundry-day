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
    public static bool InBeginingScreen
    {
        get; private set;
    }

    [SerializeField] private float introDuration = 3f;
    private float introTimeLeft;
    public static bool IntroIsPlaying
    {
        get; private set;
    }
    public static ClothingType CurrentClothingTypeRequired { get; private set; }
    private static ClothingType nextClothingTypeRequired;
    public static event Action<ClothingType, ClothingType> OnClothingTypeRequiredChanged;

    [SerializeField] private float minimumSecondsBetweenClothingTypeChanges;
    [SerializeField] private float maximumSecondsBetweenClothingTypeChanges;
    private float nextClothingTypeChangeScheduled;
    public static event Action<float> OnClothingTypeChangeWarning;

    [SerializeField] private int warningOfClothingTypeChangeDuration;
    private bool warningOfClothingTypeChangeInProgress;
    [SerializeField] private GameObject menu;

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
        InBeginingScreen = true;
    }

    public void BackToMenu()
    {
        GameIsPaused = true;
        menu.SetActive(true);
    }

    public void Restart()
    {
        OnRestart();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        GameIsPaused = true;
        SoundSettings.PauseAllPausableSounds();

        OnPause();
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        GameIsPaused = false;
        SoundSettings.UnPauseAllPausableSounds();

        OnUnPause();
    }

    private void Initialise()
    {
        InBeginingScreen = false;
        GameIsOver = false;
        IntroIsPlaying = true;
        warningOfClothingTypeChangeInProgress = false;
        introTimeLeft = introDuration;
        InitialiseClothingTypeRequired();
        UnPauseGame();
        SoundSettings.StopAllSounds();
        SoundSettings.Instance.PlaySound(SoundNames.Background);
    }

    private void DetermineNextClothingTypeChangeSchedule()
    {
        nextClothingTypeChangeScheduled = Time.time + 
            (UnityEngine.Random.Range(minimumSecondsBetweenClothingTypeChanges, maximumSecondsBetweenClothingTypeChanges));
    }

    private void InitialiseClothingTypeRequired()
    {
        CurrentClothingTypeRequired = (ClothingType)
            UnityEngine.Random.Range(0, (int)ClothingType.LENGTH);

        bool newColthingTypeIsDifferentToOldOne = false;
        while (!newColthingTypeIsDifferentToOldOne)
        {
            ClothingType newClothingType = (ClothingType)
               UnityEngine.Random.Range(0, (int)ClothingType.LENGTH);
            if (newClothingType != CurrentClothingTypeRequired)
            {
                nextClothingTypeRequired = newClothingType;
                newColthingTypeIsDifferentToOldOne = true;
            }
        }

        DetermineNextClothingTypeChangeSchedule();

        OnClothingTypeRequiredChanged(CurrentClothingTypeRequired, nextClothingTypeRequired);
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
                CurrentClothingTypeRequired = nextClothingTypeRequired;
                nextClothingTypeRequired = newClothingType;
                newColthingTypeIsDifferentToOldOne = true;
            }
        }
        
        OnClothingTypeRequiredChanged(CurrentClothingTypeRequired, nextClothingTypeRequired);
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
        SoundSettings.Instance.StopSound(SoundNames.TickingClock);
        SoundSettings.Instance.StopSound(SoundNames.Background);
        SoundSettings.Instance.PlaySound(SoundNames.Lose);
        OnGameOver();
    }

    private void Update()
    {

        if (!GameIsOver && !GameIsPaused &&!InBeginingScreen)
        {
            if (IntroIsPlaying)
            {
                introTimeLeft -= Time.deltaTime;
                if (introTimeLeft <= 0)
                {
                    IntroIsPlaying = false;
                }
            }
            else
            {
                float time = Time.time;
                float timeDifference = nextClothingTypeChangeScheduled - time;
                if (timeDifference < warningOfClothingTypeChangeDuration)
                {
                    if (!warningOfClothingTypeChangeInProgress)
                    {
                        warningOfClothingTypeChangeInProgress = true;
                        SoundSettings.Instance.PlaySound(SoundNames.TickingClock);
                    }
                    OnClothingTypeChangeWarning(timeDifference);

                }
                if (time >= nextClothingTypeChangeScheduled)
                {
                    ChangeClothingTypeRequired();
                    DetermineNextClothingTypeChangeSchedule();

                    SoundSettings.Instance.StopSound(SoundNames.TickingClock);
                    SoundSettings.Instance.PlaySound(SoundNames.ClothingTypeRequiredChange);
                    warningOfClothingTypeChangeInProgress = false;
                }
            }
        }
    }
}