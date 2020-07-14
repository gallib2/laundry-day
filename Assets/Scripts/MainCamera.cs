using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform anchor;
    [SerializeField] private Animator anchorAnimator;
    [SerializeField]  private float lerpSpeed = 2f;

    private float xPosition = 0f;
    private float yPosition = 0f;
    private CameraOption cameraOption;
    private bool followAnchor = true;

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
    }

    public void Release()
    {
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
