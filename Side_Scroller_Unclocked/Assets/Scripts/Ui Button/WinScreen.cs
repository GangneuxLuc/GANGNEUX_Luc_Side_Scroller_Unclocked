using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreen : MonoBehaviour // Script pour le menu de victoire, qui affiche le nombre d'items collectés par le joueur en récupérant les données stockées dans PlayerPrefs
{
    [SerializeField] private TextMeshProUGUI collectibleText;

    private void OnEnable()
    {
        if (collectibleText == null)
            return;

        if (!PlayerPrefs.HasKey("CollectedItems"))
        {
            collectibleText.text = "0 item collectes";
            return;
        }
         
        string raw = PlayerPrefs.GetString("CollectedItems", string.Empty); //Récupère la chaîne des items collectés
        if (string.IsNullOrWhiteSpace(raw)) // Si la chaîne est vide ou ne contient que des espaces, on considère qu'aucun item n'a été collecté
        {
            collectibleText.text = "0 item collectes";
            return;
        }

      
        char[] separators = new[] { ',', ';', '|' }; // Séparateurs possibles pour les items collectés 
        var items = raw
            .Split(separators, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s));

        // Compter les éléments uniques (évite les doublons)
        int count = new HashSet<string>(items).Count;

        collectibleText.text = count == 1
            ? "1 items collecte"
            : $"{count} items collectes";
    }
}
