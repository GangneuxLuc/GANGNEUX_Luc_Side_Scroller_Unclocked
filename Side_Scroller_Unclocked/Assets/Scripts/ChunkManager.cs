using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChunkManager : MonoBehaviour // Script pour gérer le chargement et le déchargement des chunks quand on passe dans une triggerZone
{
    public GameObject[] Chunks1755; // Tableau de chunks à activer/désactiver
    public GameObject[] Chunks2055; 

    // Si vous voulez filtrer par joueur, mettez ici le tag du joueur
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private Image TransitionImage; // Image utilisée pour la transition de fade]

    private int currentChunkIndex; // Index du chunk actuel
    private int nextChunkIndex; // Index du prochain chunk 
    private int previousChunkIndex; // Index du chunk précédent

    private int nextNextChunkIndex; // Index du chunk après le prochain chunk
    private int previousPreviousChunkIndex; // Index du chunk avant le chunk précédent

    public void Start()
    {
        // currentCunkIndex est égal au premier élément du tableau de chunks, je veux donc écrire ça dans le code
        currentChunkIndex = 0;
        nextChunkIndex = 1;
        previousChunkIndex = -1;
        nextNextChunkIndex = 2;
        previousPreviousChunkIndex = -2;
    }

    /*  public void LoadNextChunk()
      {
          Debug.Log("LoadNextChunk called");
          if (previousChunkIndex >= 0)
          {  // Chunks1755[previousPreviousChunkIndex].SetActive(false); // Désactive le chunk précédent
              Chunks2055[previousPreviousChunkIndex].SetActive(false);
          }


          //Chunks1755[nextNextChunkIndex].SetActive(true); // Active le chunk suivant
          Chunks2055[nextNextChunkIndex].SetActive(true);

          previousPreviousChunkIndex = previousChunkIndex; // Met à jour l'index du chunk avant le chunk précédent
          previousChunkIndex = currentChunkIndex; // Met à jour l'index du chunk précédent
          currentChunkIndex = nextChunkIndex; // Met à jour l'index du chunk actuel
          nextChunkIndex = nextNextChunkIndex; // Met à jour l'index du prochain chunk
          nextNextChunkIndex++; // Incrémente l'index du chunk après le prochain chunk

      } */


    public void ChunkManagement()
    {
        //Selon la direction du joueur récupéré dans les triggerZones, on charge le chunk d'après ou précédent dans le tableau et on décharge le chunk d'avant ou d'après dans le tableau
    }
}
