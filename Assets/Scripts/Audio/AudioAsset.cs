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

    [Header("Music")]
    [Range(0f, 1f)] public float MusicVolume = 0.05f;
    public AudioClip BackgroundMusic;
}