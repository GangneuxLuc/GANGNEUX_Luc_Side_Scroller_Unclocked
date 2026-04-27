using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu; // Référence au menu des options
    [SerializeField] private GameObject titleScreen;

    bool isOptionsMenuActive; // Variable pour suivre l'état du menu des options
    public void ActivateOptions()
    {
        optionsMenu.SetActive(true); // Bascule l'état d'activation du menu des options
        titleScreen.SetActive(false);
    }
    public void DeactivateOptions()
    {
        optionsMenu.SetActive(false); // Bascule l'état d'activation du menu des options
        titleScreen.SetActive(true);
    }
}
