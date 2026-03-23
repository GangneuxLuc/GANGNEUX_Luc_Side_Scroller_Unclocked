using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class PatrolState : State
{
    [Header("Patrol references")]
    public Transform t;
    public AttackState Attack;
    [SerializeField] bool playerDetected = false;

    [Header("Patrol settings")]
    public Transform[] myPatrolTarget;
    [SerializeField] Vector3 direction;
    [Range(0, 5f)] public float waitDuration;
    // [Range(0,5f)] public float speed = 1.5f;

    [Header("Player detection settings")]
    public float dst;
    [Range(0, 10f)] public float range = 5f;

    int curTarget = 0;
    bool pWait;
  
    
    public override State RunCurrentState()
    {
        if (PlayerDetection())
        {
            Debug.Log("On run la state");
            playerDetected = false;
            return Attack;
        }
        else return this;
    } 
    private void Start()
    {
       
        GetDirection();
    }
    private bool PlayerDetection(bool isPlayerDetected = false)
    {
        dst = Vector2.Distance(transform.position, playerPos.position);
        Vector3 directionToTarget = playerPos.position - transform.position;
        float dot = Vector3.Dot(directionToTarget, transform.right);

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
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitDuration);
        pWait = false;
        GetDirection();
    }
    void GetDirection()
    {
        direction = Vector3.Normalize (myPatrolTarget[curTarget].position - t.position);
        Debug.Log(direction);
    }
   
    private void SetFacing(int direction)
    {
        Vector2 s = t.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        t.localScale = s;
    }

    void Update()
    {
        if (!pWait)
        {
            t.position += new Vector3(direction.x * speed * Time.deltaTime, 0);
            if (direction.x < 0) SetFacing(-1);
            else SetFacing(1);

            if (Vector3.Distance(myPatrolTarget[curTarget].position, t.position) <= 0.5)
            {

                curTarget++;
                pWait = true;

                if (curTarget >= myPatrolTarget.Length) curTarget = 0;
                StartCoroutine(Wait());
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

