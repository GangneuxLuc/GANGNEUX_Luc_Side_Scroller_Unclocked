using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AttackState : State
{
    public PatrolState Patrol;
    [Header("Target References")]
    [SerializeField] Transform firePoint;

    [Header("Bullets infos")]
    public int bulletSpeed = 5;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shoot infos")]
    [Range(0f, 2f)] public float burstsCooldown;
    [Range(0f, 2f)] public float shootingSpeed;
    [Range(0f, 10f)] public int bulletPerBurst;
    [Range(0f, 5f)] public int burst;
    [Range(0f, 5f)] public int magazineNumber;

    public bool playerOutOfSight;
    Coroutine shootCoroutine;

    public override State RunCurrentState()
    {
        // Ne démarre le coroutine qu'une seule fois tant qu'il tourne
        if (shootCoroutine == null)
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
                Debug.Log("Cooldown between bursts");
                yield return new WaitForSeconds(burstsCooldown);
            }
            Debug.Log("On recommence les bursts");
        }

        // Coroutine terminée : remettre la référence à null pour pouvoir relancer proprement si nécessaire
        shootCoroutine = null;
    }
}
