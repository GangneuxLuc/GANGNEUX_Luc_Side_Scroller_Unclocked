using System.Collections;
using UnityEngine;

public class ItemToGather : MonoBehaviour //Script pour collecter les items avec un feedback
{
    [Header("References")]
    [SerializeField] Canvas itemCanvas; // Canvas pour afficher les interactions possibles avec l'item
    [SerializeField] private string itemName; // Nom de l'item à collecter
    [SerializeField] private SpriteRenderer itemSprite; // Sprite de l'item à collecter

    [Header("Pickup Animation")]
    [SerializeField] float centerScale = 2.5f;
    [SerializeField] float centerDuration = 0.25f;
    [SerializeField] float returnDuration = 0.6f;
    [SerializeField] float waitBeforeCenter = 0.05f;

    ItemManager itemManager;

    private void Awake()
    {
        itemSprite = GetComponentInChildren<SpriteRenderer>(); // Récupère le SpriteRenderer attaché au GameObject
        itemCanvas = GetComponentInChildren<Canvas>(); // Récupère le Canvas attaché au GameObject ou à ses enfants
        if (itemCanvas != null) itemCanvas.gameObject.SetActive(false); // Désactive le canvas au début pour ne pas afficher les interactions possibles

        itemManager = FindFirstObjectByType<ItemManager>();
        if (itemManager == null)
        {
            Debug.LogWarning("ItemManager introuvable dans la scène. Assurez-vous qu'un GameObject avec ItemManager est présent.");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (itemCanvas != null && collision.CompareTag("Player")) itemCanvas.gameObject.SetActive(true); // Affiche le canvas lorsque le joueur est dans la zone de l'item
        if (collision.CompareTag("Player") && Input.GetButton("Interact"))
        {
            
            Debug.Log("Item collected: " + gameObject.name);

            GetComponent<Collider2D>().enabled = false; // Désactive le collider pour éviter les interactions supplémentaires
            if (itemCanvas != null) itemCanvas.gameObject.SetActive(false);

            StartCoroutine(CollectSequence(collision.transform)); // Démarre la coroutine d'animation et d'ajout à l'ItemManager
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
       if (itemCanvas != null) itemCanvas.gameObject.SetActive(false); // Désactive le canvas lorsque le joueur quitte la zone de l'item
    }

    private IEnumerator CollectSequence(Transform player)
    {
        // Attente pour éviter double-trigger
        yield return new WaitForSeconds(waitBeforeCenter);

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Camera.main introuvable. L'animation de collecte utilisera la position actuelle.");
        }

        // Sauvegardes d'état
        Vector3 originalPos = itemSprite.transform.position;
        Vector3 originalScale = itemSprite.transform.localScale;
        int originalSorting = itemSprite.sortingOrder;
        itemSprite.sortingOrder = originalSorting + 50; // Mettre devant la plupart des sprites

        // Calculde  la position du centre de l'écran en world space 
        Vector3 centerWorld = originalPos;
        if (cam != null)
        {
            float zDist = Mathf.Abs(originalPos.z - cam.transform.position.z);
            centerWorld = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, zDist));
            centerWorld.z = originalPos.z; // conserver la profondeur du sprite
        }

        // 1) Agrandir et aller au centre
        float t = 0f;
        while (t < centerDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / centerDuration);
            itemSprite.transform.position = Vector3.Lerp(originalPos, centerWorld, k);
            itemSprite.transform.localScale = Vector3.Lerp(originalScale, originalScale * centerScale, k);
            yield return null;
        }

        // Avant de repartir vers le joueur : ajouter à ItemManager 
        if (itemManager != null)
        {
            // Créer un placeholder GameObject (non visible) stocké dans le manager pour représenter l'item collecté
            GameObject stored = new GameObject(string.IsNullOrEmpty(itemName) ? gameObject.name : itemName);
            var sr = stored.AddComponent<SpriteRenderer>();
            if (itemSprite != null) sr.sprite = itemSprite.sprite;
            stored.SetActive(false); // le manager garde la référence sans l'afficher
            itemManager.CollectItem(stored);
        }
        else
        {
            Debug.LogWarning("Item non ajouté : ItemManager manquant.");
        }

        // 2) Se diriger vers le joueur en rapetissant
        Vector3 startPos = itemSprite.transform.position;
        Vector3 startScale = itemSprite.transform.localScale;
        t = 0f;
        while (t < returnDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / returnDuration);
            Vector3 targetPos = player != null ? player.position : originalPos;
            itemSprite.transform.position = Vector3.Lerp(startPos, targetPos, k);
            itemSprite.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, k);
            yield return null;
        }

        // Nettoyage
        Destroy(gameObject);
    }
}
