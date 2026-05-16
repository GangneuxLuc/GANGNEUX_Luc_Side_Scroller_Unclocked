using System.Collections;
using UnityEngine;

public class PatrolState : State // Classe représentant l'état de patrouille d'un ennemi, gérant les déplacements et la détection du joueur
{
    [Header("Patrol references")]
    public Transform t;
    public AttackState Attack;
    [SerializeField] bool playerDetected = false;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer animSprite, legSprite, torsoSprite;
    [SerializeField] private AnimationClip shootTransition;
    [SerializeField] private GameObject questionMark;
    [SerializeField] private GameObject exclamationMark;

    [Header("Patrol settings")]
    public Transform[] myPatrolTarget;
    [SerializeField] Vector3 direction;
    [Range(0, 5f)] public float waitDuration;
    [SerializeField] Rigidbody2D rb;
    bool onPatrol;
    bool onInvestigation;

    [Header("Player detection settings")]
    public float dst;

    [Header("Detection Settings")]
    [Range(-15f, 15f)] public float detectionDistance = 5f;
    public LayerMask detectionLayers;
    [Range(0, 30f)] public int rayCount = 5;
    [Range(0f, 180f)] public float spreadAngle = 45f;

    [Header("Investigation Settings")]
    [Range(-15f, 15f)] public float investigationDetectionDistance = 5f;
    public LayerMask investigationLayers;
    [Range(0, 30f)] public int investigationRayCount = 5;
    [Range(0f, 180f)] public float investigationSpreadAngle = 45f;
    Coroutine Investigation;
    [SerializeField] private float investigationDuration = 5f;

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
       
        onPatrol = true;
        // animSprite.enabled = true;
        anim.SetBool("Transition", false);
        anim.SetBool("StopShooting", false);
        // StartCoroutine(WaitForTransition());
        anim.SetBool("StopShooting", true);
        anim.SetBool("onPatrol", true);
        GetDirection();
       


    }
    IEnumerator WaitForTransition()
    {
        anim.SetBool("StopShooting", true);
        yield return new WaitForSeconds(shootTransition.length);
        Debug.Log("Transition finished");
        GetDirection();
        anim.SetBool("onPatrol", true);
    }
    private void OnDisable()
    {
        anim.SetBool("onPatrol", false);
        anim.SetBool("StopShooting", false);



    }
    public override State RunCurrentState()
    {
        if (PlayerInvestigation() && !playerDetected)
        {
            if (Investigation == null) Investigation = StartCoroutine(Investigate());
        }

        if (PlayerDetection() || playerDetected)
        {
            playerDetected = false;
            if (Investigation != null)
            {
                StopCoroutine(Investigation);
                Investigation = null;
            }
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
 private bool PlayerInvestigation(bool isPlayerInRange = false)
    {
       
        if (playerPos == null) return false;
        float startAngle = -investigationSpreadAngle / 2f;
        float angleStep = investigationRayCount > 1 ? investigationSpreadAngle / (investigationRayCount - 1) : 0f;

        for (int i = 0; i < Mathf.Max(1, investigationRayCount); i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 dir = RotateVector(transform.right, angle);
            if (IsFacingLeft) dir = -dir;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, investigationDetectionDistance, investigationLayers);
            if (showDebugRays) Debug.DrawRay(transform.position, dir * investigationDetectionDistance, hit.collider ? Color.red : Color.blue);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                isPlayerInRange = true;
                break;
            }
        }
        return isPlayerInRange;
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
        anim.SetBool("wait", pWait);
        if (!pWait && onPatrol)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
            

            if (direction.x < 0)
            {
                
                 SetFacing(-1);
                //animSprite.flipX = true;
                IsFacingLeft = true;
            }
            else
            {
                SetFacing(1);
                //animSprite.flipX = false;
                IsFacingLeft = false;
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

    IEnumerator Investigate()
    {
        Debug.Log("Player detected for investigation");
        onInvestigation = true;
        onPatrol = false;
        // Code pour faire enquêter l'ennemi vers la dernière position connue du joueur
        //faire apapraitre le ?
        //faire un fade in du ? pour indiquer que l'ennemi est en mode enquête

        SpriteRenderer qSprite = questionMark.GetComponent<SpriteRenderer>();

       // questionMark.transform.localScale =  // Ajuster la taille du point d'interrogation
       if (t.localScale.x > 0)
        {

            questionMark.transform.localScale = new Vector3(1.274f, 1.274f, 1.274f);
        }
        else
        {
            questionMark.transform.localScale = new Vector3(1.274f, 1.274f, 1.274f);
        }
        questionMark.SetActive(true);
        float duration = 0.5f; // Durée du fade-in en secondes
        Color c = qSprite.color;
        c.a = 0f;
        qSprite.color = c;
        float time = 0f;
        while (time < duration) 
        {
            time += Time.deltaTime;
            c.a = Mathf.Clamp01(time / duration);
            qSprite.color = c;
            yield return null;
        }
        c.a = 1f;
        qSprite.color = c;
        
        
        new WaitForSeconds(0.5f); // Attendre un moment avant de commencer à enquêter

         Vector2 lastKnownPosition = playerPos.position; // Obtenir la dernière position connue du joueur
        // Déplacer l'ennemi vers la dernière position connue du joueur
        rb.linearVelocity = Vector2.zero; // Arrêter le mouvement actuel
        Vector2 directionToLastKnown = (lastKnownPosition - (new Vector2(transform.position.x, transform.position.y))).normalized; // Calculer la direction vers la dernière position connue
        rb.linearVelocity.x = directionToLastKnown * speed; // Déplacer l'ennemi vers la dernière position connue


        yield return new WaitForSeconds(investigationDuration); // Attendre un moment avant de revenir à la patrouille
        questionMark.SetActive(false); // Faire disparaître le ? après l'enquête
        onInvestigation = false; // L'ennemi n'est plus en mode enquête
        GetDirection(); // Recalculer la direction vers la cible de patrouille
    }
}

