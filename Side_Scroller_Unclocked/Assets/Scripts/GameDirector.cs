using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameDirector : MonoBehaviour // Script pour gérer les éléments globaux du jeu comme le respawn du joueur et la gestion des UI
{
    [Header("Player Management")]
    [SerializeField] private GameObject player;
    private PlayerController playerController;
    private Rigidbody2D playerRb;
    public Image RespawnFadeImage;
    public borderTrigger borderTrigger; // référence au script borderTrigger pour vérifier si le joueur est dans le trigger
    public GameObject debugPanel;
    public Transform spawn;
    public int RewindEnergy = 100; // Variable pour stocker l'énergie de rewind du joueur
    public Animator playerAnim; // Référence à l'Animator du joueur pour déclencher les animations de dégâts

    [Header("UI Management")]
    public bool isGamePaused = false; // Variable pour suivre l'état de pause du jeu
    public GameObject pauseMenuUI; // Référence à l'UI du menu de pause
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject rewindEnergyUI; // Référence à l'UI de rewind energy
    [SerializeField] private GameObject ItemUI;
    [SerializeField] private GameObject HPUI;

    [Header("Slide settings (shared)")]
    [SerializeField] private float uiSlideDuration = 0.5f; // durée du slide
    [SerializeField] private float uiVisibleDuration = 1.5f; // durée visible avant de ressortir (pour temporaires)
    [SerializeField] private Vector2 rewindSlideOffset = new Vector2(-1000f, 0f); // offset pour rewind 
    [SerializeField] private Vector2 hpSlideOffset = new Vector2(-500f, 0f); // offset pour HP 
    [SerializeField] private Vector2 itemSlideOffset = new Vector2(0f, 200f); // offset pour Item 

    [Header("Idle detection")]
    [SerializeField] private float idleVelocityThreshold = 0.1f; // vitesse en dessous de laquelle on considère le joueur à l'arrêt
    [SerializeField] private float idleTimeToShow = 1.0f; // temps d'inactivité avant d'afficher les UI
    private float idleTimer = 0f;
    private bool idleUIsShown = false;

    private Transform respawnPoint; // Variable pour stocker la référence au dernier checkpoint passé par le joueur


    private Coroutine rewindSlideCoroutine;
    private Coroutine itemSlideCoroutine;
    private Coroutine hpSlideCoroutine;

    // Positions cibles des UI
    private Vector2 rewindTargetPos;
    private Vector2 itemTargetPos;
    private Vector2 hpTargetPos;
    private bool isHPUIAnimating = false;
    private float hpUIHideTimer = 0f;

    private int previousHP = -1;

    private Slider hpSlider; 

    private void Awake() // Dans l'Awake j'attribue les valeurs
    {
        Time.timeScale = 1f; // Assure que le temps est normal au lancement du jeu
        if (RespawnFadeImage != null)
            RespawnFadeImage.enabled = true;

       hpSlider = HPUI.GetComponent<Slider>();
        if (gameOverUI != null) gameOverUI.SetActive(false); // Assure que le menu de game over est désactivé au lancement du jeu
        if (winUI != null) winUI.SetActive(false); // Assure que le menu de victoire est désactivé au lancement du jeu

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY"))
        {
            LoadPosition(); // Charge la position sauvegardée du joueur si elle existe
        }
        else if (spawn != null) player.transform.position = spawn.position; // Place le joueur à la position de spawn au début du jeu



        if (spawn != null)
        {
            respawnPoint = spawn;
            SavePosition(spawn); // Sauvegarde la position de spawn comme point de respawn initial
        }

        borderTrigger = FindAnyObjectByType<borderTrigger>(); // Trouve une instance de borderTrigger dans la scène

        // Capturer positions cibles des UI et désactiver 
        CaptureAndDisableUI(rewindEnergyUI, ref rewindTargetPos);
        CaptureAndDisableUI(ItemUI, ref itemTargetPos);
        CaptureAndDisableUI(HPUI, ref hpTargetPos);

        // init HP tracking
        if (playerController != null) previousHP = playerController.HP;
    }

    private void CaptureAndDisableUI(GameObject uiObj, ref Vector2 outTarget) 
    {
        if (uiObj == null)
        {
            outTarget = Vector2.zero;
            return;
        }

        RectTransform rt = uiObj.GetComponent<RectTransform>();
        if (rt != null)
        {
            outTarget = rt.anchoredPosition;
        }
        else
        {
            outTarget = Vector2.zero;
        }

        uiObj.SetActive(false);
    }

    private void ShowDebugOptions()// Touche F1 pour afficher le panneau de Debug
    {
        if (debugPanel != null)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                debugPanel.SetActive(!debugPanel.activeSelf);
            }
        }
    }
    private void Start()
    {
        if (RespawnFadeImage != null)
            RespawnFadeImage.CrossFadeAlpha(0.0f, 1.0f, false); //Fondu de l'image de respawn au lancement du jeu
    }
    void Update()
    {
        ShowDebugOptions();

        if (RewindEnergy <= 0)
        {
            if (gameOverUI != null) gameOverUI.SetActive(true);
            Time.timeScale = 0f; // Affiche le menu de game over et met le temps à 0 pour pauser le jeu lorsque l'énergie de rewind est épuisée
        }
        if (playerController.HP <= 0)
        {
            Respawn();
        }
        if (Input.GetKeyDown(KeyCode.F2)) //  touche F2 pour effacer la position sauvegardée et réinitialiser le checkpoint
        {
            ErasePosition();
        }

        if (Input.GetButtonDown("Pause")) //  touche Échap pour basculer la pause du jeu
        {
            Debug.Log("Pause button pressed");
            TogglePause();
        }
        Win();
        GameOver();
        CheckIdleUI();
        CheckHPChange();
    }
    
    private void Win()
    {
        if (PlayerPrefs.HasKey("CollectedItems") && PlayerPrefs.GetString("CollectedItems").Contains("Parchment") && winUI != null)
        {
            Time.timeScale = 0f; // Affiche le menu de victoire et met le temps à 0 pour pauser le jeu lorsque le parchemin est collecté
            winUI.SetActive(true);
            PlayerPrefs.DeleteAll(); // Efface les données sauvegardées pour éviter les problèmes si on relance le jeu

        }      
    }

    private void GameOver()
        {
        if (gameOverUI != null && RewindEnergy <= 0)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f; // Met le temps à 0 pour pauser le jeu
            PlayerPrefs.DeleteAll(); //  Efface les données sauvegardées pour éviter les problèmes si on relance le jeu

        }
       
    }
    private void CheckIdleUI()
    {
        if (playerRb == null)
            return;

        if (ItemUI == null || HPUI == null || rewindEnergyUI == null)
            return;

        
        float speed = playerRb.linearVelocity.magnitude; // On récupère la vitesse du joueur pour déterminer s'il est à l'arrêt ou non
        if (speed <= idleVelocityThreshold)
        {
            idleTimer += Time.deltaTime;
            if (!idleUIsShown && idleTimer >= idleTimeToShow)
            {
                // On affiche les trois UI et on les laisse  tant que le joueur ne bouge pas
                // ShowItemUIHold(); pas eu le temps de faire l'UI d'item 
                ShowHPUIHold();
                ShowRewindUIHold();
                idleUIsShown = true;
            }
        }
        else
        {
            idleTimer = 0f;
            idleUIsShown = false;
            // Les coroutines hold détecteront le mouvement et feront disparaître les UI
        }
    }

    private void CheckHPChange() // Vérifie si la HP du joueur a changé pour afficher temporairement l'UI des HP et déclencher l'animation de dégâts
    {
        if (playerController == null || HPUI == null)
            return;

        if (previousHP != playerController.HP)
        {
            // HP a changé -> afficher temporairement la HP UI
            playerAnim.SetTrigger("Damage");
            hpSlider.value = (float)playerController.HP / playerController.maxHP; // Met à jour la barre de HP
            ShowHPUITemporary();
            previousHP = playerController.HP;

           
        }
    }

    public void TogglePause() // Méthode pour basculer entre pause et reprise du jeu
    {
        isGamePaused = !isGamePaused; // Inverse l'état de pause
        Time.timeScale = isGamePaused ? 0f : 1f; // Met le temps à 0 pour pauser, ou à 1 pour reprendre
        if (pauseMenuUI != null) pauseMenuUI.SetActive(isGamePaused); // Affiche ou masque le menu de pause
    }

    public void Respawn() //Méthode qui gère le respawn de mon joueur au dernier checkpoint passé.
    {
        if (player == null || respawnPoint == null)
        {
            Debug.LogWarning("[GameDirector] player ou respawnPoint null dans Respawn()");
            return;
        }

        RewindEnergy -= Mathf.RoundToInt(Vector2.Distance(player.transform.position, respawnPoint.position) / 2); // Réduit l'énergie de rewind en fonction de la distance entre le joueur et le dernier checkpoint

        if (rewindEnergyUI != null)
        {
          rewindEnergyUI.GetComponent<Slider>().value = (float)RewindEnergy / 100f; // Met à jour la barre d'énergie de rewind

          
            ShowRewindUIHold();
        }

        Debug.Log("Rewind Energy: " + RewindEnergy);
        LoadPosition();
        if (RespawnFadeImage != null)
            RespawnFadeImage.CrossFadeAlpha(1.0f, 0.05f, false); // Monte l'alpha vers 1

        if (playerController != null)
        {
            if (playerController.HP <= 0)
            {
                LoadPosition();
                playerController.HP = playerController.maxHP;
            }
        }
    }



    public void SavePosition(Transform lastCheckpoint)   // Sauvegarde la position du joueur
    {       

        // Assurer que respawnPoint existe
        if (respawnPoint == null)
            respawnPoint = new GameObject("RespawnPoint").transform;

        respawnPoint.position = lastCheckpoint.position;

        // Sauvegarde la position fournie (utiliser lastCheckpoint, plus fiable)
        PlayerPrefs.SetFloat("PlayerPosX", lastCheckpoint.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", lastCheckpoint.position.y);
        PlayerPrefs.Save();
    }
    public void LoadPosition()
    {         // Charge la position du joueur

        float x = PlayerPrefs.GetFloat("PlayerPosX", 0f);
        float y = PlayerPrefs.GetFloat("PlayerPosY", 0f);
        if (player != null)
            player.transform.position = new Vector2(x, y);
    }

    void OnApplicationQuit() //Méthode pour effacer les données de PlayerPrefs lorsque l'application est quittée
    {
        PlayerPrefs.DeleteAll();
    }

    public void ErasePosition()
    {         // Efface la position sauvegardée du joueur
        PlayerPrefs.DeleteKey("PlayerPosX");
        PlayerPrefs.DeleteKey("PlayerPosY");

    }

  
    public void ShowRewindUIHold()
    {
        if (rewindEnergyUI == null) return;
        if (rewindSlideCoroutine != null) StopCoroutine(rewindSlideCoroutine);
        rewindSlideCoroutine = StartCoroutine(SlideInAndHoldUntilMove(rewindEnergyUI, rewindSlideOffset, uiSlideDuration));
    }

    /*
    public void ShowItemUIHold()
    {
        if (ItemUI == null) return;
        if (itemSlideCoroutine != null) StopCoroutine(itemSlideCoroutine);
        itemSlideCoroutine = StartCoroutine(SlideInAndHoldUntilMove(ItemUI, -itemSlideOffset, uiSlideDuration));
    }
    */
    public void ShowHPUIHold()
    {
        if (HPUI == null) return;
        if (hpSlideCoroutine != null) StopCoroutine(hpSlideCoroutine);
        hpSlideCoroutine = StartCoroutine(SlideInAndHoldUntilMove(HPUI, hpSlideOffset, uiSlideDuration));
    }

    // S'affiche temporairement lorsque les Hp changent
    public void ShowHPUITemporary()
    {
        if (HPUI == null)
            return;

        // reset le timer de disparition
        hpUIHideTimer = uiVisibleDuration;

        // si déjà en animation -> ne rien relancer
        if (isHPUIAnimating)
            return;

        hpSlideCoroutine = StartCoroutine(HPUICoroutine());
    }
    private IEnumerator HPUICoroutine() // Coroutine pour faire apparaître temporairement l'UI des HP avec un slide-in et slide-out après un délai
    {
        isHPUIAnimating = true;

        RectTransform rt = HPUI.GetComponent<RectTransform>();

        Vector2 targetPos = GetStoredTargetPos(HPUI, rt);
        Vector2 startPos = targetPos + hpSlideOffset;

        rt.anchoredPosition = startPos;
        HPUI.SetActive(true);

        float elapsed = 0f; // elapsed pour le slide-in

        while (elapsed < uiSlideDuration)
        {
            elapsed += Time.unscaledDeltaTime; // On utilise unscaledDeltaTime pour que l'animation ne soit pas affectée par une pause du jeu
            float t = Mathf.SmoothStep( 0f, 1f,Mathf.Clamp01(elapsed / uiSlideDuration)); //SmoothStep permet d'avoir une animation plus fluide en accélérant au début et en ralentissant à la fin
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t); // Lerp pour faire le slide-in de la position de départ à la position cible
            yield return null;
        }

        rt.anchoredPosition = targetPos;

        // Attend tant que le timer est > 0
        while (hpUIHideTimer > 0f)
        {
            hpUIHideTimer -= Time.unscaledDeltaTime;
            yield return null;
        }

        // Slide out, même chose mais en inversant les positions
        elapsed = 0f;

        while (elapsed < uiSlideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(  0f,  1f,  Mathf.Clamp01(elapsed / uiSlideDuration) );
            rt.anchoredPosition = Vector2.Lerp(targetPos, startPos, t);
            yield return null;
        }

        rt.anchoredPosition = startPos;

        HPUI.SetActive(false);

        isHPUIAnimating = false;
        hpSlideCoroutine = null;
    }


    // Slide-in and on maintient l'UI tant que le joueur est immobile, puis slide-out dès qu'il bouge
    private IEnumerator SlideInAndHoldUntilMove(GameObject uiObject, Vector2 offsetFromTarget, float slideDuration)
    {
        if (uiObject == null) yield break;

        RectTransform rt = uiObject.GetComponent<RectTransform>();
        if (rt == null)
        {
            uiObject.SetActive(true);
            // wait until player moves
            while (playerRb != null && playerRb.linearVelocity.magnitude <= idleVelocityThreshold)
                yield return null;
            uiObject.SetActive(false);
            yield break;
        }

        Vector2 targetPos = GetStoredTargetPos(uiObject, rt);
        Vector2 startPos = targetPos + offsetFromTarget;

        // Slide-in
        rt.anchoredPosition = startPos;
        uiObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / slideDuration));
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rt.anchoredPosition = targetPos;

        // Rester visible tant que le joueur est immobile
        while (playerRb != null && playerRb.linearVelocity.magnitude <= idleVelocityThreshold)
        {
            yield return null;
        }

        // Slide-out
        elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / slideDuration));
            rt.anchoredPosition = Vector2.Lerp(targetPos, startPos, t);
            yield return null;
        }
        rt.anchoredPosition = startPos;
        uiObject.SetActive(false);
    }

    private Vector2 GetStoredTargetPos(GameObject uiObject, RectTransform rt) // Récupère la position cible stockée pour une UI donnée, ou retourne la position actuelle si aucune n'est trouvée
    {
        if (uiObject == rewindEnergyUI) return rewindTargetPos;
        if (uiObject == ItemUI) return itemTargetPos;
        if (uiObject == HPUI) return hpTargetPos;
        return rt.anchoredPosition;
    }
}
