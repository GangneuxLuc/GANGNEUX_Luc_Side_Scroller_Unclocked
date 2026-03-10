using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SoldierScript : EnnemyClass
{

    [SerializeField] private GameObject bulletPrefab;

    [Header("Bullets infos")]
    public int bulletSpeed = 5;



    [Header("Shoot infos")]
    [Range(0f, 10f)] public int bulletPerBurst;
    [Range(0f, 5f)] public int burst;
    

    [Header("Target References")]
    [SerializeField] Transform firePoint;

    Coroutine shootCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    private void FixedUpdate()
    {
        if (PlayerDetection())
        {
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
        }
    }
    IEnumerator Shoot()
    {
        Rigidbody2D rb;
        for (int i = 0; i < burst; i++)
        {
            for (int j = 0; j < bulletPerBurst; j++)
            {
                
             
 
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, -90));
                bullet.transform.SetParent(transform, true);

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
                yield return new WaitForSeconds(AttackSpeed);
            }
            yield return new WaitForSeconds(AttackSpeed * 20);
        }
    }


    private bool PlayerDetection(bool isPlayerDetected = false)
    {
        dst = Vector2.Distance(transform.position, playerPos.position);
        if (dst < range)
        {
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }
        return isPlayerDetected;
    }

    
}
