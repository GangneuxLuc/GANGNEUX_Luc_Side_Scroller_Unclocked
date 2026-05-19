using UnityEngine;
using UnityEngine.UI;

public class sliderMusic : MonoBehaviour // Script pour le slider de volume de la musique, qui récupère la valeur du slider et la stocke dans PlayerPrefs pour être utilisée par le MusicManager
{
   float MusicVolume;
    private void Start()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
        GetComponent<Slider>().value = MusicVolume;
    }
     
}
