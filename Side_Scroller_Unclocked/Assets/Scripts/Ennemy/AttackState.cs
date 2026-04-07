using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AttackState : State
{
    public PatrolState Patrol;
    [Header("References")]
    [SerializeField] Transform firePoint;
    public Transform t;

    [Header("Bullets infos")]
    public int bulletSpeed = 5;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shoot infos")]
    [Range(0f, 2f)] public float burstsCooldown;
    [Range(0f, 2f)] public float shootingSpeed;
    [Range(0f, 10f)] public int bulletPerBurst;
    [Range(0f, 5f)] public int burst;
    [Range(0f, 5f)] public int magazineNumber;

    [Header("Player Sighting")]
    [Range(0f, 10f)] public float sightRange;
    [Range(0f, 10f)] public float sightRadius;
    public float dst;



    [Header("Debug")]
    public bool showDebugRays = false;

    public bool playerOutOfSight;
    Coroutine shootCoroutine;
    private bool isActivated;

    private void OnEnable()
    {
        // Réinitialiser les variables d'état
        playerOutOfSight = false;
        shootCoroutine = null;
        isActivated = true;
    }
    public override State RunCurrentState()
    {
        // Ne démarre le coroutine qu'une seule fois tant qu'il tourne
        if (shootCoroutine == null && isActivated)
        {
            shootCoroutine = StartCoroutine(Shoot());
        }

        // Lorsque l'ennemi perd de vue le joueur, arrêter proprement le tir et revenir en patrol
        if (playerOutOfSight)
        {
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
            StopAllCoroutines();
            playerOutOfSight = false;
            return Patrol;
        }
        playerSighting();
        return this;
    }

    private void OnDisable()
    {
        // Nettoyage si l'état est désactivé
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
        StopAllCoroutines();
        isActivated = false;
    }
    private void SetFacing(int direction)// M?thode pour faire face ? la direction de d?placement en ajustant l'?chelle locale de l'objet
    {
        Vector2 s = t.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        t.localScale = s;

    }
    private void playerSighting()
    {
        dst = Vector2.Distance(transform.position, playerPos.position);
        //Savoir si le joueur est à gauche ou à droite de l'ennemi pour faire face à la bonne direction
            if (playerPos.position.x < transform.position.x) SetFacing(-1);
            else SetFacing(1);

        if (dst < sightRange) playerOutOfSight = false;
        else playerOutOfSight = true;
    }
    IEnumerator Shoot()
    {
        Rigidbody2D rb;
        for (int h = 0; h < magazineNumber; h++)
        {
            for (int i = 0; i < burst; i++)
            {
                for (int j = 0; j < bulletPerBurst; j++)
                {
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, -90));

                    GameObject activeChild = null;
                    foreach (Transform child in activeTimeline)
                    {
                        if (child.gameObject.activeSelf)
                        {
                            activeChild = child.gameObject;
                            bullet.transform.SetParent(child, true);
                            break; // stop aprèes avoir trouvé le premier enfant actif
                        }
                    }

                    // Calcul de la direction
                    Vector2 direction;
                    Vector2 rotation;
                    if (playerPos != null)
                    {
                        // Vers une cible
                        direction = (playerPos.position - firePoint.position).normalized;
                        rotation = new Vector2(direction.x, direction.y);
                        bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, rotation);
                    }
                    else
                    {
                        // Vers l'avant du FirePoint
                        direction = firePoint.forward;
                    }

                    // Ajouter une vitesse (si Rigidbody)
                    rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = direction * bulletSpeed;
                    }

                    yield return new WaitForSeconds(shootingSpeed);
                }
                yield return new WaitForSeconds(burstsCooldown);
            }
        }

        // Coroutine terminée : remettre la référence à null pour pouvoir relancer proprement si nécessaire
        shootCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
