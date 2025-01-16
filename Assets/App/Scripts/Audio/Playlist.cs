using UnityEngine;

[System.Serializable]
public class Playlist
{
    public AudioClip clip;

    [Space(10)]
    public float volumMultiplier;
    public bool isLooping;
}