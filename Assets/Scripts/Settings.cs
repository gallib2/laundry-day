using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : Singleton<Settings>
{
	private const int CAMERA_OPTIONS_NUMBER = 7;

	private CameraOption chosenCameraOption;
	public int PlayerMinimumSpeed { get; set; }//Why is this an int?
    public bool SetMinSpeed { get; set; }
    public int PlayerMaximumSpeed { get; set; }//Why is this an int?
	public bool SetMaxSpeed { get; set; }
    public int PlayerXSpeed { get; set; } //Why is this an int?
    public bool PlayerXSpeedIsSet { get; set; }
    public bool IsSetCameraOptions { get; set; }
	public CameraOption[] CameraOptions { get; private set; }
	public float PlayeJumpForce { get; set; }
	public bool IsSetJumpForce { get; set; }

    public int LivesAtStart { get; set; }
    public bool LivesAtStartIsSet { get; set; }
    public bool ForbidSwitchingLanesWhileAirborne { get; set; }
    public bool ForbidSwitchingLanesWhileAirborneIsSet { get; set; }


    public CameraOption ChosenCameraOption
	{
		get { return chosenCameraOption; }
		set { chosenCameraOption = value; }
	}

	public int CameraOptionsNumber
	{
		get { return CAMERA_OPTIONS_NUMBER; }
	}

	protected Settings()
	{
		CameraOptions = new CameraOption[CAMERA_OPTIONS_NUMBER];
		SetCameraOptions();
	}

	private void SetCameraOptions()
	{
		CameraOptions = new CameraOption[CAMERA_OPTIONS_NUMBER];
		CameraOptions[0].Name = "Option x";
		CameraOptions[0].ToFollowOnX = false;
        CameraOptions[0].FollowOnY = true;
        CameraOptions[0].FieldOfView = 65.0f;
		CameraOptions[0].Offset = new Vector3(0.0f, 5.0f, -20f);

		CameraOptions[1].Name = "Follow Player";
		CameraOptions[1].ToFollowOnX = true;
        CameraOptions[1].FollowOnY = true;
        CameraOptions[1].FieldOfView = 70.0f;
		CameraOptions[1].Offset = new Vector3(0.0f, 2.5f, -18f);

		CameraOptions[2].Name = "Option z";
		CameraOptions[2].ToFollowOnX = true;
        CameraOptions[2].FollowOnY = true;
        CameraOptions[2].FieldOfView = 65.0f;
		CameraOptions[2].Offset = new Vector3(0.0f, 10.0f, -20f);

        CameraOptions[3].Name = "ThirdPerson";
        CameraOptions[3].ToFollowOnX = false;
        CameraOptions[3].FollowOnY = true;
        CameraOptions[3].FieldOfView = 62f;
        CameraOptions[3].Offset = new Vector3(0.0f, 10f, -19f);
        CameraOptions[3].Angle = Quaternion.Euler(16, 0, 0);

        CameraOptions[4].Name = "ThirdPersonFront";
        CameraOptions[4].ToFollowOnX = false;
        CameraOptions[4].FollowOnY = true;
        CameraOptions[4].FieldOfView = 62f;
        CameraOptions[4].Offset = new Vector3(0.0f, 4f, 20f);
        CameraOptions[4].Angle = Quaternion.Euler(16, 180, 0);

        CameraOptions[5].Name = "ThirdPersonYonatan";
        CameraOptions[5].ToFollowOnX = false;
        CameraOptions[5].FollowOnY = false;
        CameraOptions[5].FieldOfView = 60f;
        CameraOptions[5].Offset = new Vector3(0f, 4.9f, -12.69f);
        CameraOptions[5].Angle = Quaternion.Euler(8.76f, 0, 0);

        CameraOptions[6] = new CameraOption
            ("SideView1", false, false, 62, new Vector3(-6.3f, 6.75f, -12.65f), Quaternion.Euler(10.55f, 21.55f, 0));

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

    public CameraOption 
        (string name, bool followOnX, bool followOnY, float fieldOfView, Vector3 offset, Quaternion angle)
    {
        Name = name;
        ToFollowOnX = followOnX;
        FollowOnY = followOnY;
        FieldOfView = fieldOfView;
        Offset = offset;
        Angle = angle;
    }

}
