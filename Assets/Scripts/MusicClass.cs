using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicClass : MonoBehaviour
{
    private static MusicClass instance;
    private AudioSource _audioSource;
    private const string VolumeKey = "MusicVolume";
    private const string MuteKey = "MusicMute";

    public static MusicClass Instance => instance;
    public static event Action<float, bool> OnVolumeMuteChanged;

    private Coroutine fadeCoroutine;
    [SerializeField] private AudioClip mainMenuClip;
    [SerializeField] private AudioClip battleClip;
    [SerializeField] private AudioClip explorationClip;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.ignoreListenerPause = true;

        _audioSource.volume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        _audioSource.mute = PlayerPrefs.GetInt(MuteKey, 0) == 1;

        OnVolumeMuteChanged?.Invoke(_audioSource.volume, _audioSource.mute);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void MuteMusic()
    {
        bool newMuteState = !_audioSource.mute;
        PlayerPrefs.SetInt(MuteKey, newMuteState ? 1 : 0);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        float startVol = _audioSource.volume;
        float targetVol = newMuteState ? 0f : PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        OnVolumeMuteChanged?.Invoke(targetVol, newMuteState);


        fadeCoroutine = StartCoroutine(FadeVolume(startVol, targetVol, 1f, () =>
        {
            _audioSource.mute = newMuteState;
        }));
    }






    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(VolumeKey, volume);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (_audioSource.mute && volume > 0)
        {
            _audioSource.mute = false;
            PlayerPrefs.SetInt(MuteKey, 0);
        }

        fadeCoroutine = StartCoroutine(FadeVolume(_audioSource.volume, volume, 0.5f));
        OnVolumeMuteChanged?.Invoke(volume, _audioSource.mute);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnVolumeMuteChanged?.Invoke(_audioSource.volume, _audioSource.mute);
        SwitchMusicForScene(scene.name);
    }
    private void SwitchMusicForScene(string sceneName)
    {
        AudioClip selectedClip = null;

        switch (sceneName)
        {
            case "Main menu":
                selectedClip = mainMenuClip;
                break;
            case "BattleScene":
                selectedClip = battleClip;
                break;
            case "tileMapTests":
                selectedClip = explorationClip;
                break;
            default:
                return; // nie zmieniaj muzyki
        }

        if (selectedClip != null && _audioSource.clip != selectedClip)
        {
            StartCoroutine(SwitchMusicWithFade(selectedClip, 1f));
        }
    }
    private IEnumerator SwitchMusicWithFade(AudioClip newClip, float fadeDuration)
    {
        float originalVolume = _audioSource.volume;

        // Fade out
        yield return FadeVolume(originalVolume, 0f, fadeDuration / 2f);

        _audioSource.clip = newClip;
        _audioSource.Play();

        // Fade in
        yield return FadeVolume(0f, originalVolume, fadeDuration / 2f);
    }

    private IEnumerator FadeVolume(float from, float to, float duration, Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _audioSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        _audioSource.volume = to;
        onComplete?.Invoke();
    }
}
