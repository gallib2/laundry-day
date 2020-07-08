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
		CameraOptions[0].Position = new Vector3(0, 3.4f, -10.91f);
		CameraOptions[0].Rotation = Quaternion.Euler(3.52f, 0f, 0f);

		CameraOptions[1].Name = "Option y";
		CameraOptions[1].Position = new Vector3(0, 2f, 0f);
		CameraOptions[1].Rotation = Quaternion.Euler(0, 0f, 0f);

		CameraOptions[2].Name = "Option z";
		CameraOptions[2].Position = new Vector3(0, 3.4f, -10.91f);
		CameraOptions[2].Rotation = Quaternion.Euler(3.52f, 0f, 0f);
	}
}

[System.Serializable]
public struct CameraOption 
{
	private Vector3 position;
	public Quaternion rotation;

    public string Name { get; set; }

    public Quaternion Rotation
	{
		get { return rotation; }
		set { rotation = value; }
	}

	public Vector3 Position
	{
		get { return position; }
		set { position = value; }
	}
}
