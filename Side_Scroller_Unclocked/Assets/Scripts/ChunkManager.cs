using System.Collections;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Chunks1755; // Prefab du chunk à instancier
    [SerializeField] private GameObject[] Chunks2055; // Prefab du chunk à instancier
    [SerializeField] private GameObject[] ChunksTriggers; // Triggers pour détecter la sortie du chunk

    // Si vous voulez filtrer par joueur, mettez ici le tag du joueur
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        foreach (GameObject triggerObj in ChunksTriggers)
        {
            if (triggerObj == null) continue;

            // S'assurer qu'il y a un Collider2D et qu'il est en mode trigger
            Collider2D col = triggerObj.GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                Debug.LogWarning($"ChunkManager: {triggerObj.name} n'a pas de Collider2D.");
            }

            // Ajouter ou récupérer le forwarder qui relaie les événements de trigger vers ce manager
            var forwarder = triggerObj.GetComponent<TriggerForwarder>();
            if (forwarder == null)
            {
                forwarder = triggerObj.AddComponent<TriggerForwarder>();
            }

            forwarder.Manager = this;
        }
    }

    // Méthode appelée par les TriggerForwarder quand un objet entre dans un trigger enfant
    public void OnChildTriggerEnter(GameObject triggerObj, Collider2D other)
    {
        Debug.Log($"ChunkManager: Trigger {triggerObj.name} détecté l'entrée de {other.name}.");
        if (other == null || triggerObj == null) return;

        // Exemple : ne réagir que si c'est le joueur qui entre
        if (!other.CompareTag(playerTag)) return;
        StartCoroutine(LoadNextChunk(Chunks1755));
        StartCoroutine(LoadNextChunk(Chunks2055));
        
    }

    // Optionnel : gestion de la sortie si nécessaire
    public void OnChildTriggerExit(GameObject triggerObj, Collider2D other)
    {
        // Implémenter si vous voulez une logique à la sortie
    }

    IEnumerator LoadNextChunk(GameObject[] chunkArray)
    {
        yield return new WaitForSeconds(1f); // Attendre un peu avant de charger le prochain chunk

        //Activer le Chunk suivant dans le tableau et désactiver le précédent 
        // Exemple minimal :
        if (chunkArray != null && chunkArray.Length > 0)
        {
            //Instantiate(chunkArray[0], transform.position, Quaternion.identity);
        }
    }
}

// Petit composant à ajouter aux objets triggers pour relayer les événements 2D vers ChunkManager
public class TriggerForwarder : MonoBehaviour
{
    public ChunkManager Manager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Manager?.OnChildTriggerEnter(gameObject, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Manager?.OnChildTriggerExit(gameObject, other);
    }
}
