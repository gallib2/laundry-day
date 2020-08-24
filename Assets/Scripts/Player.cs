using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Singleton<Player>
{

    [SerializeField] private Vector3 startingPosition;
    #region Movement:
    [Header("Movement:")]
    private CharacterController controller;
    private float currentSpeedOnZ;
    private float speedOnX;

    [SerializeField] private float minimumSpeedOnZ;
    [SerializeField] private float maximumSpeedOnZ;
    private float timeToReachMaximumZSpeed;
    private bool maximumSpeedReached;
    private float timePassedSinceStart;

    [SerializeField] private float jumpForce = 10.0f;
    private bool forbidSwitchingLanesWhileAirborne = true;
    [SerializeField] private float gravity = 1.0f;
    private float yVelocity = 0.0f;
    private float xVelocity = 0.0f;
    private int desiredLane = 1;
    private float desiredLaneX;

    private UInt32 _mileageInUnits;
    public UInt32 MileageInUnits
    {
        get { return _mileageInUnits; }
        private set
        {
            _mileageInUnits = value;
            OnMileageChanged(_mileageInUnits);
        }
    }

    public static event Action<UInt32> OnMileageChanged;

    private InputType inputType;

    #endregion

    #region Washing Related:
   

    public static event Action<int> OnLivesChanged;   
    private UInt32 _washedItems;
    public UInt32 WashedItems
    {
        get { return _washedItems; }
        private set
        {
            _washedItems = value;
            OnWashedItemsChanged(_washedItems, washedItemsCombo);
        }
    }
    private UInt32 washedItemsCombo;

    public static event Action<UInt32, UInt32> OnWashedItemsChanged;

    private Stack<ClothingItemPhysicalProperties> collectedClothingItemsPhysicalProperties;
    #endregion

    #region Graphics:

    [SerializeField] private Animator modelAnimator;
    [SerializeField] private ParticleSystem bubbleBurstParticleSystem;

    #endregion

    private void OnEnable()
    {
        GameManager.OnRestart += Initialise;
        GameManager.OnGameStateChanged += ConformToNewGameState;

    }

    private void OnDisable()
    {
        GameManager.OnRestart -= Initialise;
        GameManager.OnGameStateChanged -= ConformToNewGameState;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Initialise();
    }

    private void Initialise()
    {
        TeleportTo(startingPosition);

        Settings.SettingsBlock settingsBlock =  Settings.Instance.CurrentBlock;
        minimumSpeedOnZ = settingsBlock.playerMinimumZSpeed;
        maximumSpeedOnZ = settingsBlock.playerMaximumZSpeed;
        speedOnX = settingsBlock.playerXSpeed;
        jumpForce = settingsBlock.playerJumpForce;
        forbidSwitchingLanesWhileAirborne = settingsBlock.forbidSwitchingLanesWhileAirborne;
        timePassedSinceStart = 0;

        timeToReachMaximumZSpeed = settingsBlock.timeToReachMaximumZSpeed;
        maximumSpeedReached = false;


        WashedItems = 0;
        washedItemsCombo = 0;

        MileageInUnits = 0;

        desiredLane = 1;
        desiredLaneX = World.LanesXYs[desiredLane, 0].x;

        modelAnimator.SetBool("GameIsOver", false);

        collectedClothingItemsPhysicalProperties = new Stack<ClothingItemPhysicalProperties>();
    }

    void Update()
    {

        if (!GameManager.GameIsPaused)
        {
            if (GameManager.CurrentGameState == GameManager.GameState.InGame)
            {
                inputType = InputManager.GetInput();
                timePassedSinceStart += Time.deltaTime;
                if (!maximumSpeedReached)
                {
                    Accelerate();
                }
            }

            Move();
            CalculateMileage();
        }
    }

    private void TeleportTo(Vector3 location)
    {
        controller.enabled = false;
        transform.position = location;
        controller.enabled = true;
    }

    private void Accelerate()
    {
        float normaliser = maximumSpeedOnZ - minimumSpeedOnZ;
        float normalisedSpeed = (timePassedSinceStart / (timeToReachMaximumZSpeed / normaliser)) + minimumSpeedOnZ;
        
        if(normalisedSpeed < maximumSpeedOnZ)
        {
            currentSpeedOnZ = normalisedSpeed;
        }
        else
        {
            maximumSpeedReached = true;
            currentSpeedOnZ = maximumSpeedOnZ;
        }
    }

    private void CalculateMileage()
    {
        UInt32 newMileage = (UInt32)(transform.position.z - startingPosition.z);
        if(newMileage != MileageInUnits)
        {
            MileageInUnits = newMileage;
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        Vector3 velocity = 
           ((GameManager.CurrentGameState == GameManager.GameState.InGame) ? direction * currentSpeedOnZ : Vector3.zero);
        bool isGrounded = controller.isGrounded;
        if (!forbidSwitchingLanesWhileAirborne || isGrounded)
        {
            if (inputType == InputType.LEFT)
            {
                MoveLane(false);
            }
            if (inputType == InputType.RIGHT)
            {
                MoveLane(true);
            }
        }

        Vector3 targetPosition = transform.position.z * Vector3.forward;
        targetPosition.x = desiredLaneX;
        xVelocity = (targetPosition - transform.position).normalized.x * speedOnX;
        
        xVelocity = (targetPosition - transform.position).normalized.x * speedOnX;

        CheckJump(isGrounded);

        if (!isGrounded)
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        velocity.x = xVelocity;
        velocity.y = yVelocity;
        controller.Move(velocity * Time.deltaTime);

    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += goingRight ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, World.NUMBER_OF_LANES - 1);
        desiredLaneX = World.LanesXYs[desiredLane, 0].x;
    }

    private void CheckJump(bool isGrounded)
    {
        if (isGrounded)
        {
            bool toJump = (inputType == InputType.UP);
            if (toJump)
            {
                SoundSettings.Instance.PlaySound(SoundNames.Jump);
                modelAnimator.SetTrigger("Jump");
                yVelocity = jumpForce;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            if(interactable is ClothingItem)
            {
                ClothingItem clothingItem = interactable as ClothingItem;
                ClothingType clothingType = clothingItem.ClothingType;
                if(clothingType == GameManager.CurrentClothingTypeRequired)
                {
                    WashItem(clothingItem);
                }
                else
                {
                    CollectWrongItem();
                }

            }
            else if (interactable is ExtraLifeItem)
            {
                CollectCoin();
            }

            interactable.Interact();
        }
    }

    [SerializeField] private float ClothingItemsInFrontCheckDistance = 5f;
    [SerializeField] private Transform ClothingItemsInFrontCheckOrigin;
    private ClothingItem lastClothingItemAccommodatedFor;

    private void FixedUpdate()
    {
        CheckForClothingItemsInFront();
    }

    private void CheckForClothingItemsInFront()
    {
        RaycastHit raycastHit;
        Physics.Raycast
            (ClothingItemsInFrontCheckOrigin.position, 
            Vector3.forward, out raycastHit, ClothingItemsInFrontCheckDistance);
        if (raycastHit.collider != null)
        {
            ClothingItem clothingItem = raycastHit.collider.gameObject.GetComponent<ClothingItem>();

            if (clothingItem != null && clothingItem != lastClothingItemAccommodatedFor)
            {
                lastClothingItemAccommodatedFor = clothingItem;
                modelAnimator.SetTrigger("OpenDoor");
            }
        }
    }

    private void WashItem(ClothingItem clothingItem)
    {
        bubbleBurstParticleSystem.Play();

        WashedItems += 1;
        SoundSettings.Instance.PlaySound(SoundNames.CollectCorrect);
        washedItemsCombo += 1;

        collectedClothingItemsPhysicalProperties.Push(new ClothingItemPhysicalProperties(clothingItem.Mesh, clothingItem.Material));
    }

    private void CollectCoin()
    {
        bubbleBurstParticleSystem.Play();
        modelAnimator.SetTrigger("ExtraLife");
        SoundSettings.Instance.PlaySound(SoundNames.CollectLife);

    }

    [SerializeField] private LostClothingItem lostClothingItemPreFab;
    [SerializeField] private Transform lostClothesSpawnPoint;

    private void CollectWrongItem()
    {
        modelAnimator.SetTrigger("LoseALife");
        SoundSettings.Instance.PlaySound(SoundNames.CollectWrong);

        washedItemsCombo = 0;
        if (collectedClothingItemsPhysicalProperties.Count > 0)
        {
            LostClothingItem lostClothingItem = Instantiate(lostClothingItemPreFab, lostClothesSpawnPoint.position, Quaternion.identity);
            lostClothingItem.Spawn(collectedClothingItemsPhysicalProperties.Pop());
        }

    }

    private void ConformToNewGameState(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.GameOver)
        {
            modelAnimator.SetBool("GameIsOver", true);
        }
    }

}

