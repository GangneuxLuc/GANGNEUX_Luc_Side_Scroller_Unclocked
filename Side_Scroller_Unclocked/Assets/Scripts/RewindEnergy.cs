using UnityEngine;

public class RewindEnergy : MonoBehaviour //Script qui gère l'item à récupérer qui fait remonter l'énergie du rewind
{
    [SerializeField] private GameDirector gameDirector;
    private void Awake()
    {
        if (gameDirector == null)
        {
            gameDirector = FindFirstObjectByType<GameDirector>(); // On cherche le premier objet qui a la classe GameDirector
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Quand on rentrer en trigger avec le joueur, on augmente l'energie de rewind par 15
    {
        if (other.CompareTag("PLayer"))
        {
            gameDirector.RewindEnergy += 15;
            Destroy(gameObject);
        }
      
    }
}
