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
    private float investigationTimer;
    [SerializeField] private float flipSearchTime = 4f;
    private Vector2 investigationTarget;

    [Header("Debug")]
    public bool showDebugRays = true;
    public bool IsFacingLeft;

    int curTarget = 0;
    public bool pWait;

    private void Start()
    {
        GetDirection();
        pWait = false;
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

    private void OnDisable()
    {
        anim.SetBool("onPatrol", false);
        anim.SetBool("StopShooting", false);
        questionMark.SetActive(false);



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

        // PATROUILLE
        if (!pWait && onPatrol)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

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

            if (Vector3.Distance(myPatrolTarget[curTarget].position, t.position) <= 0.5f)
            {
                curTarget++;

                pWait = true;

                if (curTarget >= myPatrolTarget.Length)
                    curTarget = 0;

                StartCoroutine(Wait());
            }
        }
        
        // INVESTIGATION
        if (onInvestigation)
        {
            Vector2 dir = (investigationTarget - (Vector2)transform.position).normalized;

            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);

            if (dir.x < 0)
            {
                SetFacing(-1);
                questionMark.transform.localScale = new Vector3(-1.274f, 1.274f, 1.274f);
                IsFacingLeft = true;
            }
            else
            {
                SetFacing(1);
                questionMark.transform.localScale = new Vector3(1.274f, 1.274f, 1.274f);
                IsFacingLeft = false;
            }

           
        }

    }

    IEnumerator Investigate()
    {
        // Timer pour limiter la durée de l'investigation, si le joueur n'est pas retrouvé au bout d'un moment, l'ennemi retourne à sa patrouille
        investigationTimer = investigationDuration;
        speed += 2f; // L'ennemi se déplace plus vite pendant l'investigation pour tenter de retrouver le joueur
        Debug.Log("Investigation started, timer set to " + investigationTimer);
        Debug.Log("Player detected for investigation");

        onInvestigation = true;
        onPatrol = false;
        anim.SetBool("onPatrol", true);


        SpriteRenderer qSprite = questionMark.GetComponent<SpriteRenderer>();
       

        questionMark.SetActive(true);

        float duration = 1f;

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

        // Attendre un peu
        yield return new WaitForSeconds(0.5f);

        // Sauvegarder la dernière position connue
        investigationTarget = playerPos.position;

        // Attendre jusqu'à ce que l'ennemi atteigne la position OU que le timer arrive à 0
        while (Vector2.Distance(transform.position, investigationTarget) > 3f && investigationTimer > 0f)
        {
          
            investigationTimer -= Time.deltaTime;
            Debug.Log("Investigating... Time left: " + investigationTimer);
            yield return null;
        }

        // Si le timer est écoulé => cleanup et retour à la patrouille
        if (investigationTimer <= 0f)
        {
            Debug.Log("Investigation timed out, returning to patrol");
            questionMark.SetActive(false);
            onInvestigation = false;
            onPatrol = true;
            speed -= 2f; // Remettre la vitesse normale
            GetDirection();
            Investigation = null;
            
            yield break;
        }

        // Attendre un peu et tourner dans les deux sens pour simuler la recherche du joueur
        float searchTime = 0f;
        while (searchTime < flipSearchTime && investigationTimer > 0f)
        {
            searchTime += Time.deltaTime;
            if (searchTime < flipSearchTime / 2f)
            {
                SetFacing(1);
            }
            else
            {
                SetFacing(-1);
            }

            // Baisser le timer pendant la recherche aussi
            investigationTimer -= Time.deltaTime;
            if (investigationTimer <= 0f)
            {
                Debug.Log("Investigation timed out during search, returning to patrol");
                break;
            }

            yield return null;
        }

        // Fin investigation 
        questionMark.SetActive(false);

        onInvestigation = false;
        onPatrol = true;

        GetDirection();

        Investigation = null;
    }
}

