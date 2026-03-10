using UnityEngine;

public class bullet : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 5f); // Détruire la balle après 5 secondes 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision détectée avec : " + collision.gameObject.name);
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Collision détectée entre la balle et le joueur");
            // Infliger des dégâts au joueur
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                Debug.Log("Infliger des dégâts au joueur");
                playerHealth.HP -= 10; // Réduire les HP du joueur de 10 (ajustez selon vos besoins)

            }
            Destroy(gameObject); // Détruire la balle après l'impact
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // Détruire la balle si elle touche un mur
        }
    }
}

