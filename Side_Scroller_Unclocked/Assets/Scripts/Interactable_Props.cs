using UnityEngine;

public class Interactable_Props : MonoBehaviour // Ce script gère les interactions avec les props dans le jeu, tels que les portes, les leviers, etc.
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            // Affiche une indication d'interaction (par exemple, un message ou une icône)

            // Ajoutez ici le code pour déclencher l'interaction spécifique avec le prop
            // Par exemple, ouvrir une porte, activer un mécanisme, etc.
            //Ouverture de porte
             if (gameObject.tag == "Door" && Input.GetButton("Interact"))
             {
                if (PlayerPrefs.HasKey("FirstKey")) // Vérifie si le joueur a la clé nécessaire pour ouvrir la porte
                {
                    OpenDoor();
                }
            }
        }
    }

    private void OpenDoor()
    {
        Collider2D doorCollider = GetComponent<Collider2D>();
        SpriteRenderer doorSprite = GetComponentInChildren<SpriteRenderer>();
        if (doorCollider != null)
        {
            doorCollider.enabled = false; // Désactive le collider de la porte pour permettre au joueur de passer à travers
        }
        if (doorSprite != null)
            {
                doorSprite.color = new Color(doorSprite.color.r, doorSprite.color.g, doorSprite.color.b, 0.5f); // Rend la porte semi-transparente pour indiquer qu'elle est ouverte
        }
        // Code pour jouer l'animation d'ouverture de la porte, désactiver les colliders, etc.
    }
}
