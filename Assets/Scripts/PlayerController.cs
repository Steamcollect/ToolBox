using MVsToolkit.Dev;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Foldout("Test")]
    public string playerName;
    public int playerID;

    [Tab("Stats")]
    public int health;
    public int damage;

    [Tab("Dash")]
    public float dashSpeed;
    public float fadeSpeed;
    public float fadeTime;

    [Tab("Fade")]
    public float fadeDuration;
    public float fadeDelay;
    public float fadeDelayTime;
}
