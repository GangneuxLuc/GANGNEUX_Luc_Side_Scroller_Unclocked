using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour // Classe pour gérer les items à collecter dans le jeu
{
    [SerializeField] private CollectibleItem[] allItems;
    [SerializeField] private List<GameObject> itemList; // Liste pour stocker les objets ItemToGather
    private List<string> collectedItemIds = new List<string>();

    private void Awake()
    {
        // Assure que la liste existe et contient au moins 6 éléments
        if (itemList == null)
        {
            itemList = new List<GameObject>();
        }
        for (int i = itemList.Count; i < 6; i++)
        {
            itemList.Add(null);
        }

        // Récupère la chaîne sauvegardée et construit la liste d'IDs collectés (sans entrées vides)
        string saved = PlayerPrefs.GetString("CollectedItems", "");
        collectedItemIds = saved.Split(',')
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();

        var collected = new HashSet<string>(collectedItemIds);

        // Parcours tous les items connus et désactive ceux déjà collectés
        foreach (CollectibleItem item in allItems)
        {
            if (item == null) continue;
            if (collected.Contains(item.itemId))
            {
                AddItemToList(itemList, item.gameObject, item.itemId);
                item.gameObject.SetActive(false);
            }
        }
    }

    private void AddItemToList(List<GameObject> list, GameObject item, string itemID)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) // Vérifie si l'emplacement actuel de la liste est null
            {
                list[i] = item; // Ajoute l'item à la première position null trouvée

                if (!collectedItemIds.Contains(itemID))
                {
                    collectedItemIds.Add(itemID);
                    PlayerPrefs.SetString("CollectedItems", string.Join(",", collectedItemIds));
                    PlayerPrefs.Save(); // Sauvegarde les modifications dans PlayerPrefs
                }

                return; // Sort de la fonction après avoir ajouté l'item
            }
        }
        Debug.LogWarning("La liste est pleine. Impossible d'ajouter l'item: " + item.name); // Affiche un avertissement si la liste est pleine
    }

    public void CollectItem(GameObject item)
    {
        if (item == null) return;
        var collectible = item.GetComponent<CollectibleItem>();
        string id = collectible != null ? collectible.itemId : item.name;
        AddItemToList(itemList, item, id); // Appelle la fonction pour ajouter l'item à la liste
    }
}
