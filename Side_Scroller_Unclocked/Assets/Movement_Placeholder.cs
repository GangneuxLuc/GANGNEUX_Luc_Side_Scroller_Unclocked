using Unity.VisualScripting;
using UnityEngine;

public class Movement_Placeholder : MonoBehaviour
{
    private float h;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float deadzone = 0.01f;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning($"Animator introuvable sur {gameObject.name}");
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // Pour un jeu 2D vue de c�t� : on n'utilise que l'axe horizontal
        h = Input.GetAxisRaw("Horizontal");

        bool isWalking = Mathf.Abs(h) > deadzone;

        if (isWalking)
        {
            // D�placement uniquement sur l'axe X (pas de diagonale)
            transform.Translate(Vector3.right * h * speed * Time.deltaTime, Space.World);
        }

        if (anim != null)
        {
            anim.SetBool("IsWalking", isWalking);
        }

        // Optionnel : retourner le sprite en fonction de la direction
        if (h > deadzone) SetFacing(1);
        else if (h < -deadzone) SetFacing(-1);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }



    private void SetFacing(int direction)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }
}
