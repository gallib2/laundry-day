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
    [Header("Washing Related:")]
    private int _lives;
    private int Lives
    {
        get { return _lives; }
        set
        {
            _lives = value;
            OnLivesChanged(_lives);
        }
    }

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
    #endregion

    #region Graphics:

    [SerializeField] private Animator modelAnimator;
    [SerializeField] private ParticleSystem bubbleBurstParticleSystem;

    #endregion

    private void OnEnable()
    {
        GameManager.OnRestart += Initialise;
        GameManager.OnGameOver += GameOver;

    }

    private void OnDisable()
    {
        GameManager.OnRestart -= Initialise;
        GameManager.OnGameOver -= GameOver;

    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Initialise();
    }

    private void Initialise()
    {
        controller.enabled = false;
        transform.position = startingPosition;
        controller.enabled = true;

        Settings.SettingsBlock settingsBlock =  Settings.Instance.CurrentBlock;
        minimumSpeedOnZ = settingsBlock.playerMinimumZSpeed;
        maximumSpeedOnZ = settingsBlock.playerMaximumZSpeed;
        speedOnX = settingsBlock.playerXSpeed;
        jumpForce = settingsBlock.playerJumpForce;
        forbidSwitchingLanesWhileAirborne = settingsBlock.forbidSwitchingLanesWhileAirborne;
        timePassedSinceStart = 0;

        timeToReachMaximumZSpeed = settingsBlock.timeToReachMaximumZSpeed;
        maximumSpeedReached = false;


        int livesAtStart = settingsBlock.livesAtStart;
        if (livesAtStart <= 0)
        {
            Debug.LogWarning("Illegal lives At Start Value.");
            livesAtStart = 666;
        }
        Lives = livesAtStart;

        WashedItems = 0;
        washedItemsCombo = 0;

        MileageInUnits = 0;

        desiredLane = 1;

        modelAnimator.SetBool("GameIsOver", false);
    }

    void Update()
    {

        if (!GameManager.GameIsPaused)
        {
            if (!GameManager.GameIsOver && !GameManager.IntroIsPlaying)
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
            ((!GameManager.GameIsOver && !GameManager.IntroIsPlaying) ? direction * currentSpeedOnZ : Vector3.zero);
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
        if (desiredLane == (int)Lane.Left)
        {
            targetPosition += Vector3.left * World.HorizontalLaneSpacing;
        }
        else if (desiredLane == (int)Lane.Right)
        {
            targetPosition += Vector3.right * World.HorizontalLaneSpacing;
        }

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
        desiredLane = Mathf.Clamp(desiredLane, (int)Lane.Left, (int)Lane.Right);
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
                    WashItem();
                }
                else
                {
                    LoseALife();
                }

            }
            else if (interactable is ExtraLifeItem)
            {
                GainALife();
            }

            interactable.Interact();
        }
    }

    [SerializeField] private LayerMask interactablesLayerMask;
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

    private void WashItem()
    {
        bubbleBurstParticleSystem.Play();

        WashedItems += 1;
        SoundSettings.Instance.PlaySound(SoundNames.CollectCorrect);
        washedItemsCombo += 1;
    }

    private void LoseALife()
    {
        modelAnimator.SetTrigger("LoseALife");
        SoundSettings.Instance.PlaySound(SoundNames.CollectWrong);

        Lives -= 1;
        washedItemsCombo = 0;
    }

    private void GameOver()
    {
        modelAnimator.SetBool("GameIsOver",true);
    }

    private void GainALife()
    {
        bubbleBurstParticleSystem.Play();
        modelAnimator.SetTrigger("ExtraLife");
        SoundSettings.Instance.PlaySound(SoundNames.CollectLife);

        Lives += 1;
    }
}

public enum Lane
{
    Left = 0,
    Mid = 1,
    Right = 2
}
