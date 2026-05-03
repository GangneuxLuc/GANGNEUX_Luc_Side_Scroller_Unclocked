using System.Collections;
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
        canTimeSwitch = false;
       
        gauge3.SetActive(false);
        gaugeEmpty.SetActive(true);
        yield return new WaitForSeconds(switchCooldown / 3);
        gaugeEmpty.SetActive(false);
        gauge1.SetActive(true);
        yield return new WaitForSeconds(switchCooldown / 3);
        gauge1.SetActive(false);
        gauge2.SetActive(true);
        yield return new WaitForSeconds(switchCooldown / 3);
        gauge2.SetActive(false);
        gauge3.SetActive(true);
       
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
}
