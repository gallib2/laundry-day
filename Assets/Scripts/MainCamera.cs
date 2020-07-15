using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform anchor;
    [SerializeField] private Animator anchorAnimator;
    [SerializeField]  private float lerpSpeed = 2f;
    private const float IN_GAME_CLIPPING = 5;
    private const float CINEMATIC_CLIPPING = 0.01f;

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

        if (GameManager.InBeginingScreen)
        {
            Camera.main.nearClipPlane = CINEMATIC_CLIPPING;
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
        }
        else
        {
            if (!GameManager.GameIsPaused )
            {
                if (followAnchor)
                {
                    Camera.main.nearClipPlane = CINEMATIC_CLIPPING;
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
                        Camera.main.nearClipPlane = IN_GAME_CLIPPING;
                        transform.rotation = cameraOption.Angle;
                        transform.position = newPosition;
                    }
                }

            }
        }
        
    }
}
