using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeTravel : MonoBehaviour // Script pour gérer le changement de timeline entre le présent et le passé
{
    [SerializeField] borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
    public Image image;

    [Header("Time Travel Management")]
    [SerializeField] GameObject present, past;
    [SerializeField] bool pastIsVisible;
    [SerializeField] bool canTimeSwitch;
    public float switchCooldown;

    [Header("Timeline Switch Cooldown Feedback")]
    [SerializeField] GameObject gaugeEmpty, gauge1, gauge2, gauge3;

    private void Update() //On appelle TimelineSwitch qui vérifie si le joueur déclenche le voyage dans le temps
    {
        TimelineSwitch();
    }
    void TimelineSwitch()
    {
        if (canTimeSwitch)
        {
            if (Input.GetButtonDown("TimelineSwap") && PlayerPrefs.HasKey("CollectedItems") && PlayerPrefs.GetString("CollectedItems").Contains("FixTimeSwap")) //Renvoie true si le boutton est pressé et si dans les PlayerPrefs, le string CollectedItems contiens FixTimeSwap
            {
                StartCoroutine(SwitchCooldown());
                pastIsVisible = !pastIsVisible;
                if (pastIsVisible)
                {
                    present.SetActive(false);
                    past.SetActive(true);
                }
                else
                {
                    present.SetActive(true);
                    past.SetActive(false);
                }
            }
        }
    }
    IEnumerator SwitchCooldown() //Cooldown sur le changement de timeline et remplissage progressive de la jauge pour feedback visuel et auditif
    {
        canTimeSwitch = false;
        float segmentDuration = Mathf.Max(0.01f, switchCooldown / 3f); // Cooldown divisé en 3 segments pour faire apparaître les différentes étapes de la jauge

        if (gauge3 != null) gauge3.SetActive(false);  // On laisse toujours la jauge vide visible


        if (gauge1 != null)
        {
            yield return StartCoroutine(FadeIn(gauge1, segmentDuration));
            gauge1.SetActive(false);
        }
        if (gauge2 != null)
        {
            yield return StartCoroutine(FadeIn(gauge2, segmentDuration));
            gauge2.SetActive(false);
        }
        if (gauge3 != null)
        {
            yield return StartCoroutine(FadeIn(gauge3, segmentDuration));
        }
        canTimeSwitch = true; //Une fois les 3 segments passés, on peut changer de timeline à nouveau
    }

  
   
    private IEnumerator FadeIn(GameObject jauge, float duration) // Coroutine pour faire un fade-in de l'opacité de la jauge jusqu'à 1.
    {
        if (jauge == null)
            yield break;


        jauge.SetActive(true);// Activer la jauge avant de modifier son opacité


        
        SpriteRenderer sr = jauge.GetComponent<SpriteRenderer>(); // On récupère le SpriteRenderer
        if (sr != null) 
        {
            Color c = sr.color; // Création d'une variable de couleur qui va stocker la couleur initiale du sprite
            c.a = 0f; // On met son alpha ( opcaité ) à 0
            sr.color = c; //Puis on réattribue la couleur du sprite à cette variable
            float t = 0f;
            while (t < duration) // Tant que la variable t est inférieure à la durée d'un segment, on l'augmente puis on fait augmenter l'alpha de c de t divisé par duration
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / duration);
                sr.color = c;
                yield return null;
            }
            c.a = 1f;
            sr.color = c; // On réattribue les valeurs et l'alpha à 1
            yield break;
        }
        yield return new WaitForSeconds(duration); 
    }
}
