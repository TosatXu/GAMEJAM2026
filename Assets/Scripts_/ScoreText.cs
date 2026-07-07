using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class ScoreText : MonoBehaviour
{
    [Header("Success Sound")]
    public AudioSource successAudioSource;
    public AudioClip successSound;
    public AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)] public float successSoundVolume = 0.8f;
    public float successScore = 100f;
    public bool playSuccessSound = true;

    void Start()
    {
        float score = GetScore();

        GetComponent<TextMeshProUGUI>().text = Convert.ToString(score);
        SetupSuccessSound();

        if (score >= successScore)
        {
            PlaySuccessSound();
        }
    }

    float GetScore()
    {
        GameObject runtimeDataObject = GameObject.Find("RecipeRuntimeData");
        if (runtimeDataObject == null)
        {
            return 0f;
        }

        RecipeRuntimeData runtimeData = runtimeDataObject.GetComponent<RecipeRuntimeData>();
        if (runtimeData == null)
        {
            return 0f;
        }

        return runtimeData.lastPotionQuality;
    }

    void SetupSuccessSound()
    {
        if (successAudioSource == null)
        {
            successAudioSource = GetComponent<AudioSource>();
        }

        if (successAudioSource == null)
        {
            return;
        }

        successAudioSource.clip = successSound;
        successAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        successAudioSource.volume = successSoundVolume;
        successAudioSource.playOnAwake = false;
        successAudioSource.loop = false;
        successAudioSource.spatialBlend = 0f;
    }

    void PlaySuccessSound()
    {
        if (!playSuccessSound)
        {
            return;
        }

        SetupSuccessSound();

        if (successAudioSource == null || successSound == null)
        {
            return;
        }

        successAudioSource.PlayOneShot(successSound, successSoundVolume);
    }
}
