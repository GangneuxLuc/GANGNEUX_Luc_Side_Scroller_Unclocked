using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Transform target;
    private float smoothTime = .2f;
    public Vector3 offset = new Vector3(0f,0f,-5f);
    public float distance = 5f;

    Vector3 Velocity = Vector3.zero;
    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        //GetComponent<Camera>().orthographicSize = distance;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref Velocity, smoothTime); // ref Velocity récupère l'adresse d'un objet , récupère à chaque frame la vélocité
        
    }
}
