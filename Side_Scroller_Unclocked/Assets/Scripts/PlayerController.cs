using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deadzone = 0.01f;


    [Header("Gravity/jump")]
    [SerializeField] float gravity = -10f;
    [SerializeField] float jumpForce = 5f;

    [Header("Stats")]
    [SerializeField] public int HP = 100;
    [SerializeField] public int maxHP = 100;
    [SerializeField] public int min = 0;
    [SerializeField] public int attackDamage = 10;


    [Header("External References")]
    GameObject GameDirector;
    Rigidbody2D rb;
    GameObject sliceSprite;
    float inputX;
    public bool inputSlice;
    public LayerMask groundLayer;
    private Animator anim;
    private bool isFacingRight = true;
    private bool isFacingLeft = true;
    [SerializeField] private SpriteRenderer spriteRenderer;

    //[Header("Timeline Switch")]

    //On récupère les composants nécessaires et on s'assure que le joueur ne soit pas détruit lors du changement de scène dans l'Awake
    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        sliceSprite = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return;
        }
        anim = GetComponent<Animator>();

    }

    // On récupère les inputs du joueur et on vérifie s'il est au sol pour lui permettre de sauter
    void Update() 
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputSlice = Input.GetKeyDown(KeyCode.E);
        

        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        if (Input.GetButton("Jump") && isGrounded) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);


        if (inputSlice) StartCoroutine(SliceAttackCoroutine());
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
        var v = rb.linearVelocity;
        v.x = inputX * movementSpeed;
        rb.linearVelocity = v;
        bool isWalking = Mathf.Abs(inputX) > deadzone;
        anim.SetBool("IsWalking", isWalking);
    }

    // On change l'orientation du joueur en fonction de la direction dans laquelle il se déplace
    private void SetFacing(int direction)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }

    IEnumerator SliceAttackCoroutine()
    {
        
        Debug.Log("Slice Attack");
       // anim.SetBool("SliceAttack", inputSlice);
       sliceSprite.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        inputSlice = false;
        sliceSprite.SetActive(false);
        // anim.SetBool("SliceAttack", inputSlice);
        yield break;
    }
}
