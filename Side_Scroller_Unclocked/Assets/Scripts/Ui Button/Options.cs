using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject optionsMenu; // Référence au menu des options
    bool isOptionsMenuActive; // Variable pour suivre l'état du menu des options
    public void ActivateOptions()
    {
        optionsMenu.SetActive(true); // Bascule l'état d'activation du menu des options
    }
    public void DeactivateOptions()
    {
        optionsMenu.SetActive(false); // Bascule l'état d'activation du menu des options
    }
}
