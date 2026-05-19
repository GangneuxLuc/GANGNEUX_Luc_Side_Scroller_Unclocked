using UnityEngine;

public class Checkpoint : MonoBehaviour // Script pour gérer les checkpoints dans le jeu. Lorsqu'un joueur entre en collision avec un checkpoint, sa position est sauvegardée dans le GameDirector pour pouvoir être réutilisée en cas de mort ou si on retourne au menu puis dans le jeu à nouveau
{
    private GameDirector gameDirector;
    private Animator animator;

    private void Awake()
    {
        gameDirector = FindFirstObjectByType<GameDirector>(); // Trouve une instance de GameDirector dans la scène
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Si l'objet avec lequel on rentre est trigger porte le tag Player alors on enregistre sa position dans le gameDirector et on lance l'animation
        {
            Debug.Log("Checkpoint reached!");
            if (gameDirector != null)
            {
                animator.SetBool("isActivated", true);
                gameDirector.SavePosition(this.transform);
            }
        }
    }
   
}

