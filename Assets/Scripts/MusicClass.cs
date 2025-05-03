using UnityEngine;
using UnityEngine.Audio;

public class MusicClass : MonoBehaviour
{
    private AudioSource _audioSource;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void MuteMusic()
    {
        _audioSource.mute = !_audioSource.mute;
    }

}