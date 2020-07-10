using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettings: Singleton<SoundSettings>
{
    public void PlaySound(SoundNames soundName)
    {
        SoundAudioClip chosenSound = GetChosenSoundByName(soundName);
        chosenSound.AudioSource.clip = chosenSound.AudioClip;
        chosenSound.AudioSource.Play();
    }

    public void StopSound(SoundNames soundName)
    {
        SoundAudioClip chosenSound = GetChosenSoundByName(soundName);
        chosenSound.AudioSource.clip = chosenSound.AudioClip;
        chosenSound.AudioSource.Stop();
    }

    private static SoundAudioClip GetChosenSoundByName(SoundNames soundName)
    {
        for (int i = 0; i < Sounds.Instance.SoundsAudioClips.Length; i++)
        {
            if (Sounds.Instance.SoundsAudioClips[i].SoundName == soundName)
            {
                return Sounds.Instance.SoundsAudioClips[i];
            }
        }

        Debug.LogError("Sound " + soundName + " not found!");
        return null;
    }
}

public enum SoundNames
{
    Background,
    CollectGood,
    CollectBad,
    Lose,
    GainLife
}
