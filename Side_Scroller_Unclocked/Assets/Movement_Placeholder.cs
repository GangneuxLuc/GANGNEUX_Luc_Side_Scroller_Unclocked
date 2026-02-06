using UnityEngine;

public class Movement_Placeholder : MonoBehaviour
{
    private float h;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float deadzone = 0.01f;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning($"Animator introuvable sur {gameObject.name}");
        }
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // Pour un jeu 2D vue de côté : on n'utilise que l'axe horizontal
        h = Input.GetAxisRaw("Horizontal");

        bool isWalking = Mathf.Abs(h) > deadzone;

        if (isWalking)
        {
            // Déplacement uniquement sur l'axe X (pas de diagonale)
            transform.Translate(Vector3.right * h * speed * Time.deltaTime, Space.World);
        }

        if (anim != null)
        {
            anim.SetBool("IsWalking", isWalking);
        }

        // Optionnel : retourner le sprite en fonction de la direction
        if (h > deadzone) SetFacing(1);
        else if (h < -deadzone) SetFacing(-1);
    }

    private void SetFacing(int direction)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }
}
