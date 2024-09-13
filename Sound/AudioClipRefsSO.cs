using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRefsSO", menuName = "ScriptableObjects/AudioClipRefsSO", order = 1)]
public class AudioClipRefsSO : ScriptableObject {

    [Header("Connection")]
    public AudioClip ConnectionSuccess;
    public AudioClip ConnectionFail;
    public AudioClip PlayerJoin;
    public AudioClip PlayerLeave;

    [Header("Button")]
    public AudioClip ButtonHover;
    public AudioClip ButtonClick;
    
    [Header("Dice")]
    public AudioClip[] DiceHit;
    public AudioClip[] TableHit;
    
    public AudioClip[] DiceGrab;
}
