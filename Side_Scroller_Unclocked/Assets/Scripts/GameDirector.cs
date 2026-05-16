using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameDirector : MonoBehaviour // Script pour gérer les éléments globaux du jeu, notamment le respawn du joueur, la sauvegarde de sa position, et la gestion de la pause du jeu.
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

    [Header("UI Management")]
    public bool isGamePaused = false; // Variable pour suivre l'état de pause du jeu
    public GameObject pauseMenuUI; // Référence à l'UI du menu de pause
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject rewindEnergyUI; // Référence à l'UI de rewind energy
    [SerializeField] private GameObject ItemUI;
    [SerializeField] private GameObject HPUI;

    [Header("Slide settings (shared)")]
    [SerializeField] private float uiSlideDuration = 0.5f; // durée du slide
    [SerializeField] private float uiVisibleDuration = 1.5f; // durée visible avant de ressortir (pour temporaires)
    [SerializeField] private Vector2 rewindSlideOffset = new Vector2(300f, 0f); // offset pour rewind (droite)
    [SerializeField] private Vector2 hpSlideOffset = new Vector2(-300f, 0f); // offset pour HP (gauche => negative X)
    [SerializeField] private Vector2 itemSlideOffset = new Vector2(0f, -200f); // offset pour Item (bas => negative Y)

    [Header("Idle detection")]
    [SerializeField] private float idleVelocityThreshold = 0.1f; // vitesse en dessous de laquelle on considère le joueur à l'arrêt
    [SerializeField] private float idleTimeToShow = 1.0f; // temps d'inactivité avant d'afficher les UI
    private float idleTimer = 0f;
    private bool idleUIsShown = false;

    private Transform respawnPoint; // Variable pour stocker la référence au dernier checkpoint passé par le joueur

    // Coroutines en cours pour contrôler une seule animation par UI
    private Coroutine rewindSlideCoroutine;
    private Coroutine itemSlideCoroutine;
    private Coroutine hpSlideCoroutine;

    // Positions cibles capturées au démarrage (évite que les LayoutGroups modifient les cibles pendant l'anim)
    private Vector2 rewindTargetPos;
    private Vector2 itemTargetPos;
    private Vector2 hpTargetPos;

    private int previousHP = -1;

    private void Awake() // Dans l'Awake j'attribue les valeurs
    {
        Time.timeScale = 1f; // Assure que le temps est normal au lancement du jeu
        if (RespawnFadeImage != null)
            RespawnFadeImage.enabled = true;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        if (player != null && spawn != null)
            player.transform.position = spawn.position; // Place le joueur à la position de spawn au début du jeu

        if (spawn != null)
        {
            respawnPoint = spawn;
            SavePosition(spawn); // Sauvegarde la position de spawn comme point de respawn initial
        }

        borderTrigger = FindAnyObjectByType<borderTrigger>(); // Trouve une instance de borderTrigger dans la scène

        // Capturer positions cibles des UI (anchoredPosition) et désactiver pour animation propre
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

    private void ShowDebugOptions()// Touche F1 pour afficher le panneau Debug
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
        LoadPosition(); // Charge la position sauvegardée du joueur au début du jeu
    }
    void Update()
    {
        ShowDebugOptions();

        if (RewindEnergy <= 0)
        {
            if (gameOverUI != null) gameOverUI.SetActive(true);
            Time.timeScale = 0f; // Affiche le menu de game over et met le temps à 0 pour pauser le jeu lorsque l'énergie de rewind est épuisée
        }
        if (Input.GetKeyDown(KeyCode.R)) //  touche R pour effacer la position sauvegardée et réinitialiser le checkpoint
        {
            ErasePosition();
        }

        if (Input.GetButtonDown("Pause")) //  touche Échap pour basculer la pause du jeu
        {
            Debug.Log("Pause button pressed");
            TogglePause();
        }

        CheckIdleUI();
        CheckHPChange();
    }

    private void CheckIdleUI()
    {
        if (playerRb == null)
            return;

        if (ItemUI == null || HPUI == null || rewindEnergyUI == null)
            return;

        // Utiliser la vitesse pour détecter l'arrêt
        float speed = playerRb.linearVelocity.magnitude;
        if (speed <= idleVelocityThreshold)
        {
            idleTimer += Time.deltaTime;
            if (!idleUIsShown && idleTimer >= idleTimeToShow)
            {
                // afficher les trois UI et les garder tant que le joueur ne bouge pas
                ShowItemUIHold();
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

    private void CheckHPChange()
    {
        if (playerController == null || HPUI == null)
            return;

        if (previousHP != playerController.HP)
        {
            // HP a changé -> afficher temporairement la HP UI
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
          rewindEnergyUI.GetComponentInChildren<TextMeshProUGUI>().text = RewindEnergy.ToString();
            
            // Démarrer le slide-in puis hold (si idle) ou temporaire (si appelé ici)
            ShowRewindUIHold();
        }

        Debug.Log("Rewind Energy: " + RewindEnergy);
        LoadPosition();
        if (RespawnFadeImage != null)
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



    public void SavePosition(Transform lastCheckpoint)
    {         // Sauvegarde la position du joueur

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

    void OnApplicationQuit() //Méthode pour effacer les données de PlayerPrefs lorsque l'application est quittée, afin d'éviter de charger une position obsolète lors du prochain lancement du jeu.
    {
        PlayerPrefs.DeleteAll();
    }

    public void ErasePosition()
    {         // Efface la position sauvegardée du joueur
        PlayerPrefs.DeleteKey("PlayerPosX");
        PlayerPrefs.DeleteKey("PlayerPosY");

    }

    // Hold variants : slide in and stay until player moves
    public void ShowRewindUIHold()
    {
        if (rewindEnergyUI == null) return;
        if (rewindSlideCoroutine != null) StopCoroutine(rewindSlideCoroutine);
        rewindSlideCoroutine = StartCoroutine(SlideInAndHoldUntilMove(rewindEnergyUI, rewindSlideOffset, uiSlideDuration));
    }

    public void ShowItemUIHold()
    {
        if (ItemUI == null) return;
        if (itemSlideCoroutine != null) StopCoroutine(itemSlideCoroutine);
        itemSlideCoroutine = StartCoroutine(SlideInAndHoldUntilMove(ItemUI, itemSlideOffset, uiSlideDuration));
    }

    public void ShowHPUIHold()
    {
        if (HPUI == null) return;
        if (hpSlideCoroutine != null) StopCoroutine(hpSlideCoroutine);
        hpSlideCoroutine = StartCoroutine(SlideInAndHoldUntilMove(HPUI, hpSlideOffset, uiSlideDuration));
    }

    // Temporary HP UI (appears even when player moves)
    public void ShowHPUITemporary()
    {
        if (HPUI == null) return;
        if (hpSlideCoroutine != null) StopCoroutine(hpSlideCoroutine);
        hpSlideCoroutine = StartCoroutine(SlideInThenOutCoroutine(HPUI, hpSlideOffset, uiSlideDuration, uiVisibleDuration));
    }

    // Coroutine générique : anime le slide-in, attend, puis anime le slide-out (utilise unscaled time)
    // Utilise positions cibles capturées au démarrage pour éviter "saut" causé par Layout.
    private IEnumerator SlideInThenOutCoroutine(GameObject uiObject, Vector2 offsetFromTarget, float slideDuration, float visibleDuration)
    {
        if (uiObject == null) yield break;

        RectTransform rt = uiObject.GetComponent<RectTransform>();
        if (rt == null)
        {
            uiObject.SetActive(true);
            yield return new WaitForSecondsRealtime(visibleDuration);
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

        // Rester visible un moment (unscaled pour fonctionner même si Time.timeScale = 0)
        float visibleElapsed = 0f;
        while (visibleElapsed < visibleDuration)
        {
            visibleElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Slide-out (retour vers la startPos)
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

    // Slide-in and hold until player moves
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
            yield return null; // unscaled not needed here because we are waiting for player movement (game may be running)
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

    private Vector2 GetStoredTargetPos(GameObject uiObject, RectTransform rt)
    {
        if (uiObject == rewindEnergyUI) return rewindTargetPos;
        if (uiObject == ItemUI) return itemTargetPos;
        if (uiObject == HPUI) return hpTargetPos;
        // fallback: current
        return rt.anchoredPosition;
    }
}
