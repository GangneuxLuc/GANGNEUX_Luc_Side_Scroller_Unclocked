using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour // Script pour le bouton de démarrage du jeu, qui charge la scène du jeu lorsque le bouton est cliqué
{
    public string sceneName; // Nom de la scène à charger
    public void StartNewGame()
    {
        // Charge la scène du jeu
        SceneManager.LoadScene(sceneName);
    }
}
