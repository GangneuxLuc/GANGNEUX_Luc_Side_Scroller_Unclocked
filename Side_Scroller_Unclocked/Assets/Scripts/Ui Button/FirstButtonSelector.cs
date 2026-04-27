using UnityEngine;
using UnityEngine.EventSystems;


public class FirstButtonSelector : MonoBehaviour
{
    [Header("First Button")]
    [SerializeField] private GameObject firstButton;

    private void OnEnable()
    {
        
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
