using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private GameObject player; 
    public Image RespawnFadeImage;
    public borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
                                        // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        RespawnFadeImage.enabled = true; // Assurez-vous que l'image est activ
        player = GameObject.FindGameObjectWithTag("Player");
        borderTrigger = FindAnyObjectByType<borderTrigger>(); // Trouve une instance de borderTrigger dans la scène
    }

    private void Start()
    {
        // Assurez-vous que l'image est active
        RespawnFadeImage.CrossFadeAlpha(0.0f, 1.0f, false); // Fade to 50% alpha over 1 second
    }
 
    // Update is called once per frame
    void Update()
    {
        Respawn();
    }
    void Respawn()
    {
        if (borderTrigger.playerInTrigger)
        {
            Debug.Log("Player is in trigger, fading in");
            RespawnFadeImage.CrossFadeAlpha(1.0f, 0.05f, false); // Fade to 100% alpha over 1 second
            player.transform.position = new Vector3(0f, 0f, 0f);
        }
        else
        {
            RespawnFadeImage.CrossFadeAlpha(0.0f, 0.2f, false); // Fade to 50% alpha over 1 second
        }
    }
}
