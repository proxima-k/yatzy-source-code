using UnityEngine;
using Random = UnityEngine.Random;

public class SoundHandler : MonoBehaviour {
    
    public static SoundHandler Instance;
    
    public AudioClipRefsSO AudioClipRefsSo => _audioClipRefsSO;
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    
    public bool IsMuted => _isMuted;
    private bool _isMuted = false;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f) {
        if (_isMuted)
            return;
        
        PlaySound(audioClips[Random.Range(0, audioClips.Length)], position, volume);
    }
    
    public void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        if (_isMuted)
            return;
        
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    
    public void ToggleMute() {
        _isMuted = !_isMuted;
    }
}
