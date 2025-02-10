using UnityEngine;

namespace BT.Audio
{
    public class MusicManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Playlist[] playlists;

        [Header("References")]
        [SerializeField] private AudioManager audioManager;

        private void Start()
        {
            audioManager.SetupPlaylist(playlists);
        }
    }
}
