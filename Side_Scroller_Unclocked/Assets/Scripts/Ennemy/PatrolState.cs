using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolState : State
{
    [Header("Patrol references")]
    public Transform t;
    public AttackState Attack;
    [SerializeField] bool playerDetected = false;

    [Header("Patrol settings")]
    public Transform[] myPatrolTarget;
    [SerializeField] Vector3 direction;
    [Range(0, 5f)] public float waitDuration;
    // [Range(0,5f)] public float speed = 1.5f;

    [Header("Player detection settings")]
    public float dst;

 
    [Header("Detection Settings")]
    [Range(-15f, 15f)] public float detectionDistance = 5f;       // How far the ray should check
    public LayerMask detectionLayers;          // Which layers to detect
    [Range(0, 30f)] public int rayCount = 5;                    // Number of rays for a "zone"
    [Range(0f, 180f)] public float spreadAngle = 45f;             // Spread of the detection zone in degrees
     

    [Header("Debug")]
    public bool showDebugRays = true;           // Show rays in Scene view
    public bool IsFacingLeft;

    int curTarget = 0;
    bool pWait;


    public override State RunCurrentState()
    {
         if (PlayerDetection())
         {
             Debug.Log("On lance la state");
             playerDetected = false;
             return Attack;
         }
         else return this;
        
    }
    private void Start()
    {
        GetDirection();
    }
   
    private bool PlayerDetection(bool isPlayerDetected= false) //Méthode pour détecter les objets dans une zone conique en utilisant plusieurs rayons
    {
        //dst = Vector2.Distance(transform.position, playerPos.position);
        Vector2 directionToTarget = playerPos.position - transform.position;
       // float dot = Vector2.Dot(directionToTarget,transform.right );
        float startAngle = -spreadAngle / 2f; // Angle de départ pour les rayons
        float angleStep = spreadAngle / (rayCount - 1);// Espacement entre les rayons
        
        for (int i = 0; i < rayCount; i++) // Boucle pour lancer plusieurs rayons
        {
            float angle = startAngle + (angleStep * i); // Calcul de l'angle pour le rayon actuel

          
            Vector2 direction = RotateVector(transform.right, angle); // Rotation du vecteur de direction de base (transform.right) pour obtenir la direction du rayon




            if (IsFacingLeft) direction = -direction;




            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, detectionLayers); // Lancement du rayon et stockage des informations de collision dans "hit"

            if (showDebugRays)
            {
                Color rayColor = hit.collider ? Color.red : Color.green;
                Debug.DrawRay(transform.position, direction * detectionDistance, rayColor);
            }

            if (hit.collider != null)
            {
                
            }
        }
        return isPlayerDetected;
    }
 

    private Vector2 RotateVector(Vector2 v, float degrees) // Méthode pour faire tourner un vecteur de direction d'un certain angle en degrés
    {
        float rad = degrees * Mathf.Deg2Rad; // Conversion de l'angle de degrés en radians
        float sin = Mathf.Sin(rad); // Calcul du sinus de l'angle 
        float cos = Mathf.Cos(rad); // Calcul du cosinus de l'angle
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y); // Application de la rotation au vecteur d'origine pour obtenir le nouveau vecteur de direction
    }
  
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitDuration);
        pWait = false;
        GetDirection();
    }
    void GetDirection()
    {
        direction = Vector3.Normalize(myPatrolTarget[curTarget].position - t.position);
    }

    private void SetFacing(int direction)
    {
        Vector2 s = t.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        t.localScale = s;
        
    }

    void Update()
    {
        if (!pWait)
        {
            t.position += new Vector3(direction.x * speed * Time.deltaTime, 0);
            if (direction.x < 0)
            {
                SetFacing(-1);
                IsFacingLeft = true;
            }
            else
            {
                SetFacing(1);
                IsFacingLeft = false;
            }
          

            if (Vector3.Distance(myPatrolTarget[curTarget].position, t.position) <= 0.5)
            {
                curTarget++;
                pWait = true;

                if (curTarget >= myPatrolTarget.Length) curTarget = 0;
                StartCoroutine(Wait());
            }
        }
    }
}

