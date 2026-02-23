using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource SFXSource;
    [Range(0f, 1f)] public float SFXVolume = 0.75f;
    public AudioSource MusicSource;
    [Range(0f, 1f)] public float MusicVolume = 0.3f;


    [Header("SFX")]
    public AudioClip Error;
    public AudioClip CoinSpent;
    public AudioClip ItemGrab;
    public AudioClip ItemDrop;

    [Header("Music")]
    public AudioClip BackgroundMusic;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //PlayMusic(BackgroundMusic, true);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (MusicSource == null || clip == null) return;
        MusicSource.clip = clip;
        MusicSource.loop = loop;
        MusicSource.volume = MusicVolume;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 0.75f)
    {
        if (SFXSource == null) return;
        SFXSource.PlayOneShot(clip, volume);
    }
}
