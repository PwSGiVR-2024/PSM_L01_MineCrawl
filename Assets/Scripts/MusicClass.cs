using UnityEngine;

public class MusicClass : MonoBehaviour
{
    private static MusicClass instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // ju¿ istnieje inny, usuñ siebie
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void MuteMusic()
    {
        _audioSource.mute = !_audioSource.mute;
    }
}
