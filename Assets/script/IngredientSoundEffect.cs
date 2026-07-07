using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class IngredientSoundEffect : MonoBehaviour, IPointerDownHandler
{
    public AudioClip soundEffect;
    public AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)] public float volume = 0.8f;
    public bool playOnPointerDown = true;
    public bool playOnMouseDown = false;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyAudioSourceSettings();
    }

    void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyAudioSourceSettings();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playOnPointerDown)
        {
            Play();
        }
    }

    void OnMouseDown()
    {
        if (playOnMouseDown)
        {
            Play();
        }
    }

    public void Play()
    {
        if (audioSource == null || soundEffect == null)
        {
            return;
        }

        audioSource.PlayOneShot(soundEffect, volume);
    }

    void ApplyAudioSourceSettings()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }
}
