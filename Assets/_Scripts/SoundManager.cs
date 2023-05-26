using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance { get; private set; }

    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int audioClipNumber, float volume)
    {
        _audioSource.PlayOneShot(_audioClips[audioClipNumber], volume);
    }
}
