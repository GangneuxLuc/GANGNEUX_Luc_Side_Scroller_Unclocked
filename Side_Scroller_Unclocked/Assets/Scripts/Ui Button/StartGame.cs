using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour
{
    public string sceneName; // Nom de la scène à charger
    public void StartNewGame()
    {
        // Charge la scène du jeu
        SceneManager.LoadScene(sceneName);
    }
}
