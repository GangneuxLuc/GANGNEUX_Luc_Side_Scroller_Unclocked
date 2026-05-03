using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AttackState : State
{
    public PatrolState Patrol;
    [Header("References")]
    [SerializeField] Transform firePoint;
    [SerializeField] Transform armPivot;
    public Transform t;
    [SerializeField] SpriteRenderer sprite;

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
    [Range(0f, 2f)] public float cooldown;

    public bool playerOutOfSight;
    Coroutine shootCoroutine;
    private bool isActivated;
    private Vector2 direction;

    public bool isShooting;
    private void OnEnable()
    {
        StartCoroutine(Cooldown());
        
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
    private void Update()
    {
        if (playerPos != null)
        {
            direction = (playerPos.position - firePoint.position).normalized;
            armPivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);
        }
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
        //Vector2 s = t.localScale;
        Vector2 s = sprite.transform.localScale;
        s.x = Mathf.Abs(s.x) * -direction;
       // t.localScale = s;
        sprite.transform.localScale = s;

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
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); // Instancie une abble depuis le firepoint avec une rotaiton de -90°

                    GameObject activeChild = null;
                    foreach (Transform child in activeTimeline)
                    {
                        if (child.gameObject.activeSelf)
                        {
                            activeChild = child.gameObject;
                            bullet.transform.SetParent(child, true);
                            break; // stop après avoir trouvé le premier enfant actif
                        }
                    }

                    // Calcul de la direction
                    if (playerPos != null)
                    {
                        // Tir dirigé vers le joueur
                        direction = (playerPos.position - firePoint.position).normalized;

                        // Orienter la rotation de la balle pour qu'elle pointe vers la cible (2D)
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    }
                    else
                    {
                        // Vers l'avant du FirePoint (utiliser right pour 2D)
                        direction = firePoint.right;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    }

                    // Ajoute d'une vitesse 
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

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        // Permet de relancer le tir après le cooldown
        // Réinitialiser les variables d'état
        playerOutOfSight = false;
        shootCoroutine = null;
        isActivated = true;
    }
}
