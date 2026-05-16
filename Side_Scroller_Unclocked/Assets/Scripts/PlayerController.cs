using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour // Script qui permet de contrôler l'avatar
{
    [Header("Movement")] //Variables relatives au mouvement
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float deadzone = 0.01f;
    int previousFacingDirection = 1;

    [Header("Jump")] //Variables qui me permettent de modifier le comportement du jump
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpShortForce = 2f;
    [Range(0f,1f)] public float jumpRange;
    bool wasGrounded = false;
    bool isGrounded = false;

    [Header("Stats")] //Statistiques du joueur en public pour être accessible par d'autres scripts (notamment GameDirector)
    [SerializeField] public int HP = 100;
    [SerializeField] public int maxHP = 100;
    [SerializeField] public int attackDamage = 10;
    [SerializeField] public int attackCooldown = 1;
    [SerializeField] public float attackKnockback = 5f;
    [SerializeField] public float speed;
    [SerializeField] public float speedMax = 5f;

    [Header("Sprite infos")] // Références relatives aux sprites
    [SerializeField] Color originalColor;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("External References")]
    GameObject GameDirector;
    Rigidbody2D rb;
    GameObject sliceSprite;
    float inputX;
    public bool inputSlice;
    public LayerMask groundLayer;
    [SerializeField] private Animator anim;
    private int facingDirection = 1; // 1 pour la droite, -1 pour la gauche
    public ParticleSystem landDustFX;
    public ParticleSystem sideDustFX;

    [Header("Debug")]
    public bool GodModeIsOn = false;
    public bool ignoreForce = false;
    private Vector2 force;
    public bool isHit = false;

    // état pour détection d'atterrissage / changement de direction
  

    private void Awake() // Dans l'Awake j'attribue les valeurs
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

        previousFacingDirection = transform.localScale.x > 0 ? 1 : -1;
        wasGrounded = false;
    }

   
    void Update() 
    {
        inputX = Input.GetAxisRaw("Horizontal");//inputX renvoie 1 ou -1 lorsque le joueur presse la direction droite ou gauche
        if (inputX > deadzone) inputX = 1;
        else if (inputX < -deadzone) inputX = -1;
        else inputX = 0;
        inputSlice = Input.GetButtonDown("Slice"); // Renvoie true quand le joueur appuie sur le bouton "Slice"

       
        bool jump = Input.GetButtonDown("Jump");  // Renvoie True quand le joueur appuie sur le bouton de saut
        bool jumpCancel = Input.GetButtonUp("Jump");

        bool isGrounded1 = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), Vector2.down,jumpRange, groundLayer);
        bool isGrounded2 = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), Vector2.down, jumpRange, groundLayer);
        isGrounded = isGrounded1 || isGrounded2;

        
        if (!wasGrounded && isGrounded) // Vérification qui permet de vérifier si le joueur atterit
        {
            landDustFX.Play(); // On lance l'effet de particule de poussière
        }
        wasGrounded = isGrounded;

        if (jump && isGrounded) // Si le joueur appuie sur le bouton de saut et qu'il est au sol, on applique une force de saut
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            float ySpeed = Mathf.Abs(rb.linearVelocity.y);
            anim.SetTrigger("JumpMontée");
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
        float yVelocity = (rb.linearVelocity.y); // Variable qui permet de passer de l'état de saut à la chute dans l'animation quand yVelocity < 0
        anim.SetFloat("yVel", yVelocity);
        anim.SetBool("isGrounded", isGrounded);

        if (inputSlice) StartCoroutine(SliceAttackCoroutine());
        
        if (GodModeIsOn) //Permet d'utiliser le godMode dans le Debug
        {
            HP = maxHP;
        }
    }

  
    private void FixedUpdate()   // Appel du Mouvement et Appel changement de direction
    {
        if (inputX > deadzone) { SetFacing(1); }
        else if (inputX < -deadzone) { SetFacing(-1); }

        facingDirection = transform.localScale.x > 0 ? 1 : -1; // On détermine la direction actuelle du joueur en fonction de son échelle locale

        // Détection de changement de direction (transition gauche <-> droite)
        if (facingDirection != previousFacingDirection)
        {
            // Le particle système se joue que si au sol
            if (isGrounded)
            {
                sideDustFX.Play();
            }
            previousFacingDirection = facingDirection;
        }

        Movement();
    }

  
    private void Movement()   //Mouvement vertical + animation
    {   
        if (!isHit)
        {
            var v = rb.linearVelocity;
            v.x = inputX * movementSpeed;
            rb.linearVelocity = new Vector2(v.x, rb.linearVelocity.y);
            float xValue = Mathf.Abs(rb.linearVelocity.x) > deadzone ? 1 : 0;
            anim.SetFloat("XValue", xValue);
        }
    }

    // On change l'orientation du joueur en fonction de la direction dans laquelle il se déplace
    private void SetFacing(int direction)
    {
        Vector2 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }


    private IEnumerator Feedback(bool bullet, bool slice) // Feedback visuel lorsque le joueur est touché
    { 
        if(bullet)
        {
            if (ignoreForce) yield break; // Si le God Mode est activé, on ne subit pas les effets du tir
            isHit = true;
            //durée du knockback + animation de hit
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.3f);

            isHit = false;
            

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
        yield return new WaitForSeconds(attackCooldown);
        inputSlice = false;
        sliceSprite.SetActive(false);
        // anim.SetBool("SliceAttack", inputSlice);
        yield break;
    }

    private void OnDrawGizmos() //Debug
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.down * jumpRange);
        Gizmos.DrawLine(transform.position + new Vector3(-0.4f, 0, 0), transform.position + new Vector3(-0.4f, 0, 0) + Vector3.down * jumpRange);
        Gizmos.DrawLine(transform.position + new Vector3(0.4f, 0, 0), transform.position + new Vector3(0.4f, 0, 0) + Vector3.down * jumpRange);
    }
}
