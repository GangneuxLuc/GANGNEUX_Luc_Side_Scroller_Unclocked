using System.Collections;
using UnityEngine;

public class borderTrigger : MonoBehaviour
{
    public bool playerInTrigger = false;
    [SerializeField] private GameDirector gameDirector; // référence au script GameDirector pour vérifier si le joueur est dans le trigger

    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            gameDirector.Respawn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
