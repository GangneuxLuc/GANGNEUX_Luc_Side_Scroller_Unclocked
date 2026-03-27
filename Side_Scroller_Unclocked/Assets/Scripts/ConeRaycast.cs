using UnityEngine;

public class DetectionZone2D : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionDistance = 5f;       // How far the ray should check
    public LayerMask detectionLayers;          // Which layers to detect
    public int rayCount = 5;                    // Number of rays for a "zone"
    public float spreadAngle = 45f;             // Spread of the detection zone in degrees

    [Header("Debug")]
    public bool showDebugRays = true;           // Show rays in Scene view

    void Update()
    {
        DetectObjects();
    }

   
    private void DetectObjects() //Méthode pour détecter les objets dans une zone conique en utilisant plusieurs rayons
    {
        float startAngle = -spreadAngle / 2f; // Angle de départ pour les rayons
        float angleStep = spreadAngle / (rayCount - 1);// Espacement entre les rayons

        for (int i = 0; i < rayCount; i++) // Boucle pour lancer plusieurs rayons
        {
            float angle = startAngle + (angleStep * i); // Calcul de l'angle pour le rayon actuel
            Vector2 direction = RotateVector(transform.right, angle); // Rotation du vecteur de direction de base (transform.right) pour obtenir la direction du rayon

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, detectionLayers); // Lancement du rayon et stockage des informations de collision dans "hit"

            if (showDebugRays)
            {
                Color rayColor = hit.collider ? Color.red : Color.green;
                Debug.DrawRay(transform.position, direction * detectionDistance, rayColor);
            }

            if (hit.collider != null)
            {
                Debug.Log($"Detected: {hit.collider.name} at distance {hit.distance}");
            }
        }
    }

    private Vector2 RotateVector(Vector2 v, float degrees) // Méthode pour faire tourner un vecteur de direction d'un certain angle en degrés
    {
        float rad = degrees * Mathf.Deg2Rad; // Conversion de l'angle de degrés en radians
        float sin = Mathf.Sin(rad); // Calcul du sinus de l'angle 
        float cos = Mathf.Cos(rad); // Calcul du cosinus de l'angle
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y); // Application de la rotation au vecteur d'origine pour obtenir le nouveau vecteur de direction
    }
}
