using UnityEngine;

public class SoundComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Sound sound;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSE_PlayClipAt rsePlayClipAt;

    public void PlayClip()
    {
        rsePlayClipAt.Call(sound, transform.position);
    }
}