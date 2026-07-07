using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MouseClickSound : MonoBehaviour
{
    public static MouseClickSound Instance { get; private set; }

    public AudioClip clickClip;
    public AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)] public float volume = 0.8f;
    public bool playLeftClick = true;
    public bool dontDestroyOnLoad = true;
    public bool keepSingleInstance = true;
    public float minimumTimeBetweenClicks = 0.03f;

    AudioSource audioSource;
    float lastClickTime = -999f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (keepSingleInstance && Instance != null && Instance != this)
        {
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

    void Update()
    {
        if (playLeftClick && Input.GetMouseButtonDown(0))
        {
            PlayClick();
        }

    }

    void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyAudioSourceSettings();
    }

    public void PlayClick()
    {
        if (Time.unscaledTime - lastClickTime < minimumTimeBetweenClicks)
        {
            return;
        }

        lastClickTime = Time.unscaledTime;

        if (audioSource == null || clickClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clickClip, volume);
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
