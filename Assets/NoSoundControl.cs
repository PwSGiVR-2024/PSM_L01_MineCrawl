using UnityEngine;
using UnityEngine.UI;

public class MusicMuteIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage; // referencja do komponentu Image ikony

    private void OnEnable()
    {
        MusicClass.OnVolumeMuteChanged += UpdateIcon;

        if (MusicClass.Instance != null)
        {
            AudioSource source = MusicClass.Instance.GetComponent<AudioSource>();
            if (source != null)
            {
                UpdateIcon(source.volume, source.mute);
            }
        }
    }

    private void OnDisable()
    {
        MusicClass.OnVolumeMuteChanged -= UpdateIcon;
    }

    private void UpdateIcon(float volume, bool isMuted)
    {
        if (iconImage != null)
        {
            iconImage.enabled = isMuted;  // W³¹cz/wy³¹cz komponent Image
            Debug.Log(isMuted);
        }
    }
}
