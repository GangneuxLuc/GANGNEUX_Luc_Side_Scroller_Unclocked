using UnityEngine;

public class Resume : MonoBehaviour // Script pour le bouton de reprise du jeu, qui désactive le menu de pause et reprend le jeu lorsque le bouton est cliqué
{
   public GameObject pauseMenu; // Référence au menu de pause
    [SerializeField] private GameDirector GameDirector; // Référence au GameDirector
    public void ResumeGame()
    {
        GameDirector.TogglePause();
    }
}
