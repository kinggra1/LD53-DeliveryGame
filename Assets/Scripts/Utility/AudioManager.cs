using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    public AudioClip backgroundMusic;

    public AudioClip softBoop;
    public AudioClip harshCutBoop;

    public AudioClip slurp;

    private AudioSource generalSfxAudioSource;
    private AudioSource musicAudioSource;

    private void Awake() {
        generalSfxAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.volume = 0.1f;
        musicAudioSource.clip = backgroundMusic;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    public void PlaySoftBoop() {
        generalSfxAudioSource.volume = (Random.Range(0.5f, 0.6f));
        generalSfxAudioSource.pitch = (Random.Range(0.8f, 1f));
        generalSfxAudioSource.PlayOneShot(softBoop);
    }

    public void PlayHarshCutBoop() {
        generalSfxAudioSource.volume = (Random.Range(0.5f, 0.6f));
        generalSfxAudioSource.pitch = (Random.Range(0.8f, 1f));
        generalSfxAudioSource.PlayOneShot(harshCutBoop);
    }

    public void PlaySlurp() {
        generalSfxAudioSource.volume = 0.3f;
        generalSfxAudioSource.pitch = (Random.Range(0.8f, 1.1f));
        generalSfxAudioSource.PlayOneShot(slurp);
    }
}
