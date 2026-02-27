using UnityEngine;

public class TimeTravel : MonoBehaviour
{
    // STEP 1
    [SerializeField] GameObject present, past;
    [SerializeField] bool presentIsVisible = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        present.SetActive(presentIsVisible);
        past.SetActive(!presentIsVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchActiveLayers();
        }
    }

    void SwitchActiveLayers()
    {
        presentIsVisible = !presentIsVisible;
        present.SetActive(!present.activeSelf);
        past.SetActive(!past.activeSelf);
    }
}
