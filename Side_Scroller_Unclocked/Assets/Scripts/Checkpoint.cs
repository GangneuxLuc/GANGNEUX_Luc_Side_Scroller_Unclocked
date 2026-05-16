using UnityEngine;

public class Checkpoint : MonoBehaviour // Script pour gérer les checkpoints dans le jeu. Lorsqu'un joueur entre en collision avec un checkpoint, sa position est sauvegardée dans le GameDirector pour pouvoir être réutilisée en cas de mort ou de retour à un point de sauvegarde.
{
    private GameDirector gameDirector;

    private void Awake()
    {
        gameDirector = FindFirstObjectByType<GameDirector>(); // Trouve une instance de GameDirector dans la scène
        if (gameDirector == null)
            Debug.LogWarning("[Checkpoint] GameDirector introuvable dans la scène.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Checkpoint reached!");
            if (gameDirector != null)
            {
 
                gameDirector.SavePosition(this.transform);
            }
            else
            {
                Debug.LogWarning("[Checkpoint] Impossible de sauvegarder: GameDirector est null.");
            }
        }
    }
   
}

