using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource audioSource;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }
}