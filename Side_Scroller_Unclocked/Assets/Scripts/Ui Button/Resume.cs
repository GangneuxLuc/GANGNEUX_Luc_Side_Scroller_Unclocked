using UnityEngine;

public class Resume : MonoBehaviour
{
   public GameObject pauseMenu; // Référence au menu de pause
    public void ResumeGame()
    {
        // Désactive le menu de pause
        pauseMenu.SetActive(false);
        // Réactive le temps du jeu
    }
}
