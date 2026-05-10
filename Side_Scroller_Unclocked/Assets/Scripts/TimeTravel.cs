using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeTravel : MonoBehaviour // Script pour gérer le changement de timeline entre le présent et le passé
{
    [SerializeField] borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
    public Image image;
    

    // STEP 1
    [SerializeField] GameObject present, past;
    [SerializeField] bool pastIsVisible;
    [SerializeField] bool canTimeSwitch;

    [Header("Timeline Switch Cooldown Feedback")]
    [SerializeField] GameObject gaugeEmpty, gauge1, gauge2, gauge3;

    public float switchCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        TimelineSwitch();
    }

    IEnumerator SwitchCooldown()
    {
        // Bloquer le switch pendant la durée du cooldown
        canTimeSwitch = false;

        // durée par segment (3 étapes)
        float segmentDuration = Mathf.Max(0.01f, switchCooldown / 3f);

        // S'assurer que gauge3 est désactivée au départ
        if (gauge3 != null) gauge3.SetActive(false);

        // Etape 1 : gaugeEmpty -> fade in
      

        // Etape 2 : gauge1 -> fade in
        if (gauge1 != null)
        {
            yield return StartCoroutine(FadeIn(gauge1, segmentDuration));
            gauge1.SetActive(false);
        }

        // Etape 3 : gauge2 -> fade in
        if (gauge2 != null)
        {
            yield return StartCoroutine(FadeIn(gauge2, segmentDuration));
            gauge2.SetActive(false);
        }

        // Etape finale : gauge3 -> fade in et reste visible
        if (gauge3 != null)
        {
            yield return StartCoroutine(FadeIn(gauge3, segmentDuration));
        }

        // Autoriser à nouveau le switch
        canTimeSwitch = true;
    }

    void TimelineSwitch()
    {
        if (canTimeSwitch )
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

    // Coroutine générique pour faire un fade-in de l'opacité jusqu'à 1.
    // Supporte CanvasGroup, Image (UI) et SpriteRenderer.
    private IEnumerator FadeIn(GameObject go, float duration)
    {
        if (go == null)
            yield break;

        // Activer l'objet avant de modifier l'alpha (sinon certains composants ne sont pas accessibles visuellement)
        go.SetActive(true);


        // Sinon SpriteRenderer
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
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

        // Si aucun composant d'opacité trouvé, on se contente d'activer et d'attendre la durée
        yield return new WaitForSeconds(duration);
    }
}
