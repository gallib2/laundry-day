using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float xPosition = 0f;
    private float yPosition = 0f;
    private CameraOption cameraOption;
    // [SerializeField] private Vector3 initialPosition;
    //[SerializeField] private Vector3 initialAngle;
    [SerializeField] private Transform anchor;
    [SerializeField] private Animator anchorAnimator;
    private bool followAnchor = true;
    [SerializeField]  private float lerpSpeed = 2f;

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
        Initialise();
    }

    private void Initialise()
    {
        cameraOption = Settings.Instance.ChosenCameraOption;

        Camera.main.fieldOfView = cameraOption.FieldOfView;
        followAnchor = true;
        anchorAnimator.SetTrigger("Play");
       // transform.position = initialPosition;
       //transform.rotation = Quaternion.Euler(initialAngle);

    }

    public void Release()
    {
        Debug.Log("Release()");
        followAnchor = false;
    }

    void Update()
    {

        if (!GameManager.GameIsPaused)
        {
            if (followAnchor)
            {
                transform.position = anchor.position;
                transform.rotation = anchor.rotation;
            }
            else
            {
                xPosition = cameraOption.ToFollowOnX ? target.position.x : 0;
                yPosition = cameraOption.FollowOnY ? target.position.y : 0;
                Vector3 newPosition = new Vector3(xPosition, yPosition, target.position.z) + cameraOption.Offset;
                if (GameManager.IntroIsPlaying)
                {
                    transform.position = 
                        Vector3.Lerp(transform.position, newPosition, lerpSpeed * Time.deltaTime);
                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, cameraOption.Angle, lerpSpeed * Time.deltaTime);

                   // transform.rotation = Quaternion.RotateTowards(transform.rotation, cameraOption.Angle, 52f * Time.deltaTime);
                    // transform.position = Vector3.Lerp(transform.position, newPosition, 0.77f * Time.deltaTime);
                   // transform.position = Vector3.MoveTowards(transform.position, newPosition, 11f * Time.deltaTime);

                }
                else
                {
                    transform.rotation = cameraOption.Angle;
                    transform.position = newPosition;
                }
            }

        }
    }
}
