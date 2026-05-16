using UnityEngine;

public class PhysicsCrate : MonoBehaviour //Script à ajouter sur les caisses pour qu'elles soient affectées par l'attaque du joueur, en appliquant une force dans la direction opposée .
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DaggerSlice"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
                float forceMagnitude = 100f; // Adjust this value as needed
                rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
            }
        }





    }
}
