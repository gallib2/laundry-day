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

    private CameraOption cameraOption;
    private bool followAnchor = true;

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

    private void Initialise()
    {
        cameraOption = Settings.Instance.ChosenCameraOption;
        Camera.main.fieldOfView = cameraOption.FieldOfView;
    }

    private void ConformToNewGameState(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.InGame || gameState == GameManager.GameState.GameOver)
        {
            Camera.main.nearClipPlane = IN_GAME_CLIPPING;
        }
        else
        {
            followAnchor = true;
            Camera.main.nearClipPlane = CINEMATIC_CLIPPING;

            if (gameState == GameManager.GameState.Intro)
            {
                anchorAnimator.SetTrigger("PlayIntro");
            }
        }
    }

    public void Release()
    {
        followAnchor = false;
    }

    void Update()
    {
        if (!GameManager.GameIsPaused )
        {
            switch (GameManager.CurrentGameState)
            {
                case GameManager.GameState.InGame:
                    InGameMove();
                    break;
                case GameManager.GameState.Intro:
                case GameManager.GameState.BeginingScreen:
                    PreInGameMove();
                    break;
            }
        }
    }

    private bool followTarget = true;
    float followTargetLerpT = 1;
   // float followTargetReductionPerSecond = 0.3f;
    private void InGameMove()
    {
        if (followTarget)
        {
            transform.rotation = cameraOption.Angle;
            transform.position = GetTargetPosition();
            if (Input.GetKeyDown(KeyCode.C))
            {
                followTarget = false;
            }
        }
       else if(followTargetLerpT>0)
        {
            Vector3 currentPosition = transform.position;
            transform.position = Vector3.Lerp(currentPosition, GetTargetPosition(), followTargetLerpT);
            // followTargetLerpT -= (followTargetReductionPerSecond * Time.deltaTime);
             followTargetLerpT -= ((followTargetLerpT*6) * Time.deltaTime);

        }

    }

    private void PreInGameMove()
    {
        if (followAnchor)
        {
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraOption.Angle, lerpSpeed * Time.deltaTime);           
        }
    }

    private Vector3 GetTargetPosition()
    {
        return cameraOption.Offset + new Vector3
             (cameraOption.ToFollowOnX ? target.position.x : 0,
             cameraOption.FollowOnY ? target.position.y : 0,
             target.position.z);
    }
}
