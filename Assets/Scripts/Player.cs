using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Movement:
    [Header("Movement:")]
    private CharacterController controller;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private bool forbidSwitchingLanesWhileAirborne = true;
    [SerializeField] private float gravity = 1.0f;
    private float yVelocity = 0.0f;
    private float xVelocity = 0.0f;

    private int desiredLane = 1;
    #endregion
    #region Lives:
    [Header("Lives:")]
    private int lives;
    [SerializeField] private int LIVES_AT_START = 3;
    #endregion

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Initialise();
    }

    private void Initialise()
    {
        lives = LIVES_AT_START;
        InformationText.Instance.UpdateText(null, lives.ToString());

    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        Vector3 velocity = direction * speed;

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

        xVelocity = (targetPosition - transform.position).normalized.x * speed;

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
                    Debug.Log("GOOD!");
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

    private void LoseALife()
    {
        lives -= 1;
        if (lives < 0)
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
