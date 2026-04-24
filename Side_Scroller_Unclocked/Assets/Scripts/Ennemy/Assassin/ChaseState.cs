using System.Collections;
using UnityEngine;

public class ChaseState : StateA
{
    private void Start()
    {
        Collider2D aCol = GetComponentInParent<Collider2D>();
        //Remettre le layer exclude en nothing

        aCol.excludeLayers = 0;
    }
    
    public override StateA RunCurrentState()
    {
        return this;
    }

    private void ChasePlayer()
    {         // Implémentez la logique de poursuite du joueur ici
        // Par exemple, vous pouvez utiliser NavMeshAgent pour suivre le joueur
    }


}
