using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    public AudioClip musicClip;
    public AudioClip music1;
    public AudioClip music2;
    public AudioClip music3;
    public AudioMixerGroup musicMixerGroup;
    [Range(0f, 1f)] public float volume = 0.7f;
    public bool playOnStart = true;
    public bool loop = true;
    public bool dontDestroyOnLoad = true;
    public bool keepSingleInstance = true;
    public bool replaceExistingMusic = false;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (keepSingleInstance && Instance != null && Instance != this)
        {
            if (replaceExistingMusic)
            {
                Instance.PlayMusic(musicClip);
            }

            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        ApplyAudioSourceSettings();
    }

    void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyAudioSourceSettings();
    }

    public void Play()
    {
        ApplyAudioSourceSettings();

        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogWarning("BackgroundMusic has no music clip assigned.", this);
            return;
        }



        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null)
        {
            return;
        }

        if (musicClip == newClip && audioSource != null && audioSource.isPlaying)
        {
            return;
        }

        musicClip = newClip;
        ApplyAudioSourceSettings();
        Play();
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        ApplyAudioSourceSettings();
    }

    void ApplyAudioSourceSettings()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.clip = musicClip;
        audioSource.outputAudioMixerGroup = musicMixerGroup;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

}
