using UnityEngine;

public class Resume : MonoBehaviour
{
   public GameObject pauseMenu; // Référence au menu de pause
    [SerializeField] private GameDirector GameDirector; // Référence au GameDirector
    public void ResumeGame()
    {
        GameDirector.TogglePause();
    }
}
