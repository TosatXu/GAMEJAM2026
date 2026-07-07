using UnityEngine;
using UnityEngine.Audio;

public class TimingBar : MonoBehaviour
{
    public PotionMechanicsManager potionMechanicsManager;

    public Transform leftPoint;
    public Transform rightPoint;
    public Transform marker;

    [Header("Timing Sound")]
    public AudioSource timingAudioSource;
    public AudioClip timingStartSound;
    public AudioMixerGroup sfxMixerGroup;

    [Range(0f, 1f)] public float timingStartVolume = 0.8f;
    public bool playTimingStartSound = true;
    public bool loopTimingSound = true;

    [Range(0f, 100f)] public float redZoneStart = 40f;
    [Range(0f, 100f)] public float redZoneEnd = 60f;

    public float markerSpeed = 1.5f;

    bool isPlaying;
    float timer;
    float currentPercent;

    void Start()
    {
        if (potionMechanicsManager == null)
        {
            potionMechanicsManager = FindFirstObjectByType<PotionMechanicsManager>();
        }

        SetupTimingAudioSource();
        gameObject.SetActive(false);
    }

    void OnValidate()
    {
        SetupTimingAudioSource();
    }

    void OnDisable()
    {
        StopTimingStartSound();
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        timer += Time.deltaTime * markerSpeed;

        float move01 = Mathf.PingPong(timer, 1f);
        currentPercent = move01 * 100f;

        marker.position = Vector3.Lerp(
            leftPoint.position,
            rightPoint.position,
            move01
        );

        if (Input.GetMouseButtonDown(0))
        {
            CheckTiming();
        }
    }

    public void StartTiming()
    {
        gameObject.SetActive(true);

        if (potionMechanicsManager == null)
        {
            potionMechanicsManager = FindFirstObjectByType<PotionMechanicsManager>();
        }

        isPlaying = true;
        timer = 0f;
        currentPercent = 0f;

        Debug.Log("Fire timing started.");

        PlayTimingStartSound();
    }

    void CheckTiming()
    {
        isPlaying = false;
        StopTimingStartSound();

        bool success = currentPercent >= redZoneStart && currentPercent <= redZoneEnd;

        Debug.Log(
            "Fire timing: " +
            currentPercent.ToString("0.0") +
            "% | " +
            (success ? "Good fire" : "Bad fire")
        );

        if (potionMechanicsManager != null)
        {
            potionMechanicsManager.ReceiveFireTimingResult(success);
        }
        else
        {
            Debug.LogWarning("No PotionMechanicsManager found for timing bar.", this);
        }
    }

    void PlayTimingStartSound()
    {
        if (!playTimingStartSound)
        {
            return;
        }

        SetupTimingAudioSource();

        if (timingAudioSource == null || timingStartSound == null)
        {
            return;
        }

        timingAudioSource.clip = timingStartSound;
        timingAudioSource.volume = timingStartVolume;
        timingAudioSource.loop = loopTimingSound;
        timingAudioSource.Play();
    }

    void StopTimingStartSound()
    {
        if (timingAudioSource != null && timingAudioSource.isPlaying)
        {
            timingAudioSource.Stop();
        }
    }

    void SetupTimingAudioSource()
    {
        if (timingAudioSource == null)
        {
            timingAudioSource = GetComponent<AudioSource>();
        }

        if (timingAudioSource == null)
        {
            return;
        }

        timingAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        timingAudioSource.playOnAwake = false;
        timingAudioSource.loop = loopTimingSound;
        timingAudioSource.spatialBlend = 0f;
    }
}
