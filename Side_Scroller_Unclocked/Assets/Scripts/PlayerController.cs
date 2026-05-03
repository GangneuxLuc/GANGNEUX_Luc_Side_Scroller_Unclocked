using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float deadzone = 0.01f;

    [Header("Gravity/jump")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpShortForce = 2f;
    [Range(0f,1f)] public float jumpRange;

    [Header("Stats")]
    [SerializeField] public int HP = 100;
    [SerializeField] public int maxHP = 100;
    [SerializeField] public int min = 0;
    [SerializeField] public int attackDamage = 10;
    [SerializeField] public int attackRange = 1;
    [SerializeField] public int attackCooldown = 1;
    [SerializeField] public float attackKnockback = 5f;

    [SerializeField] public float speed;
    [SerializeField] public float speedMax = 5f;

    [Header("Sprite infos")]
    [SerializeField] Color originalColor;


    [Header("External References")]
    GameObject GameDirector;
    Rigidbody2D rb;
    GameObject sliceSprite;
    float inputX;
    public bool inputSlice;
    public LayerMask groundLayer;
    [SerializeField] private Animator anim;
    private bool isFacingRight = true;
    private bool isFacingLeft = true;
    [SerializeField] private SpriteRenderer spriteRenderer;


    [Header("Debug")]
    public bool GodModeIsOn = false;
    public bool ignoreForce = false;
    private Vector2 force;
    public bool isHit = false;


    //[Header("Timeline Switch")]

    //On récupère les composants nécessaires et on s'assure que le joueur ne soit pas détruit lors du changement de scène dans l'Awake
    private void Start()
    {
        
    }
    private void Awake() 
    {
        originalColor = spriteRenderer.color;
       // DontDestroyOnLoad(gameObject);
        sliceSprite = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return;
        }
        anim = GetComponentInChildren<Animator>();

    }

    // On récupère les inputs du joueur et on vérifie s'il est au sol pour lui permettre de sauter
    void Update() 
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputSlice = Input.GetButtonDown("Slice");
        bool jump = Input.GetButtonDown("Jump");
        bool jumpCancel = Input.GetButtonUp("Jump");

        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down,jumpRange, groundLayer);
        if (jump && isGrounded) // Si le joueur appuie sur le bouton de saut et qu'il est au sol, on applique une force de saut
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jump = false;
        }
        if (jumpCancel && !isGrounded) // Si le joueur relâche le bouton de saut et qu'il n'est pas au sol, on réduit la force de saut pour permettre un saut plus court
        {
            if (rb.linearVelocity.y > jumpShortForce)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpShortForce);
                jumpCancel = false;
            }

        }
            



            if (inputSlice) StartCoroutine(SliceAttackCoroutine());
        
      if (GodModeIsOn)
        {
            HP = maxHP;
        }


        
        

    }

    // Appel du Mouvement et Appel changement de direction
    private void FixedUpdate() 
    {

        Movement();
       
        

        if (inputX > deadzone) SetFacing(1);
        else if (inputX < -deadzone) SetFacing(-1);

        
    }

    //Mouvement vertical + animation
    private void Movement() 
    {   
        if (!isHit)
        {
            var v = rb.linearVelocity;
            v.x = inputX * movementSpeed;
            rb.linearVelocity = new Vector2(v.x, rb.linearVelocity.y);
            //float isWalking = Mathf.Abs(inputX) > deadzone;
            float xValue = Mathf.Abs(rb.linearVelocity.x) > deadzone ? 1 : 0;
            // Animation : on considère le joueur en marche si la vitesse absolue dépasse le deadzone
            // on joue la vitesse de déplacement dans l'animation pour faire varier la vitesse de marche en fonction de la vitesse réelle du joueur
            anim.SetFloat("XValue", xValue);
            //mettre la vitesse de l'animation egale à xValue


        }

    }

    // On change l'orientation du joueur en fonction de la direction dans laquelle il se déplace
    private void SetFacing(int direction)
    {
        Vector2 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }

    private IEnumerator Feedback(bool bullet, bool slice) // Feedback visuel lorsque l'ennemi est touché
    { 
        if(bullet)
        {
            if (ignoreForce) yield break; // Si le God Mode est activé, on ne subit pas les effets du tir
            isHit = true;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.3f);//durée du knockback + animation de hit et ne pas pouvoir bouger pendant ce temps
            isHit = false;
            //knockback sur le côté  opposé à la direction du tir

        }
        if (slice)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            float knockbackForce = collision.GetComponent<bullet>().knockbackForce;
            StartCoroutine(Feedback(true, false));
            Vector2 direction = new Vector2((transform.position.x - collision.transform.position.x), (0)).normalized;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }

    }

    IEnumerator SliceAttackCoroutine()
    {
        
       // anim.SetBool("SliceAttack", inputSlice);
       sliceSprite.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        inputSlice = false;
        sliceSprite.SetActive(false);
        // anim.SetBool("SliceAttack", inputSlice);
        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * jumpRange);
    }
}
