using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoldierScript : EnnemyClass
{
    Coroutine shootCoroutine;
    Coroutine patrolCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    /* private bool PlayerDetectionWithRaycast(bool isPlayerDetected = false)
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
                // SetFacing(dot > 0 ? 1 : -1);
                 isPlayerDetected = true;
             }
         }
         else
         {
             isPlayerDetected = false;
         }
         return isPlayerDetected;
     }
    */
   
}

   
