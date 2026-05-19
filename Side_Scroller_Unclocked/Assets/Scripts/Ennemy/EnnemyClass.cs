using UnityEngine;
using System.Collections;
using UnityEditor;

public class EnnemyClass : MonoBehaviour // Classe de base pour les ennemis, g�re les statistiques, les collisions et la mort
{
    [Header("Statistiques de l'ennemi")]
    [SerializeField] public int HP;
    [SerializeField] protected string Name;

    [Header("R�f�rences")]
    public Transform activeTimeline;
    public GameObject player;
    private float knockbackForce;
    public Transform playerPos;
    public SpriteRenderer sprite, legSprite, torsoSprite;
    protected Rigidbody2D rb;
    public GameObject rewindEnergy;

    protected float dst;
    public bool bDebugCanMove = true;
    protected Color originalColor;


    protected void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        originalColor = sprite.color;
    }


    protected void OnTriggerEnter2D(Collider2D collision) // D�tection des collisions avec le joueur
    {
        if (collision.gameObject.CompareTag("DaggerSlice"))
        {
            //Debug.Log("Collision d�tect�e entre l'ennemi et la DaggerSlice");
            HP -= player.GetComponent<PlayerController>().attackDamage; // R�duction des HP de l'ennemi
            knockbackForce = player.GetComponent<PlayerController>().attackKnockback;

            StartCoroutine(Feedback());
            //rb.AddForce((transform.position - playerPos.position).normalized * 2f, ForceMode2D.Impulse); // Knockback de l'ennemi
            rb.AddForce(new Vector2(playerPos.localScale.x * knockbackForce, 0f), ForceMode2D.Impulse);

        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            //Debug.Log("Collision d�tect�e entre l'ennemi et la Bullet");
            HP -= collision.GetComponent<bullet>().damage; // R�duction des HP de l'ennemi
            knockbackForce = collision.GetComponent<bullet>().knockbackForce;
            StartCoroutine(Feedback());
            Vector2 direction = new Vector2((transform.position.x - collision.transform.position.x), (0)).normalized;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse); // Knockback de l'ennemi
        }

       
        /* if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
         {
             StartCoroutine(Feedback());
             Die();
             Debug.Log("Ennemi assassin�");
         }
        */
    }

  
    protected IEnumerator Feedback() // Feedback visuel lorsque l'ennemi est touch�
    {
        sprite.color = Color.red;
        legSprite.color = Color.red;
        torsoSprite.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sprite.color = originalColor;
        legSprite.color = originalColor;
        torsoSprite.color = originalColor;
    }

    protected virtual void OnDisable()
    {
        // Debug.Log("Ennemi d�sactiv�, arr�t de toutes les coroutines");
        StopAllCoroutines();
        return;
    }

    private void Update()
    {
        if (HP <= 0)
        {
            Die();
        }
    }
    protected virtual void Die() // Mort de l'ennemi
    {
        Instantiate(rewindEnergy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}