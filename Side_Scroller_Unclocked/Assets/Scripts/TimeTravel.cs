using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeTravel : MonoBehaviour
{
    [SerializeField] borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
    public Image image;
    

    // STEP 1
    [SerializeField] GameObject present, past;
    [SerializeField] bool pastIsVisible;
    [SerializeField] bool canTimeSwitch;
    public float switchCooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 
    private void Update()
    {
        TimelineSwitch();
        
    }

   IEnumerator SwitchCooldown()
    {
        canTimeSwitch = false;
        yield return new WaitForSeconds(switchCooldown);
        canTimeSwitch = true;
    }

    void TimelineSwitch()
    {
        if (canTimeSwitch)
        {
            if (Input.GetKeyDown(KeyCode.Q)) 
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
