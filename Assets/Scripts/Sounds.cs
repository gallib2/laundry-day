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

    [SerializeField] private bool loop;
    [SerializeField] private bool isPausable;
    [SerializeField][Range(0,1)] private float volume =1;


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

    public bool Loop
    {
        get { return loop; }
        set { loop = value; }
    }

    public bool IsPausable
    {
        get { return isPausable; }
        set { isPausable = value; }
    }

    public float Volume
    {
        get { return volume; }
    }
}
