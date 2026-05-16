using System.Collections;
using UnityEngine;

public class PatrolState : State // Classe représentant l'état de patrouille d'un ennemi, gérant les déplacements et la détection du joueur
{
    [Header("Patrol references")]
    public Transform t;
    public AttackState Attack;
    [SerializeField] bool playerDetected = false;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer animSprite;

    [Header("Patrol settings")]
    public Transform[] myPatrolTarget;
    [SerializeField] Vector3 direction;
    [Range(0, 5f)] public float waitDuration;
    [SerializeField] Rigidbody2D rb;

    [Header("Player detection settings")]
    public float dst;

    [Header("Detection Settings")]
    [Range(-15f, 15f)] public float detectionDistance = 5f;
    public LayerMask detectionLayers;
    [Range(0, 30f)] public int rayCount = 5;
    [Range(0f, 180f)] public float spreadAngle = 45f;

    [Header("Debug")]
    public bool showDebugRays = true;
    public bool IsFacingLeft;

    int curTarget = 0;
    public bool pWait;

    private void Start()
    {
        GetDirection();
    }

    private void OnEnable()
    {
        Debug.Log("PatrolState");
        GetDirection();
        animSprite.enabled = true;
        Debug.Log("PatrolState");
        anim.SetBool("isShooting", false);
        anim.SetBool("onPatrol", true);

        

       
    }
    public override State RunCurrentState()
    {
        if (PlayerDetection() || playerDetected)
        {
            playerDetected = false;
            return Attack;
        }
        return this;
    }

    private bool PlayerDetection(bool isPlayerDetected = false)
    {
        if (playerPos == null) return false;
        float startAngle = -spreadAngle / 2f;
        float angleStep = rayCount > 1 ? spreadAngle / (rayCount - 1) : 0f;

        for (int i = 0; i < Mathf.Max(1, rayCount); i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 dir = RotateVector(transform.right, angle);
            if (IsFacingLeft) dir = -dir;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionDistance, detectionLayers);
            if (showDebugRays) Debug.DrawRay(transform.position, dir * detectionDistance, hit.collider ? Color.red : Color.green);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                isPlayerDetected = true;
                break;
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
    void GetDirection() // Méthode pour calculer la direction vers la cible de patrouille actuelle
    {
        direction = Vector3.Normalize(myPatrolTarget[curTarget].position - t.position);
    }

    private void SetFacing(int direction)// Méthode pour faire face à la direction de déplacement en ajustant l'échelle locale de l'objet
    {
        Vector2 s = t.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        t.localScale = s;
        
    }

    private void OnTriggerEnter2D(Collider2D collision) // Méthode pour détédcter la présence du joueur si il est trop près de l'ennemi
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player detected by trigger");
            playerDetected = true;
        }
    } 


    void Update()
    {
        if (!pWait)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        
            if (direction.x < 0)
            {
               // SetFacing(-1);
                animSprite.flipX = true;
                IsFacingLeft = true;
            }
            else
            {
                //SetFacing(1);
                animSprite.flipX = false;
                IsFacingLeft = false;
            }
            if (rb.linearVelocity == Vector2.zero)
            {
                anim.SetBool("wait", true);
            }
            else
            {
                anim.SetBool("wait", false);
            }

            if (Vector3.Distance(myPatrolTarget[curTarget].position, t.position) <= 0.5)
            {
                curTarget++;
                pWait = true;

                if (curTarget >= myPatrolTarget.Length) curTarget = 0;
                StartCoroutine(Wait());
            }
           //Si on sort de la zone de patrouille, on y retourne

        }
    }
}

