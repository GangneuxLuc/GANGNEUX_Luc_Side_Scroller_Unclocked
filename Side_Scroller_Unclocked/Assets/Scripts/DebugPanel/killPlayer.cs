using UnityEngine;

public class killPlayer : MonoBehaviour // Script pour tuer le joueur en un clic depuis le panneau de debug
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
