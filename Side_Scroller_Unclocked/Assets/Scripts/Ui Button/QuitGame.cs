using UnityEngine;

public class QuitGame : MonoBehaviour // Script pour le bouton de quitter le jeu, qui quitte l'application lorsque le bouton est cliqué
{
    public void Quit()
    {
        // Quitte l'application
        Application.Quit();
    }
}
