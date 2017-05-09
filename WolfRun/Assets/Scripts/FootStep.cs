using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStep : MonoBehaviour {

    public Vector2 randomPitchRange = new Vector2(0.8f, 1.0f);
    public Vector2 randomVolumeRange = new Vector2(0.8f, 1.0f);
    public AudioClip[] footStepSounds;
    public AudioClip[] doubleStepSounds;

    private AudioSource audioSource;
    public GameObject soundSourceObject;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnLeftStep()
    {
        AssignRandomFootStep(true);
        AudioSource.PlayClipAtPoint(audioSource.clip, soundSourceObject.transform.position);
    }

    private void OnRightStep()
    {
        AssignRandomFootStep(true);
        AudioSource.PlayClipAtPoint(audioSource.clip, soundSourceObject.transform.position);
    }

    private void OnDoubleStep()
    {
        AssignRandomFootStep(false);
        AudioSource.PlayClipAtPoint(audioSource.clip, soundSourceObject.transform.position);
    }

    private void AssignRandomFootStep(bool isSingleStep)
    {
        int randomIndex;
        AudioClip randomStepSound;

        randomIndex = isSingleStep ? Random.Range(0, footStepSounds.Length) :
                                     Random.Range(0, doubleStepSounds.Length);

        randomStepSound = footStepSounds[randomIndex];
        audioSource.clip = randomStepSound;

        audioSource.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
        audioSource.volume = Random.Range(randomVolumeRange.x, randomVolumeRange.y);
    }
}
