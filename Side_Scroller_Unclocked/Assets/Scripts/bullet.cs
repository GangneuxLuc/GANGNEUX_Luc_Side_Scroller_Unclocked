using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour
{
    Vector2 baseSpeed;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f); // Détruire la balle après 5 secondes 
    }

    private void FixedUpdate()
    {
         GetSpeedBack();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            // Infliger des dégâts au joueur
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.HP -= 10; // Réduire les HP du joueur de 10 (ajustez selon vos besoins)

            }
            Destroy(gameObject); // Détruire la balle après l'impact
        }
        
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // Détruire la balle si elle touche un mur
        }
    }
    void GetSpeedBack()
    {
        if (gameObject.activeSelf)
        {
            baseSpeed = rb.linearVelocity; // On récupère la vélocité pour la réappliquer lorsque l'objet est réactivé
        }
    }

    private void OnEnable()
    {
        rb.linearVelocity = baseSpeed;
    }
}

