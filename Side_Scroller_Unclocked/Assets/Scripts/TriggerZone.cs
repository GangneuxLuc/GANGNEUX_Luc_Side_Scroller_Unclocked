using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
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

        onTriggerEnter?.Invoke();
        

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

    private int DetermineDirection(Collider2D collision)
    {
        Debug.Log("Direction: " + (collision.transform.position.x > transform.position.x ? "Right" : "Left"));
        return collision.transform.position.x > transform.position.x ? 1 : -1;
       
    }

}