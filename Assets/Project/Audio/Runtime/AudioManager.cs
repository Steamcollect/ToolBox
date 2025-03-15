using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    AudioMixerGroup musicMixerGroup;

    [SerializeField] AudioMixerGroup soundMixerGroup;

    private readonly Queue<AudioSource> _soundsGo = new();

    private Transform _playlistParent, _soundParent;

    [Header("System References")] 
    [SerializeField, Tooltip("Number of GameObject create on start for the sound")]
    private int startingAudioObjectsCount = 30;

    [Header("Output")] [SerializeField] RSE_PlayClipAt rsePlayClipAt;

    private void OnEnable()
    {
        rsePlayClipAt.Action += PlayClipAt;
    }

    private void OnDisable()
    {
        rsePlayClipAt.Action -= PlayClipAt;
    }

    private void Start()
    {
        SetupParent();

        // Create Audio Object
        for (int i = 0; i < startingAudioObjectsCount; i++)
        {
            _soundsGo.Enqueue(CreateSoundsGo());
        }
    }

    /// <summary>
    /// Play clip at a specific position
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="position"></param>
    void PlayClipAt(Sound sound, Vector3 position)
    {
        AudioSource tmpAudioSource;
        if (_soundsGo.Count <= 0) tmpAudioSource = CreateSoundsGo();
        else tmpAudioSource = _soundsGo.Dequeue();

        tmpAudioSource.transform.position = position;


        float volumeMultiplier = Mathf.Clamp(sound.volumeMultiplier, 0, 1);
        tmpAudioSource.volume = volumeMultiplier;
        tmpAudioSource.spatialBlend = sound.spatialBlend;


        tmpAudioSource.clip = sound.clips.GetRandom();
        tmpAudioSource.Play();
        StartCoroutine(AddAudioSourceToQueue(tmpAudioSource));
    }

    private IEnumerator AddAudioSourceToQueue(AudioSource current)
    {
        yield return new WaitForSeconds(current.clip.length);
        _soundsGo.Enqueue(current);
    }

    private AudioSource CreateSoundsGo()
    {
        AudioSource tmpAudioSource = new GameObject("Audio Go").AddComponent<AudioSource>();
        tmpAudioSource.transform.SetParent(_soundParent);
        tmpAudioSource.outputAudioMixerGroup = soundMixerGroup;
        return tmpAudioSource;
    }

    public void SetupPlaylist(Playlist[] playlists)
    {
        foreach (Playlist playlist in playlists)
        {
            AudioSource audioSource = new GameObject("PlaylistGO").AddComponent<AudioSource>();
            audioSource.volume = playlist.volumeMultiplier;
            audioSource.loop = playlist.isLooping;
            audioSource.outputAudioMixerGroup = musicMixerGroup;

            audioSource.clip = playlist.clip;
            audioSource.Play();
        }
    }

    private void SetupParent()
    {
        _playlistParent = new GameObject("PLAYLIST").transform;
        _playlistParent.parent = transform;

        _soundParent = new GameObject("SOUNDS").transform;
        _soundParent.parent = transform;
    }
}