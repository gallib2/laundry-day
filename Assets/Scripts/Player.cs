using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Singleton<Player>
{
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

    private UInt32 mileageInUnits;
    public UInt32 MileageInUnits
    {
        get { return mileageInUnits; }
    }
    private float zAtStart;

    #endregion

    #region Washing Related::
    [Header("Washing Related:")]
    private int lives;
    [SerializeField] private int LIVES_AT_START = 3;
    private UInt32 washedItems;
    #endregion

    private void Awake()
    {
        Debug.Log("minimumSpeed: awakew " + minimumSpeed);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Initialise();
    }

    private void Initialise()
    {
        Debug.Log("Settings.Instance.SetMinSpeed: " + Settings.Instance.SetMinSpeed);
        minimumSpeed = Settings.Instance.SetMinSpeed ? Settings.Instance.PlayerMinimumSpeed : minimumSpeed;
        maximumSpeed = Settings.Instance.SetMaxSpeed ? Settings.Instance.PlayerMaximumSpeed : maximumSpeed;
        timePassedSinceStart = 0;
        maximumSpeedReached = false;
        lives = LIVES_AT_START;
        washedItems = 0;
        mileageInUnits = 0;
        zAtStart = transform.position.z;
        InformationText.Instance.UpdateText
            (null, lives.ToString(), washedItems.ToString(), mileageInUnits.ToString());
    }

    void Update()
    {
        timePassedSinceStart += Time.deltaTime;
        if(!maximumSpeedReached)
        {
            Accelerate();
        }
        Move();
        CalculateMileage();
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
        UInt32 newMileage = (UInt32)(transform.position.z - zAtStart);
        if(newMileage != mileageInUnits)
        {
            mileageInUnits = newMileage;
            InformationText.Instance.UpdateText(null, null, null,mileageInUnits.ToString());
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        Vector3 velocity = direction * currentSpeed;

        if(!forbidSwitchingLanesWhileAirborne || controller.isGrounded)
        {
            bool toMoveRight = /*MobileInput.SwipeRight ||*/
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
            bool toMoveLeft = /*MobileInput.SwipeLeft ||*/
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
            targetPosition += Vector3.left * World.LANE_DISTANCE;
        }
        else if (desiredLane == (int)Lane.Right)
        {
            targetPosition += Vector3.right * World.LANE_DISTANCE;
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
            bool toJump = Input.GetKeyDown(KeyCode.Space);
            if (toJump)
            {
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
                if(clothingType == GameManager.ClothingTypeRequired)
                {
                    WasheItem();
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

    private void WasheItem()
    {
        washedItems += 1;
        InformationText.Instance.UpdateText(null, null, washedItems.ToString());
    }

    private void LoseALife()
    {
        lives -= 1;
        if (lives <= 0)
        {
            Lose();
            return;
        }
        InformationText.Instance.UpdateText(null, lives.ToString());
    }

    private void GainALife()
    {
        lives += 1;
        InformationText.Instance.UpdateText(null, lives.ToString());
    }

    private void Lose()
    {
        Destroy(gameObject);
    }
}

public enum Lane
{
    Left = 0,
    Right = 2
}
