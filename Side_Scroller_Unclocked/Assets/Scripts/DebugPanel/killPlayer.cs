using UnityEngine;

public class killPlayer : MonoBehaviour
{
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void KillPlayer()
    {
        if (playerController != null)
        {
            playerController.HP = 0;
        }
    }
}
