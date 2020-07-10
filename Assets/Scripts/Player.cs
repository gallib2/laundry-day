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
    private float currentSpeed;
    [SerializeField] private float minimumSpeed;
    [SerializeField] private float maximumSpeed;
    [SerializeField] private float timeToGetToMaximumSpeed;
    private bool maximumSpeedReached;
    private float timePassedSinceStart;

    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private bool forbidSwitchingLanesWhileAirborne = true;
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

    #endregion

    #region Washing Related::
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

    //[SerializeField] private int LIVES_AT_START = 3;
    public static event Action<int> OnLivesChanged;   
    private UInt32 _washedItems;
    public UInt32 WashedItems
    {
        get { return _washedItems; }
        private set
        {
            _washedItems = value;
            OnWashedItemsChanged(_washedItems);
        }
    }
    public static event Action<UInt32> OnWashedItemsChanged;
    #endregion

    #region Graphics:

    [SerializeField] private Animator animator;

    #endregion


    private void OnEnable()
    {
        GameManager.OnRestart += Initialise;
    }

    private void OnDisable()
    {
        GameManager.OnRestart -= Initialise;
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
        minimumSpeed = Settings.Instance.SetMinSpeed ? Settings.Instance.PlayerMinimumSpeed : minimumSpeed;
        maximumSpeed = Settings.Instance.SetMaxSpeed ? Settings.Instance.PlayerMaximumSpeed : maximumSpeed;
        jumpForce = Settings.Instance.IsSetJumpForce ? Settings.Instance.PlayeJumpForce : jumpForce;
        timePassedSinceStart = 0;
        maximumSpeedReached = false;

        int livesAtStart = Settings.Instance.LivesAtStart;
        if (livesAtStart <= 0)
        {
            Debug.LogWarning("Illegal lives At Start Value.");
            livesAtStart = 666;
        }
        Lives = livesAtStart;

        WashedItems = 0;

        MileageInUnits = 0;

        desiredLane = 1;
    }

    void Update()
    {
        if (!GameManager.GameIsOver && !GameManager.GameIsPaused)
        {
            timePassedSinceStart += Time.deltaTime;
            if (!maximumSpeedReached)
            {
                Accelerate();
            }
            Move();
            CalculateMileage();
        }

    }

    private void Accelerate()
    {
        float normaliser = maximumSpeed - minimumSpeed;
        float normalisedSpeed = (timePassedSinceStart / (timeToGetToMaximumSpeed / normaliser)) + minimumSpeed;
        if(normalisedSpeed < maximumSpeed)
        {
            currentSpeed = normalisedSpeed;
        }
        else
        {
            maximumSpeedReached = true;
            currentSpeed = maximumSpeed;
            Debug.Log("Maximum speed reached!");
        }

    }

    private void CalculateMileage()
    {
        UInt32 newMileage = (UInt32)(transform.position.z - startingPosition.z);
        if(newMileage != MileageInUnits)
        {
            MileageInUnits = newMileage;
           // InformationText.Instance.UpdateText(null, null, null,mileageInUnits.ToString());
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        Vector3 velocity = direction * currentSpeed;

        if(!forbidSwitchingLanesWhileAirborne || controller.isGrounded)
        {
            bool toMoveRight = MobileInput.Instance.SwipeRight ||
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
            bool toMoveLeft = MobileInput.Instance.SwipeLeft ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            if (toMoveLeft)
            {
                MoveLane(false);
            }
            if (toMoveRight)
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

        xVelocity = (targetPosition - transform.position).normalized.x * currentSpeed;

        CheckJump();

        velocity.x = xVelocity;
        velocity.y = yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += goingRight ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, (int)Lane.Left, (int)Lane.Right);
    }

    private void CheckJump()
    {
        if (controller.isGrounded)
        {
            bool toJump = Input.GetKeyDown(KeyCode.Space) || MobileInput.Instance.SwipeUp ;
            if (toJump)
            {
                animator.SetTrigger("Jump");
                yVelocity = jumpForce;
            }
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
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
                    SoundSettings.Instance.PlaySound(SoundNames.CollectGood);
                    WasheItem();
                }
                else
                {
                    SoundSettings.Instance.PlaySound(SoundNames.CollectBad);
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
    private ClothingItem lastClothingItemAccommodatedFor;//Hacky...

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
                animator.SetTrigger("OpenDoor");
            }
        }
    }

    private void WasheItem()
    {
        WashedItems += 1;
    }

    private void LoseALife()
    {
        Lives -= 1;
        /*if (Lives <= 0)
        {
            Lose();
            return;
        }*/
    }

    private void GainALife()
    {
        Lives += 1;
    }

}

public enum Lane
{
    Left = 0,
    Right = 2
}
