using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : Singleton<Settings>
{
    [System.Serializable]
    public struct SettingsBlock
    {
        public float timeToReachMaximumZSpeed;
        public float playerMinimumZSpeed;
        public float playerMaximumZSpeed;
        public float playerXSpeed;
        public int cameraOptionsIndex;
        public float playerJumpForce;
        public int livesAtStart;
        public bool forbidSwitchingLanesWhileAirborne;
        public bool useInputButtons;
    }

    private SettingsBlock _currentBlock;
    public SettingsBlock CurrentBlock
    {
        get { return _currentBlock; }
        set { _currentBlock = value; }
    }

    [SerializeField] private SettingsBlock defaultBlock;
    public SettingsBlock DefaultBlock
    {
        get { return defaultBlock; }
    }

    private const int CAMERA_OPTIONS_NUMBER = 4;
    public CameraOption[] CameraOptions { get; private set; }


    public CameraOption ChosenCameraOption
    {
        get
        {
            return CameraOptions[CurrentBlock.cameraOptionsIndex];
        }
    }

    public int CameraOptionsNumber
    {
        get { return CAMERA_OPTIONS_NUMBER; }
    }

    private void Awake()
    {
        SetCameraOptions();
        CurrentBlock = defaultBlock;
    }

    private void SetCameraOptions()
	{
		CameraOptions = new CameraOption[CAMERA_OPTIONS_NUMBER];
        int i = 0;

        CameraOptions[i++] = new CameraOption
           ("Back View", false, false, 62,
           new Vector3(0f, 7.3f, -11.3f), Quaternion.Euler(18.5f, 0, 0));

        CameraOptions[i++] = new CameraOption
            ("Back Top View", false, false, 62,
            new Vector3(0.0f, 10f, -19f), Quaternion.Euler(16, 0, 0));

        CameraOptions[i++] = new CameraOption
            ("Side View", false, false, 62,
            new Vector3(-6.55f, 8.68f, -15.8f), Quaternion.Euler(13.4f, 18.5f, 0));

        CameraOptions[i++] = new CameraOption
            ("Front View", false, false, 62,
            new Vector3(0.0f, 6.5f, 29.8f), Quaternion.Euler(17.5f, 180, 0), true);

    }
}

[System.Serializable]
public struct CameraOption 
{
    public string Name { get; set; }

    public bool ToFollowOnX { get; set; }
    public bool FollowOnY { get; set; }

    public float FieldOfView { get; set; }

    public Vector3 Offset { get; set; }

    public Quaternion Angle { get; set; }

    public bool SwapLeftAndRight { get; set; }

    public CameraOption 
        (string name, bool followOnX, bool followOnY, float fieldOfView, Vector3 offset, Quaternion angle, bool swapLeftAndRight=false)
    {
        Name = name;
        ToFollowOnX = followOnX;
        FollowOnY = followOnY;
        FieldOfView = fieldOfView;
        Offset = offset;
        Angle = angle;
        SwapLeftAndRight = swapLeftAndRight;
    }

}
