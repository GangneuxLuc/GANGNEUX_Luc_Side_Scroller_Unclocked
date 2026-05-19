
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    // Retourne l'AudioSource utilisé pour que l'appelant puisse régler .loop, .volume, etc.
    public AudioSource PlayMusic(string trackName)
    {
        if (musicLibrary == null)
        {
            Debug.LogWarning("MusicManager: musicLibrary is null.");
            return null;
        }

        if (musicSource == null)
        {
            Debug.LogWarning("MusicManager: musicSource is null.");
            return null;
        }

        AudioClip clip = musicLibrary.GetClipFromName(trackName);
        if (clip == null)
        {
            Debug.LogWarning($"MusicManager: clip '{trackName}' not found.");
            return null;
        }

        musicSource.clip = clip;
        musicSource.Play();
        return musicSource;
    }
}