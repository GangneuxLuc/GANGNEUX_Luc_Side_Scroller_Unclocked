using System.Collections;
using UnityEngine;

public class AttackState : State // État d'attaque pour les soldats

//Régler arm Pivot 
// Problème : animation de transition qui ne se lance pas, ou qui se lance mais ne joue pas l'animation de tir ensuite
{
    public PatrolState Patrol;
    [Header("References")]
    [SerializeField] Transform firePoint;
    [SerializeField] Transform armPivot;
    public Transform t;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] private AnimationClip shootTransition;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer animSprite;

    [Header("Bullets infos")]
    public int bulletSpeed = 5;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shoot infos")]
    [Range(0f, 2f)] public float burstsCooldown;
    [Range(0f, 2f)] public float shootingSpeed;
    [Range(0f, 10f)] public int bulletPerBurst;
    [Range(0f, 5f)] public int burst;
    [Range(0f, 5f)] public int magazineNumber;

    [Header("Player Sighting")]// Variables pour la détection du joueur et le champ de vision de l'ennemi
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

        anim.SetBool("onPatrol", false);

        Debug.Log("AttackState");
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
        shootCoroutine = StartCoroutine(ShootTransition());
        StartCoroutine(Cooldown());
        
    }
    IEnumerator ShootTransition()

    {
        Debug.Log("ShootTransition");
        yield return new WaitForSeconds(shootTransition.length + 1f);
        animSprite.enabled = false;
        anim.SetBool("isShooting", true);

        

    }

    public override State RunCurrentState()
    {
        // Ne démarre le coroutine qu'une seule fois tant qu'il tourne
        if (shootCoroutine == null && isActivated)
        {
            shootCoroutine = StartCoroutine(Shoot());
        }

        // Lorsque l'ennemi perd de vue le joueur, arrête le tir et revenir en patrol
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
    private void SetFacing(int direction)// Méthode pour faire face à la direction de déplacement en ajustant l'échelle locale de l'objet
    {
        Vector2 s = t.localScale;
        s.x = Mathf.Abs(s.x) * -direction;
        t.localScale = s;
       

    }
    private void playerSighting()
    {
        dst = Vector2.Distance(transform.position, playerPos.position);
        //Savoir si le joueur est à gauche ou à droite de l'ennemi pour faire face à la bonne direction
            if (playerPos.position.x < transform.position.x)  animSprite.flipX = false;
            else animSprite.flipX = true;

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
                    // Instancier la balle sans parent, rotation neutre ; on gère rotation/vitesse avant de la parenter
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

                    // trouver le parent actif 
                    GameObject activeChild = null;
                    foreach (Transform child in activeTimeline)
                    {
                        if (child.gameObject.activeSelf)
                        {
                            activeChild = child.gameObject;
                            break; // stop après avoir trouvé le premier enfant actif
                        }
                    }

                    // Calcul de la direction
                    if (playerPos != null)
                    {
                        // Tir dirigé vers le joueur
                        direction = (playerPos.position - firePoint.position).normalized;
                    }
                    else
                    {
                        // Vers l'avant du FirePoint
                        direction = firePoint.right;
                    }

                    // Orienter la rotation de la balle pour qu'elle pointe vers la cible 
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg ;
                    bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                    // Ajout d'une vitesse 
                    rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = direction * bulletSpeed;
                    }

                    // Parent après avoir défini rotation et vitesse pour éviter inversion (scale négatif du parent)
                    if (activeChild != null)
                    {
                        bullet.transform.SetParent(activeChild.transform, true);
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
