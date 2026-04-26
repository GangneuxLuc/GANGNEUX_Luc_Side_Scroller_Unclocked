using UnityEngine;

[DefaultExecutionOrder(0)] // Assure que ce script s'éxécute avant SettingsSave
public class TitleScreenMusicVolume : MonoBehaviour
{
    SettingsSave settingsSave;
    AudioSource audioSource;

    void Awake()
    {
        settingsSave = FindFirstObjectByType<SettingsSave>();
        audioSource = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else audioSource.volume = 1f;
    }


    void Update()
    {
        if (audioSource != null && settingsSave != null)
        {
           // audioSource.volume = settingsSave.MusicVolume;
        }
       
    }
}
