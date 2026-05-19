using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 


public class AudioManager : MonoBehaviour //Script pour gérer les volumes de la musique et des effets sonores, ainsi que pour jouer la musique de fond en fonction de la scène active
                                          // Code récupéré grâce à un tuto (Raycastly)
{
    public AudioMixer audioMixer;
    public Slider mainSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public MusicManager musicManager;


  
    private void Start() // Charge les volumes sauvegardés et joue la musique de fond en fonction de la scène active
    {
        LoadVolume();
   
            musicManager.PlayMusic("MainMenu");
        if (SceneManager.GetActiveScene().name == "Level design test")
        {
            musicManager.PlayMusic("Level1").loop = true;
            
        }

    }

   
    public void UpdateMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
    }
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    
    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume() // Sauvegarde les volumes actuels dans les PlayerPrefs
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        audioMixer.GetFloat("MainVolume", out float mainVolume);
        PlayerPrefs.SetFloat("MainVolume", mainVolume);
    }

    public void LoadVolume() // Charge les volumes sauvegardés depuis les PlayerPrefs
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        mainSlider.value = PlayerPrefs.GetFloat("MainVolume");
    }
}