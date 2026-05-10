using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private GameObject player; 
    private PlayerController playerController;
    public Image RespawnFadeImage;
    public borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
    public GameObject debugPanel;
    public Transform spawn;

    [Header("Pause Management")]
    public bool isGamePaused = false; // Variable pour suivre l'état de pause du jeu
    public GameObject pauseMenuUI; // Référence à l'UI du menu de pause


    private void Awake()
    {
        Time.timeScale = 1f; // Assurez-vous que le temps du jeu est à l'échelle normale au démarrage
        RespawnFadeImage.enabled = true; // Assurez-vous que l'image est activ
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        player.transform.position = spawn.position;
        SavePosition();
            
        borderTrigger = FindAnyObjectByType<borderTrigger>(); // Trouve une instance de borderTrigger dans la scène
    }

    private void ShowDebugOptions()
    {
        if (debugPanel != null)
        {
            if (Input.GetKeyDown(KeyCode.F1)) // Appuyez sur la touche F1 pour afficher/masquer le panneau de débogage
            {
                debugPanel.SetActive(!debugPanel.activeSelf);
            }
        }
    }
 
    private void Start()
    {
        // Assurez-vous que l'image est active
        RespawnFadeImage.CrossFadeAlpha(0.0f, 1.0f, false); // Fade to 50% alpha over 1 second
        LoadPosition();
    }
 
    // Update is called once per frame
    void Update()
    {
        ShowDebugOptions();
        if (Input.GetKeyDown(KeyCode.R)) // Appuyez sur la touche R pour effacer la position sauvegardée et réinitialiser le checkpoint
        {
            ErasePosition();
        }

        if (Input.GetButtonDown("Pause")) // Appuyez sur la touche Échap pour basculer la pause du jeu
        {
            TogglePause();
        }

        
    }


    //Gérer la pause

    public void TogglePause()
    {
        isGamePaused = !isGamePaused; // Inverse l'état de pause
        Time.timeScale = isGamePaused ? 0f : 1f; // Met le temps à 0 pour pauser, ou à 1 pour reprendre
        pauseMenuUI.SetActive(isGamePaused); // Affiche ou masque le menu de pause
    }

    //Gérer les Checkpoints et le respawn du joueur
    public void Respawn() //Méthode qui gère le respawn de mon joueur au dernier checkpoint passé.
    {
        
            LoadPosition();
            RespawnFadeImage.CrossFadeAlpha(1.0f, 0.05f, false); // Fade to 100% alpha over 1 second
       
        if (playerController != null)
        {
            if (playerController.HP <= 0)
            {
                LoadPosition();
                playerController.HP = playerController.maxHP;
            }
            
        }
    }

    public void SavePosition()
    {         // Sauvegarder la position du joueur
        Vector3 playerPosition = player.transform.position;
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
        PlayerPrefs.Save();
    }
    public void LoadPosition()
    {         // Charger la position du joueur
        float x = PlayerPrefs.GetFloat("PlayerPosX", 0f);
        float y = PlayerPrefs.GetFloat("PlayerPosY", 0f);
        float z = PlayerPrefs.GetFloat("PlayerPosZ", 0f);
        player.transform.position = new Vector3(x, y, z);
    }

    void OnApplicationQuit() //Méthode pour effacer les données de PlayerPrefs lorsque l'application est quittée, afin d'éviter de charger une position obsolète lors du prochain lancement du jeu.
    {
        PlayerPrefs.DeleteAll();
    }

    public void ErasePosition()
    {
        PlayerPrefs.DeleteKey("PlayerPosX");
        PlayerPrefs.DeleteKey("PlayerPosY");
        PlayerPrefs.DeleteKey("PlayerPosZ");
    }

}
