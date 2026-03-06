using UnityEngine;

public class borderTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Si le joueur entre en collision avec ce trigger, on le téléporte à la position de départ
            other.transform.position = new Vector3(0f, 0f, 0f); // Remplacez (0f, 0f, 0f) par la position de départ souhaitée
        }
    }

}
