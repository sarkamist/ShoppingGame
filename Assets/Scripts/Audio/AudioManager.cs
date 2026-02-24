using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource SFXSource;
    public AudioSource MusicSource;

    public AudioAsset Data;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(Data.BackgroundMusic, true);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (MusicSource == null || clip == null) return;
        MusicSource.clip = clip;
        MusicSource.loop = loop;
        MusicSource.volume = Data.MusicVolume;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float? volume = null)
    {
        if (SFXSource == null) return;
        SFXSource.PlayOneShot(clip, volume.GetValueOrDefault(Data.SFXVolume));
    }
}
