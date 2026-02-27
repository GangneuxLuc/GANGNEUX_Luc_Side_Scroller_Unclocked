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

    [Header("External References")]
    GameObject GameDirector;
    Rigidbody2D rb;
    float inputX;
    public LayerMask groundLayer;
    private Animator anim;
    public string pastScene;
    public string presentScene;

    //[Header("Timeline Switch")]


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return;
        }
        anim = GetComponent<Animator>();

    }



    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    
    }

    private void FixedUpdate()
    {
        Movement();

        if (inputX > deadzone) SetFacing(1);
        else if (inputX < -deadzone) SetFacing(-1);
    }


    void Movement()
    {   //Mouvement vertical + animation
        var v = rb.linearVelocity;
        v.x = inputX * movementSpeed;
        rb.linearVelocity = v;
        bool isWalking = Mathf.Abs(inputX) > deadzone;
        anim.SetBool("IsWalking", isWalking);
    }


    private void SetFacing(int direction)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }


}
