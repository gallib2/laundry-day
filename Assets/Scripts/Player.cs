﻿using System.Collections;
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
    private float zAtStart;

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
    [SerializeField] private int LIVES_AT_START = 3;
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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Initialise();
    }

    private void Initialise()
    {
        timePassedSinceStart = 0;
        maximumSpeedReached = false;
        Lives = LIVES_AT_START;

        WashedItems = 0;

        MileageInUnits = 0;

        zAtStart = transform.position.z;
        InformationText.Instance.UpdateText
            (null, Lives.ToString(), WashedItems.ToString(), MileageInUnits.ToString());
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
            targetPosition += Vector3.left * World.LANE_HORIZONTAL_SPACING;
        }
        else if (desiredLane == (int)Lane.Right)
        {
            targetPosition += Vector3.right * World.LANE_HORIZONTAL_SPACING;
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
                if(clothingType == GameManager.CurrentClothingTypeRequired)
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
        WashedItems += 1;
        InformationText.Instance.UpdateText(null, null, WashedItems.ToString());
    }

    private void LoseALife()
    {
        Lives -= 1;
        if (Lives <= 0)
        {
            Lose();
            return;
        }
        InformationText.Instance.UpdateText(null, Lives.ToString());
    }

    private void GainALife()
    {
        Lives += 1;
        InformationText.Instance.UpdateText(null, Lives.ToString());
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
