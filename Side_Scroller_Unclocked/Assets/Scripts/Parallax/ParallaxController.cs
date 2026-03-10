using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera cam;

    [Header("Parallax Settings")]
    public bool enableVertical = true;

    [Tooltip("Lissage du mouvement (0 = aucun // 1 = instant")]
    [Range(0f, 1f)]
    public float smoothing = 0.1f;

    public ParallaxLayer[] layers;
    private Vector3 previousCamPos;

    private void Awake()
    {   
        if (cam == null)  cam = Camera.main;
        previousCamPos = cam.transform.position;

        int count = transform.childCount;
        layers = new ParallaxLayer[count];


        for (int i = 0; i < count ; i++)
        {
            layers[i] = new ParallaxLayer(transform.GetChild(i));
        }
    }

    private void LateUpdate()
    {
        Vector3 camPos = cam.transform.position;
        Vector3 delta = camPos - previousCamPos;

        foreach (ParallaxLayer layer in layers)
        {
           layer.Move(delta, enableVertical, smoothing); // ca bug
        }
        previousCamPos = camPos;
    }

}
