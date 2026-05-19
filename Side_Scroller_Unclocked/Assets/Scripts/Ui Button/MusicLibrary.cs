using UnityEngine;

[System.Serializable]
public struct MusicTrack // Un struct c'est une structure de données qui permet de regrouper plusieurs variables sous un même type.
                         // Ici, on utilise un struct pour regrouper le nom d'une piste musicale et son clip audio correspondant, ce qui facilite la gestion de la bibliothèque musicale dans l'inspecteur de Unity.
{
    public string trackName;
    public AudioClip clip;
}


public class MusicLibrary : MonoBehaviour // Script pour gérer la bibliothèque musicale et récupérer les clips audio en fonction du nom de la piste
{
    public MusicTrack[] tracks;

    public AudioClip GetClipFromName(string trackName)
    {
        foreach (var track in tracks) // pour chaque piste dans la bibliothèque musicale, on vérifie si le nom de la piste correspond au nom recherché, et si c'est le cas, on retourne le clip audio correspondant. Si aucune piste ne correspond, on retourne null.
        {
            if (track.trackName == trackName)
            {
                return track.clip;
            }
        }
        return null;
    }
}
