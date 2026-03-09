using UnityEngine;

public class bullet : MonoBehaviour
{
    Rigidbody2D rb;
    int speed = 25;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f); // Détruire la balle après 5 secondes pour éviter les fuites de mémoire
    }

    private void Update()
    {
        // Déplacer la balle vers la droite à une vitesse constante

        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y); // Ajustez la vitesse selon vos besoins
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

