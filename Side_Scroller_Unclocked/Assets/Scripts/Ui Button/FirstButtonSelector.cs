using UnityEngine;
using UnityEngine.EventSystems;


public class FirstButtonSelector : MonoBehaviour // Script pour sélectionner automatiquement le premier bouton d'un menu lorsque celui-ci est activé pour permettre la navigation avec une manette ou le clavier
{
    [Header("First Button")]
    [SerializeField] private GameObject firstButton;

    private void Start()
    {
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    private void OnEnable()
    {
        
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
