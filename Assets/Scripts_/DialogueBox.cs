using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class DialogueBox : MonoBehaviour
{
    public int dialogueNum;
    public int encounterCounter;
    public string[] intro1 = new string[4];
    public string[] intro2 = new string[4];
    public string[] intro3 = new string[4];
    public TextMeshProUGUI display;
    public GameObject nextButton;

    [Header("NPC Sound")]
    public AudioSource npcAudioSource;
    public AudioMixerGroup sfxMixerGroup;
    public AudioClip npc1Sound;
    public AudioClip npc2Sound;
    public AudioClip npc3Sound;
    [Range(0f, 1f)] public float npcSoundVolume = 0.8f;
    public bool playNpcSoundOnAppear = false;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("encounterNum"))
        {
            PlayerPrefs.SetInt("encounterNum", -1);
        }
    }

    void OnEnable()
    {
        PlayerPrefs.SetInt("encounterNum", PlayerPrefs.GetInt("encounterNum") + 1);
        encounterCounter = PlayerPrefs.GetInt("encounterNum") % 3;
        PlayerPrefs.SetInt("encounterNum", encounterCounter);

        if (playNpcSoundOnAppear)
        {
            PlayCurrentNpcSound();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(encounterCounter);
            Debug.Log(PlayerPrefs.GetInt("encounterNum"));

            if (encounterCounter == 0)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro1[dialogueNum];
            }
            if (encounterCounter == 1)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro2[dialogueNum];
            }
            if (encounterCounter == 2)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro3[dialogueNum];
            }
        }

        if (dialogueNum >= 3)
        {
            nextButton.SetActive(true);
        }
    }

    void PlayCurrentNpcSound()
    {
        if (npcAudioSource == null)
        {
            npcAudioSource = GetComponent<AudioSource>();
        }

        if (npcAudioSource == null)
        {
            return;
        }

        AudioClip clipToPlay = null;

        if (encounterCounter == 0)
        {
            clipToPlay = npc1Sound;
        }
        else if (encounterCounter == 1)
        {
            clipToPlay = npc2Sound;
        }
        else if (encounterCounter == 2)
        {
            clipToPlay = npc3Sound;
        }

        if (clipToPlay == null)
        {
            return;
        }

        npcAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        npcAudioSource.playOnAwake = false;
        npcAudioSource.loop = false;
        npcAudioSource.spatialBlend = 0f;
        npcAudioSource.PlayOneShot(clipToPlay, npcSoundVolume);
    }
}
