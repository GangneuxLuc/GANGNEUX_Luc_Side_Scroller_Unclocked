using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)] // Assure que ce script s'éxécute après TitleScreenMusicVolume
public class SettingsSave : MonoBehaviour
{
   public static float MusicVolume;
   public static float SFXVolume;
   [SerializeField] private Slider sliderMusic;


    private void Start()
    {
       // MusicVolume = 1f;
        SFXVolume = 1f;
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
       // SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);

       // sliderMusic = FindFirstObjectByType<sliderMusic>().GetComponent<Slider>();

    }
    private void Update() // Pas opti
    {
        MusicVolume = sliderMusic.value;



        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }
}

