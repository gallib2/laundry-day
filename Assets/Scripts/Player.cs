using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float jumpHeight = 10.0f;
    [SerializeField]
    private float gravity = 1.0f;
    private float yVelocity = 0.0f;
    private float xVelocity = 0.0f;

    private const float LANE_DISTANCE = 5.0f;
    private int desiredLane = 1;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        Vector3 velocity = direction * speed;

        bool toMoveRight = /*MobileInput.SwipeRight ||*/ Input.GetKeyDown(KeyCode.RightArrow);
        bool toMoveLeft = /*MobileInput.SwipeLeft ||*/ Input.GetKeyDown(KeyCode.LeftArrow);
        if (toMoveLeft)
        {
            MoveLane(false);
        }
        if (toMoveRight)
        {
            MoveLane(true);
        }

        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == (int)Lane.Left)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == (int)Lane.Right)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
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
                yVelocity = jumpHeight;
            }
        }
        else
        {
            yVelocity -= gravity;
        }
    }
}

public enum Lane
{
    Left = 0,
    Right = 2
}
