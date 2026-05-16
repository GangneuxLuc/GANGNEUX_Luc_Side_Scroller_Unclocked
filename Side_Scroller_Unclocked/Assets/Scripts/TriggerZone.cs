using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour // Code réutilisable pour des triggerZone qui sont trigger par le collisionTag. Permet aussi de passer par des events Unity pour plus d'ergonomies dans l'inspecteur
{


    public bool oneShot = false;
    private bool alreadyEntered = false;
    private bool alreadyExited = false;

    public string collisionTag;
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DetermineDirection(collision);
        if (alreadyEntered)
            return;

        if (!string.IsNullOrEmpty(collisionTag) && !collision.CompareTag(collisionTag))
            return;

        onTriggerEnter?.Invoke(); //Appelle le code depuis les Unity Events
        

        if (oneShot)
            alreadyEntered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (alreadyExited)
            return;

        if (!string.IsNullOrEmpty(collisionTag) && !collision.CompareTag(collisionTag))
            return;

        onTriggerExit?.Invoke();

        if (oneShot)
            alreadyExited = true;
    }

    private int DetermineDirection(Collider2D collision) // Méthode qui renvoie un int (1 ou -1) pour indiquer quel côté du trigger le joueur a rencontré
    {
        //Debug.Log("Direction: " + (collision.transform.position.x > transform.position.x ? "Right" : "Left"));
        return collision.transform.position.x > transform.position.x ? 1 : -1;
       
    }

}