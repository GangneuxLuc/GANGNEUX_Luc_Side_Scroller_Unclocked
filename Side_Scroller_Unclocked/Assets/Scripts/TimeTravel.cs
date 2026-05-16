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
            if (Input.GetButtonDown("TimelineSwap") && PlayerPrefs.HasKey("FixTimeSwap"))
            {
                StartCoroutine(SwitchCooldown());
                pastIsVisible = !pastIsVisible;
                if (pastIsVisible)
                {
                    present.SetActive(false);
                    past.SetActive(true);
                }
                if (!pastIsVisible)
                {
                    present.SetActive(true);
                    past.SetActive(false);
                }
            }
        }
    }
    IEnumerator SwitchCooldown() //Cooldwon sur le changement de timeline et remplissage progressive de la jauge pour feedback visuel et auditif
    {
        canTimeSwitch = false;
        float segmentDuration = Mathf.Max(0.01f, switchCooldown / 3f); // On divise le cooldown en 3 segments pour faire apparaître les différentes étapes de la jauge
        if (gauge3 != null) gauge3.SetActive(false);
        // On laisse toujours la jauge vide visible

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
        canTimeSwitch = true; //Une fois les 3 segments passés, on réactive le changement de timeline
    }

  
    // Coroutine pour faire un fade-in de l'opacité de la jauge jusqu'à 1.
    private IEnumerator FadeIn(GameObject jauge, float duration)
    {
        if (jauge == null)
            yield break;

        // Activer l'objet avant de modifier l'alpha (sinon certains composants ne sont pas accessibles visuellement)
        jauge.SetActive(true);


        // Sinon SpriteRenderer
        SpriteRenderer sr = jauge.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / duration);
                sr.color = c;
                yield return null;
            }
            c.a = 1f;
            sr.color = c;
            yield break;
        }
        yield return new WaitForSeconds(duration);
    }
}
