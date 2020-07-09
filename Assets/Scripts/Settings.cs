using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : Singleton<Settings>
{
	private const int CAMERA_OPTIONS_NUMBER = 3;

	private CameraOption chosenCameraOption;
	public int PlayerMinimumSpeed { get; set; }
	public bool SetMinSpeed { get; set; }
	public int PlayerMaximumSpeed { get; set; }
	public bool SetMaxSpeed { get; set; }
	public bool IsSetCameraOptions { get; set; }
	public CameraOption[] CameraOptions { get; private set; }
	public float PlayeJumpForce { get; set; }
	public bool IsSetJumpForce { get; set; }


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
		CameraOptions[0].FieldOfView = 65.0f;
		CameraOptions[0].Offset = new Vector3(0.0f, 5.0f, -20f);

		CameraOptions[1].Name = "Follow Player";
		CameraOptions[1].ToFollowOnX = true;
		CameraOptions[1].FieldOfView = 70.0f;
		CameraOptions[1].Offset = new Vector3(0.0f, 2.5f, -18f);

		CameraOptions[2].Name = "Option z";
		CameraOptions[2].ToFollowOnX = false;
		CameraOptions[2].FieldOfView = 65.0f;
		CameraOptions[2].Offset = new Vector3(0.0f, 10.0f, -20f);
	}
}

[System.Serializable]
public struct CameraOption 
{
    public string Name { get; set; }

    public bool ToFollowOnX { get; set; }

    public float FieldOfView { get; set; }

    public Vector3 Offset { get; set; }

}
