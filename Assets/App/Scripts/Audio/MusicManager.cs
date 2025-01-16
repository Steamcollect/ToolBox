using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Playlist[] playlists;

    [Header("References")]
    [SerializeField] AudioManager audioManager;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        audioManager.SetupPlaylist(playlists);
    }
}