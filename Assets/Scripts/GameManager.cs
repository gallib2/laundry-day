using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnRestart;
    public static event Action OnPause;
    public static event Action OnUnPause;

    public enum GameState
    {
        BeginingScreen, Intro, InGame, GameOver
    }

    private static GameState currentGameState;
    public static GameState CurrentGameState
    {
        get
        {
            return currentGameState;
        }
        private set
        {
            currentGameState =value;
            OnGameStateChanged(currentGameState);
        }
    }
    public static bool GameIsPaused
    {
        get; private set;
    }

    [SerializeField] private float introDuration = 3f;
    private float introTimeLeft;

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
        CurrentGameState = GameState.BeginingScreen;
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
        CurrentGameState = GameState.Intro;
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
        CurrentGameState = GameState.GameOver;
        SoundSettings.Instance.StopSound(SoundNames.TickingClock);
        SoundSettings.Instance.StopSound(SoundNames.Background);
        SoundSettings.Instance.PlaySound(SoundNames.Lose);
    }

    private void Update()
    {

        if (!GameIsPaused)
        {
            if(CurrentGameState == GameState.Intro)
            {
                 introTimeLeft -= Time.deltaTime;
                 if (introTimeLeft <= 0)
                 {
                    CurrentGameState = GameState.InGame;
                 }
                
            }
            else if(CurrentGameState == GameState.InGame)
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