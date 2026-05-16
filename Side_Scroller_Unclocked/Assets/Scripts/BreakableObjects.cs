using UnityEngine;

public class BreakableObjects : MonoBehaviour //Gère les objets destructibles
{
    [SerializeField] private int hp = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DaggerSlice"))
        {
            hp--;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
