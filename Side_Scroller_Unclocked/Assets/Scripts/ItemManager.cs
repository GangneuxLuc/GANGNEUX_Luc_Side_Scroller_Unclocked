using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemList; // Liste pour stocker les objets ItemToGather

    private void Awake()
    {
        for (int i = 0; i < 6; i++) // Ajoute six éléments null à la liste itemList car il n'y aura que 6 items à collecter
        {
            itemList.Add(null);
        }
    }


    static void AddItemToList(List<GameObject> list, GameObject item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) // Vérifie si l'emplacement actuel de la liste est null
            {
                list[i] = item; // Ajoute l'item à la première position null trouvée
                PlayerPrefs.SetString(item.name, item.name); // Sauvegarde le nom de l'item collecté dans PlayerPrefs pour la persistance
                PlayerPrefs.Save(); // Sauvegarde les modifications dans PlayerPrefs
                return; // Sort de la fonction après avoir ajouté l'item
            }
        }
        Debug.LogWarning("La liste est pleine. Impossible d'ajouter l'item: " + item.name); // Affiche un avertissement si la liste est pleine
    }
     public void CollectItem(GameObject item)
    {
        AddItemToList(itemList, item); // Appelle la fonction pour ajouter l'item à la liste
    }

}
