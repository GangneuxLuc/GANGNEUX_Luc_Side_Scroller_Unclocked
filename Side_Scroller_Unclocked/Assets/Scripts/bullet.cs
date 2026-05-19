using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour // Script pour gérer les balles tirées par les ennemis, infligeant des dégâts au joueur et se détruisant après un certain temps ou à l'impact.
{
    Vector2 baseSpeed;
    Rigidbody2D rb;
    public float knockbackForce;
    public int damage = 10;
    private void Awake() // Dans l'Awake j'attribue les valeurs
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
                playerHealth.HP -= damage; // Réduire les HP du joueur de la valeur de damage

            }
            Destroy(gameObject); // Détruire la balle après l'impact
        }
        else 
        {
           Destroy(gameObject); // Détruire la balle si elle touche un autre objet
        }
    }
    void GetSpeedBack() // Fonction pour récupérer la vélocité de la balle avant qu'elle ne soit désactivée, afin de la réappliquer lorsqu'elle est réactivée
    {
        if (gameObject.activeSelf)
        {
            baseSpeed = rb.linearVelocity; // On récupère la vélocité pour la réappliquer lorsque l'objet est réactivé
        }
    }

    private void OnEnable() // Lorsque la balle est réactivée, on réapplique la vélocité pour qu'elle continue à se déplacer dans la même direction et à la même vitesse qu'avant d'être désactivée
    {
        rb.linearVelocity = baseSpeed;
    }
}

