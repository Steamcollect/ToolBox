using UnityEngine;

namespace BT.Audio
{
    [System.Serializable]
    public class Playlist
    {
        public AudioClip clip;

        [Space(10)]
        public float volumeMultiplier;
        public bool isLooping;
    }
}
