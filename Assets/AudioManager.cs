using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------ Audio Source ------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------ Audio Clip ------")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip hurt;
    public AudioClip gunShot;
    public AudioClip bulletDestroy;
    public AudioClip machineGunShots;
    public AudioClip explosion;
    public AudioClip spikeTrap;
    public AudioClip burner;
    public AudioClip acidTrap;
    public AudioClip wallArrow;
    public AudioClip impactSound;
    public AudioClip impactSound2;

    [SerializeField] bool isLevel = true;

    public void Start()
    {
        musicSource.clip = background;

        if (!isLevel)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PauseAllSFX()
    {
        SFXSource.mute = true;
    }

    public void PlayAllSFX()
    {
        SFXSource.mute = false;
    }
}
