using System.Collections;
using UnityEngine;

public class borderTrigger : MonoBehaviour
{
    public bool playerInTrigger = false;
    bool canTp = false;
    Transform player;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) StartCoroutine(ScreenFade());
    }

    IEnumerator ScreenFade()
    {
        playerInTrigger = true;
        canTp = true;
        yield return new WaitForSeconds(0.2f);
        playerInTrigger = false;
        yield return new WaitForSeconds(0.2f);
    }
}
