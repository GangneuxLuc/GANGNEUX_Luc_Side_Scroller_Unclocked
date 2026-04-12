using UnityEngine;

public class GetToMenu : MonoBehaviour
{
    public string sceneName; // Nom de la scène du menu principal
    public void GoToMenu()
    {
        // Charger la scène du menu principal
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
