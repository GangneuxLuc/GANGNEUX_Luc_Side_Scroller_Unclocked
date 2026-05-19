using UnityEngine;

public class ParallaxLayerSettings : MonoBehaviour
{
    [Tooltip("Facteur Horizontal (0= follow camera | 1 = static")]
    [Range(0f, 1f)] public float speedX = 0.5f;

    [Tooltip("Facteur Vertical (0= follow camera | 1 = static")]
    [Range(0f, 1f)] public float speedY = 0.5f;

    [Header("Scaling")]
    [Tooltip("Faire grossir/rapetisser la couche suivant le zoom de la caméra")]
    public bool scaleWithCamera = false;

    [Tooltip("Echelle minimale relative (multiplicateur)")]
    public float minScale = 0.5f;

    [Tooltip("Echelle maximale relative (multiplicateur)")]
    public float maxScale = 1.5f;

    [Tooltip("Taille orthographique de référence (valeur qui donne multiplicateur = 1). Si 0 => valeur actuelle de la caméra utilisée")]
    public float referenceOrthoSize = 0f;

    [Tooltip("Smoothing pour l'interpolation de l'échelle (0 = aucun, 1 = instantané)")]
    [Range(0f, 1f)] public float scaleSmoothing = 0.1f;
}
