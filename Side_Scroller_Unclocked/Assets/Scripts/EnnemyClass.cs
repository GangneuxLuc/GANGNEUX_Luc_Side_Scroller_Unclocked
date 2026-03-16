using UnityEngine;
using System.Collections;

public class EnnemyClass : MonoBehaviour
{
    [Header("Statistiques de l'ennemi")]
    [SerializeField] public int HP;
    [SerializeField] protected string Name;
    [SerializeField] protected float speed;
    [SerializeField] protected int attackDmg;
    [SerializeField] protected float attackSpeed = 2f;
    [SerializeField] protected bool isAttacking = false;
    [Range(0, 20f)] public float range;

    [Header("Références")]
    public Transform activeTimeline;
    public GameObject player;
    protected Transform playerPos;
    public SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    
    protected float dst;
    public bool bDebugCanMove = true;
    protected Color originalColor;


    protected void Awake()
    {
       // activeTimeline = 
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        originalColor = spriteRenderer.color;
    }     
    

    protected void OnTriggerEnter2D(Collider2D collision) // Détection des collisions avec le joueur
    {
        if (collision.gameObject.CompareTag("DaggerSlice"))
        {
            //Debug.Log("Collision détectée entre l'ennemi et la DaggerSlice");
            HP -= player.GetComponent<PlayerController>().attackDamage; // Réduction des HP de l'ennemi

            StartCoroutine(Feedback());
            if (HP <= 0) // Mort de l'ennemi
            {
                Die();
                Debug.Log("L'ennemi est mort !");
            }
        }

        if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Feedback());
            Die();
            Debug.Log("Ennemi assassiné");
        }
    }
    protected IEnumerator Feedback() // Feedback visuel lorsque l'ennemi est touché
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
    }

    protected virtual void OnDisable()
    {
       // Debug.Log("Ennemi désactivé, arrêt de toutes les coroutines");
        StopAllCoroutines();
        return;
    }
    protected virtual void Die() // Mort de l'ennemi
    {
        Destroy(gameObject);
    }
}