using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoldierScript : EnnemyClass
{

    [SerializeField] private GameObject bulletPrefab;

    [Header("Bullets infos")]
    public int bulletSpeed = 5;



    [Header("Shoot infos")]
    [Range(0f, 2f)] public float burstsCooldown;
    [Range(0f, 2f)] public float shootingSpeed;
    [Range(0f, 10f)] public int bulletPerBurst;
    [Range(0f, 5f)] public int burst;
    [Range(0f, 5f)] public int magazineNumber;
    


    [Header("Target References")]
    [SerializeField] Transform firePoint;

    Coroutine shootCoroutine;
    Coroutine patrolCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    private void FixedUpdate()
    {
        if (PlayerDetectionWithRaycast())
        {
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                rb.linearVelocity = Vector3.zero;
                patrolCoroutine = null;
            }
            if (shootCoroutine == null)
            {
                shootCoroutine = StartCoroutine(Shoot());
            }
        }
        else
        {
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
            if (patrolCoroutine == null)
            {
                patrolCoroutine = StartCoroutine(Patrol());
            }
        }
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
    }

    IEnumerator Patrol()
    {
        for (int i = 0; i < 10; i++)
        {
            SetFacing(1);
            rb.linearVelocity = new Vector2(1f, 0f);
            yield return new WaitForSeconds(2f);
            SetFacing(-1);
            rb.linearVelocity = new Vector2(-1f, 0f);
            yield return new WaitForSeconds(2f);
        }
    }

  /*  private bool PlayerDetection(bool isPlayerDetected = false)
    {
        dst = Vector2.Distance(transform.position, playerPos.position);
        Vector3 directionToTarget = playerPos.position - transform.position;
        float dot = Vector3.Dot( directionToTarget, transform.right);
       
        if (dst < range)
        {
            if (dot > 0)
            {
                SetFacing(1);
            }
            else if (dot < 0)
            {
                SetFacing(-1);
            }
            isPlayerDetected = true;

        }
        else
        {
            isPlayerDetected = false;
        }
        return isPlayerDetected;
    } */

    private bool PlayerDetectionWithRaycast(bool isPlayerDetected = false)
    {
        dst = Vector2.Distance(transform.position, playerPos.position);
        if (dst < range)
        {
            //Faire un raycast en cone pour detecter le joueur
            Vector3 directionToTarget = playerPos.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget.normalized, range);
            Debug.DrawRay(transform.position, directionToTarget.normalized * range, Color.red);
            float dot = Vector3.Dot(directionToTarget, transform.right);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                SetFacing(dot > 0 ? 1 : -1);
                isPlayerDetected = true;
            }
        }
        else
        {
            isPlayerDetected = false;
        }
        return isPlayerDetected;
    }
    private void SetFacing(int direction)
    {
        Vector2 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }

   /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    } */
}
