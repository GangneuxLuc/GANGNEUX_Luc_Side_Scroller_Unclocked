using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameDirector gameDirector;

    private void Awake()
    {
        gameDirector = FindFirstObjectByType<GameDirector>(); // Trouve une instance de GameDirector dans la scène
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Checkpoint reached!");
            gameDirector.SavePosition(); // Appelle la méthode SavePosition du GameDirector pour sauvegarder la position du joueur
        }
    }
}

