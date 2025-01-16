using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Playlist[] playlists;

    [Header("References")]
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup soundMixerGroup;

    Queue<AudioSource> soundsGo = new Queue<AudioSource>();

    Transform playlistParent, soundParent;

    [Header("System References")]
    [SerializeField, Tooltip("Number of GameObject create on start for the sound")] int startingAudioObjectsCount = 30;

    [Header("Output")]
    [SerializeField] RSE_PlayClipAt rsePlayClipAt;

    private void OnEnable()
    {
        rsePlayClipAt.action += PlayClipAt;
    }
    private void OnDisable()
    {
        rsePlayClipAt.action -= PlayClipAt;
    }

    private void Start()
    {
        SetupParent();

        SetupPlaylist();

        // Create Audio Object
        for (int i = 0; i < startingAudioObjectsCount; i++)
        {
            soundsGo.Enqueue(CreateSoundsGO());
        }
    }

    /// <summary>
    /// Require the clip and the power of the sound
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="soundPower"></param>
    /// <param name="position of the sound"></param>
    void PlayClipAt(Sound sound, Vector3 position)
    {
        AudioSource tmpAudioSource;
        if (soundsGo.Count <= 0) tmpAudioSource = CreateSoundsGO();
        else tmpAudioSource = soundsGo.Dequeue();

        tmpAudioSource.transform.position = position;

        // Set the volum
        float volumMultiplier = Mathf.Clamp(sound.volumMultiplier, 0, 1);
        tmpAudioSource.volume = volumMultiplier;

        // Set the clip
        tmpAudioSource.clip = sound.clips.GetRandom();
        tmpAudioSource.Play();
        StartCoroutine(AddAudioSourceToQueue(tmpAudioSource));
    }
    IEnumerator AddAudioSourceToQueue(AudioSource current)
    {
        yield return new WaitForSeconds(current.clip.length);
        soundsGo.Enqueue(current);
    }

    AudioSource CreateSoundsGO()
    {
        AudioSource tmpAudioSource = new GameObject("Audio Go").AddComponent<AudioSource>();
        tmpAudioSource.transform.SetParent(soundParent);
        tmpAudioSource.outputAudioMixerGroup = soundMixerGroup;
        soundsGo.Enqueue(tmpAudioSource);

        return tmpAudioSource;
    }

    void SetupPlaylist()
    {
        foreach (Playlist playlist in playlists)
        {
            AudioSource audioSource = new GameObject("PlaylistGO").AddComponent<AudioSource>();
            audioSource.volume = playlist.volumMultiplier;
            audioSource.loop = playlist.isLooping;
            audioSource.outputAudioMixerGroup = musicMixerGroup;

            audioSource.clip = playlist.clip;
            audioSource.Play();
        }
    }

    void SetupParent()
    {
        playlistParent = new GameObject("PLAYLIST").transform;
        playlistParent.parent = transform;

        soundParent = new GameObject("SOUNDS").transform;
        soundParent.parent = transform;
    }
}