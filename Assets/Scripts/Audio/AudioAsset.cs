using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Asset", menuName = "Audio Manager/Audio Asset")]
public class AudioAsset : ScriptableObject
{
    [Header("SFX")]
    [Range(0f, 1f)] public float SFXVolume = 0.6f;
    public AudioClip Error;
    public AudioClip ItemSelect;
    public AudioClip ItemGrab;
    public AudioClip ItemDrop;
    public AudioClip CoinSpent;
    public AudioClip DrinkPotion;
    public AudioClip FoodEaten;
    public AudioClip DamageReceived;
    public AudioClip PopSound;
    public AudioClip DoorOpen;
    public AudioClip DoorClose;
    public AudioClip DefeatSound;

    [Header("Music")]
    [Range(0f, 1f)] public float MusicVolume = 0.2f;
    public AudioClip BackgroundMusic;
}