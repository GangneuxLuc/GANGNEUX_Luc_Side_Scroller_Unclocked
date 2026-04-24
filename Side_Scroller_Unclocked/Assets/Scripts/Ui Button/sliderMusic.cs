using UnityEngine;
using UnityEngine.UI;

public class sliderMusic : MonoBehaviour
{
   float MusicVolume;
    private void Start()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
        GetComponent<Slider>().value = MusicVolume;
    }
     
}
