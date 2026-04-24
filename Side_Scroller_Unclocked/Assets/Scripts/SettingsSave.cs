using UnityEngine;
using UnityEngine.UI;

public class SettingsSave : MonoBehaviour
{
   public float MusicVolume;
   public float SFXVolume;
   Slider sliderMusic;


    private void Start()
    {

        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
       // SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);

        sliderMusic = FindFirstObjectByType<sliderMusic>().GetComponent<Slider>();

    }
    private void Update() // Pas opti
    {
        MusicVolume = sliderMusic.value;



        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }
}

