using System.Collections;
using UnityEngine;

public class SoldierScript : EnnemyClass
{

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform activeTimeline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void newAwake()
    {
        
    }
    IEnumerator Shoot()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                bullet.transform.SetParent(activeTimeline);
                Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, -90));
                yield return new WaitForSeconds(AttackSpeed);
            }
            yield return new WaitForSeconds(AttackSpeed * 20);
        }
    }

    private void PlayerDetection()
    {


    }
}
