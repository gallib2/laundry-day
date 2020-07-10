using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : Singleton<Sounds>
{
	[SerializeField]
	private SoundAudioClip[] soundAudioClips;

	public SoundAudioClip[] SoundsAudioClips
	{
		get { return soundAudioClips; }
		set { soundAudioClips = value; }
	}
}

[System.Serializable]
public class SoundAudioClip
{
	[SerializeField]
	private SoundNames soundName;

	[SerializeField]
	private AudioClip audioClip;

	[SerializeField]
	private AudioSource audioSource;

	public SoundNames SoundName
	{
		get { return soundName; }
		set { soundName = value; }
	}

	public AudioClip AudioClip
	{
		get { return audioClip; }
		set { audioClip = value; }
	}

	public AudioSource AudioSource
	{
		get { return audioSource; }
		set { audioSource = value; }
	}

}
