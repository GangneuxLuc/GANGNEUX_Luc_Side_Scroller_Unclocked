using System.Collections;
using UnityEngine;

public class PrepState : StateA
{
    [SerializeField] float waitChase;
    bool PlayerDetected = false;
    public ChaseState Chase;
    public override StateA RunCurrentState()
    {
        if (PlayerDetected) return Chase;
        else  return this;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Detected");
            StartCoroutine(Wait());
        }
    }

    
    IEnumerator Wait()
    {
        Debug.Log("Waiting for " + waitChase + " seconds before chasing the player.");
        yield return new WaitForSeconds(waitChase);
        PlayerDetected = true;
    }
   
}
