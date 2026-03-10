using UnityEngine;

public class ParallaxLayerSettings : MonoBehaviour
{
    [Tooltip("Facteur Horizontal (0= follow camera | 1 = static")]
    [Range(0f, 1f)] public float speedX = 0.5f;

    [Tooltip("Facteur Vertical (0= follow camera | 1 = static")]
    [Range(0f, 1f)] public float speedY = 0.5f;

}
