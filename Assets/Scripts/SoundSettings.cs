using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettings: Singleton<SoundSettings>
{
    [SerializeField] private float JumpPitchAlteration = 0.08f;
    [SerializeField] private float goodCollectionPitchAddition = 0.1f;
    private float goodCollectionPitch;

    private void OnEnable()
    {
        Player.OnWashedItemsChanged += DetermineGoodCollectionPitch;
    }

    private void OnDisable()
    {
        Player.OnWashedItemsChanged -= DetermineGoodCollectionPitch;
    }

    private void DetermineGoodCollectionPitch(System.UInt32 washedItems, System.UInt32 washedItemsCombo)
    {
        goodCollectionPitch = (1 + ((float)washedItemsCombo * goodCollectionPitchAddition));
    }

    public void PlaySound(SoundNames soundName)
    {

        SoundAudioClip chosenSound = GetChosenSoundByName(soundName);
        AudioSource source = chosenSound.AudioSource;
        if (soundName == SoundNames.Jump)
        {
            source.pitch = 1 + (Random.Range(-JumpPitchAlteration, JumpPitchAlteration));
        }
        else if(soundName == SoundNames.CollectGood)
        {
            source.pitch = goodCollectionPitch;
        }
        else
        {
            source.pitch = 1;
        }

        source.loop = chosenSound.Loop;

        source.clip = chosenSound.AudioClip;
        source.Play();
    }

    public void StopSound(SoundNames soundName)
    {
        SoundAudioClip chosenSound = GetChosenSoundByName(soundName);
        //chosenSound.AudioSource.clip = chosenSound.AudioClip;
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
    GainLife,
    Jump,
    TickingClock,
    ClothingTypeRequiredChange,
    DoorOpening

}
