using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
    public Image image;


    void Awake()
    {
        image = GetComponent<Image>();
        borderTrigger = FindAnyObjectByType<borderTrigger>(); // Trouve une instance de borderTrigger dans la scène
    }
    void Start()
    {
        image.CrossFadeAlpha(0.0f, 1.0f, false); // Fade to 50% alpha over 1 second
    }

    void Update()
    {
        BorderScreenFade();
    }

    void BorderScreenFade()
    {        
        if (borderTrigger.playerInTrigger)
        {
            image.CrossFadeAlpha(1.0f, 0.05f, false); // Fade to 100% alpha over 1 second
        }
        else
        {
            image.CrossFadeAlpha(0.0f, 0.2f, false); // Fade to 50% alpha over 1 second
        }
    }
}