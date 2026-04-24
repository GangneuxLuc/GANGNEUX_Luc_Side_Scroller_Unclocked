using UnityEngine;

public class TitleScreenMusicVolume : MonoBehaviour
{
    SettingsSave settingsSave;

    void Start()
    {
        settingsSave = FindFirstObjectByType<SettingsSave>();
    }

    void Update()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && settingsSave != null)
        {
            audioSource.volume = settingsSave.MusicVolume;
        }
    }
}
