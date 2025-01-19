using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;


public class GameManager : MonoBehaviour
{
    [Header("TEXTOS")]
    [SerializeField] Text txtMessage;
    [SerializeField] Text txtNiveles;
    [SerializeField] Text counterCoins;
    [Header("VALORES OBJETOS")]
    [SerializeField] PlayerController player;
    [SerializeField] CheckPointController checkController;
    [SerializeField] Image black;
    [Header("BARRA DE VIDA")]
    [SerializeField] GameObject imgLife;
    [SerializeField] Sprite life3, life2, life1, nolife;

    [Header("SONIDOS")]
    [SerializeField] AudioClip sfxGameOver;

    [Header("BOTONES")]
    [SerializeField] Button jumpButton;
    [SerializeField] Button upButton;
    [SerializeField] Button downButton;
    [SerializeField] Button rightButton;
    [SerializeField] Button leftButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button menuButton;
    [Header("IDIOMAS")]
    [SerializeField] LocalizeStringEvent localizedStringEventMessage;
    [SerializeField] LocalizeStringEvent localizedStringEventLevels;
    [SerializeField] Locale[] flagLocales;
    List<CoinsController> coinsControllers;
    public static GameManager Instance { get; private set; }
    public int levelNumber;
    AudioSource sfx;
    ScoreManager scoreManager;
    int lives;
    int coins;
    int sceneId;
    const int totalLevels = 4;
    bool fading = false;
    bool gameOver = false;
    bool sceneRestarted = false;
    bool paused;
    static bool checkpointReached = false;
    public int GetLives() => lives;
    public int GetCoins() => coins;
    public bool isGameOver() => gameOver;
    public bool isFading() => fading;

    public bool isChecking() => checkpointReached;

    public Image GetBlack() => black;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (!Instance.isFading())
            {
                Destroy(gameObject);
            }
        }
    }

    [Obsolete]
    public void Start()
    {
        Debug.Log("Start method called");
        Debug.Log("PlayerPrefs 'isFirstTime' value: " + PlayerPrefs.GetInt("isFirstTime", 1));

        scoreManager = ScoreManager.Instance;
        // Asegúrate de que el botón esté desactivado al inicio
        InitializeButtons();

        // Obtiene el ID de la escena y el número del nivel
        sceneId = SceneManager.GetActiveScene().buildIndex;
        levelNumber = sceneId - 2;

        //el audio
        sfx = GetComponent<AudioSource>();

        // Carga las vidas guardadas, si no hay ninguna guardada, el valor por defecto será 3
        lives = PlayerPrefs.GetInt("Lives", 3);
        //carga la lengua
        int languageIndex = PlayerPrefs.GetInt("SelectedLanguage", 1);

        // Si la escena no ha cambiado, recupera las monedas
        coins = PlayerPrefs.GetInt("TotalCoins", 0);

        // Guarda la escena actual
        PlayerPrefs.SetInt("LastScene", sceneId);
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
        ReadPreferences();
        coinsControllers = new List<CoinsController>(FindObjectsOfType<CoinsController>());
        //si es la primera vez que se recarga la escena
        if (PlayerPrefs.GetInt("isFirstTime", 1) == 1)
        {
            Debug.Log("Entramos en que es la primera vez que se carga");
            fading = true;
            black.gameObject.SetActive(true);
            // Tiempo de espera en segundos
            float delay = 2f;
            Invoke("StartFadeOut", delay);
            PlayerPrefs.SetInt("isFirstTime", 0);
            foreach (CoinsController controller in coinsControllers)
            {
                Debug.Log("Start: estado de la moneda con clave: " + controller.coinKey + ", valor: " + PlayerPrefs.GetInt(controller.coinKey, 0));
                if (PlayerPrefs.GetInt(controller.coinKey, 0) == 1) // Solo borrar si la moneda fue recogida
                {
                    Debug.Log("Start: Borrando clave de moneda: " + controller.coinKey);
                    PlayerPrefs.DeleteKey(controller.coinKey);
                }
            }

        }

        else
        {
            fading = true;

            StartCoroutine(FadeOut());
        }

        UpdateUI();
    }

    private void InitializeButtons()
    {
        jumpButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
    }

    void UpdateLanguage(int languageIndex)
    {
        if (languageIndex >= 1 && languageIndex <= flagLocales.Length)
        {
            LocalizationSettings.SelectedLocale = flagLocales[languageIndex - 1];
            localizedStringEventLevels.StringReference.SetReference("Prueba", "Niveles");
            localizedStringEventLevels.RefreshString();
            localizedStringEventMessage.StringReference.SetReference("Prueba", "Mensajes");
            localizedStringEventMessage.RefreshString();
        }
    }
    void StartFadeOut()
    {
        localizedStringEventLevels.StringReference.SetReference("Prueba", "Niveles");
        localizedStringEventLevels.StringReference.Arguments = new object[] { levelNumber };
        localizedStringEventLevels.RefreshString();
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnApplicationQuit();
        }
        else if (!gameOver && Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        else if (gameOver)
        {

#if UNITY_ANDROID
            restartButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
            jumpButton.gameObject.SetActive(false);
            upButton.gameObject.SetActive(false);
            downButton.gameObject.SetActive(false);
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
#else
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Nivel1");
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene("Menu");
            }
#endif
        }

    }
    public void OnRestartButtonPressed()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void OnMenuButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ReadPreferences()
    {
        int volumeMusic = PlayerPrefs.GetInt("MusicVolum", 5);
        int volumeSound = PlayerPrefs.GetInt("SoundVolum", 4);

        Camera.main.GetComponent<AudioSource>().volume = volumeMusic / 10F;
        sfx.volume = volumeSound / 10F;
        // Ajusta el volumen de los otros scripts

        player.AdjustVolume(volumeSound / 10F);
        checkController.AdjustVolume(volumeSound / 10F);

        GameObject allCoins = GameObject.Find("AllCoins");
        foreach (Transform child in allCoins.transform)
        {
            CoinsController coinController = child.GetComponent<CoinsController>();
            if (coinController != null)
            {
                coinController.AdjustVolume(volumeSound / 10F);
            }
        }
    }

    IEnumerator FadeIn()
    {
        txtMessage.text = "";
        jumpButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        fading = true;
        for (float i = 0; i <= 1; i += 1f * Time.deltaTime)
        {
            // Ajusta la transparencia del sprite, se aparece lentamente
            var color = black.color;
            color.a = i;
            black.color = color;

            yield return null;
        }
        // Comprueba si la escena que se va a cargar no es la última
        if (sceneId != SceneManager.sceneCountInBuildSettings - 1)
        {
            localizedStringEventLevels.StringReference.SetReference("Prueba", "Niveles");
            localizedStringEventLevels.StringReference.Arguments = new object[] { levelNumber };
            localizedStringEventLevels.RefreshString();          
        }
        else
        {
            txtNiveles.text = "";
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneId);
    }

    IEnumerator FadeOut()
    {
        // Comprueba si la escena que se va a cargar no es la última
        if (sceneId != SceneManager.sceneCountInBuildSettings - 1)
        {
            localizedStringEventLevels.StringReference.SetReference("Prueba", "Niveles");
            localizedStringEventLevels.StringReference.Arguments = new object[] { levelNumber };
            localizedStringEventLevels.RefreshString();          
        }
        Camera.main.GetComponent<AudioSource>().Play();
        black.gameObject.SetActive(true);
        for (float i = 1; i >= 0; i -= Time.deltaTime / 4)
        {
            //se desvanece
            var color = black.color;
            color.a = i;
            black.color = color;
            yield return null;
        }
        // Activar el botón de salto dependiendo de la plataforma
#if UNITY_ANDROID
        jumpButton.gameObject.SetActive(true);
        upButton.gameObject.SetActive(true);
        downButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);
        leftButton.gameObject.SetActive(true);
#else
        jumpButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
#endif
        txtNiveles.text = "";
        fading = false;
    }

    public void LoseLife()
    {

        if (!gameOver)
        {
            lives--;
            //resetea vidas a las actuales
            PlayerPrefs.SetInt("Lives", lives);

            if (lives <= 0)
            {
                Camera.main.GetComponent<AudioSource>().Stop();
                sfx.clip = sfxGameOver;
                sfx.Play();
                GameOver();
            }
            UpdateUI();
        }
    }
    void UpdateUI()
    {
        switch (lives)
        {
            case 3:
                imgLife.GetComponent<Image>().sprite = life3;
                break;
            case 2:
                imgLife.GetComponent<Image>().sprite = life2;
                break;
            case 1:
                imgLife.GetComponent<Image>().sprite = life1;
                break;
            case 0:
                imgLife.GetComponent<Image>().sprite = nolife;
                break;
            default:
                break;
        }
        if (gameOver)
        {
            counterCoins.text = "00";
        }
        else
        {
            // Actualiza el texto de las monedas acumuladas sin mostrar el total necesario
            if (coins < 10)
            {
                counterCoins.text = "0" + coins;
            }
            else
            {
                counterCoins.text = coins.ToString();
            }
        }
    }

    public void CheckpointReached()
    {
        checkpointReached = true;
        sceneRestarted = false;
    }

    public async void GameOver()
    {
        gameOver = true;
        checkpointReached = false;
        //mostramos el texto de Game Over
#if UNITY_ANDROID
        localizedStringEventMessage.StringReference.SetReference("Prueba", "GameOver_Android");
#else
        localizedStringEventMessage.StringReference.SetReference("Prueba", "GameOver_PC");
#endif
        // Guardar el puntaje en Firestore si es lo suficientemente alto
        if (scoreManager != null)
        {
            await scoreManager.SaveScoreIfHighEnough(coins);
        }
        // Resetear las vidas, la posición del jugador y monedas en PlayerPrefs
        PlayerPrefs.SetInt("Lives", 3);
        PlayerPrefs.SetFloat("PlayerX", player.GetInitialPosition().x);
        PlayerPrefs.SetFloat("PlayerY", player.GetInitialPosition().y);
        PlayerPrefs.SetInt("TotalCoins", 0);
        PlayerPrefs.DeleteKey("isFirstTime");
        Debug.Log("Gameover'isFirstTime' value: " + PlayerPrefs.GetInt("isFirstTime", 1));
        foreach (CoinsController controller in coinsControllers)
        {
            if (PlayerPrefs.GetInt(controller.coinKey, 0) == 1)
            {
                Debug.Log("GameOver: borrar clave de moneda: " + controller.coinKey);
                PlayerPrefs.DeleteKey(controller.coinKey);
            }
        }

    }
    public async void CompleteLevel(int levelNumber)
    {
        // Verificar si es el último nivel
        if (levelNumber == 4)
        {
            // Guardar puntuación si es mayor o igual 4000 al completar el nivel 4
            if (scoreManager != null)
            {
                await scoreManager.SaveScoreIfHighEnough(coins);
            }
        }
    }
    [Obsolete]
    public async void LoadScene(bool loadNextScene = false)
    {
        // Guarda la puntuación actual en PlayerPrefs
        PlayerPrefs.SetInt("TotalCoins", coins);
        black.gameObject.SetActive(true);
        sceneRestarted = true;
        gameOver = false;
        Camera.main.GetComponent<AudioSource>().Stop();
        // Ocultar los botones de reinicio y menú
        restartButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);

        // Verificar si se ha completado el último nivel
        if (sceneId >= totalLevels)
        {
            await scoreManager.SaveScoreIfHighEnough(coins);
        }
        if (loadNextScene)
        {
            sceneId++;
            checkpointReached = false;
        }

        StartCoroutine(FadeIn());
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Lives");
        PlayerPrefs.DeleteKey("TotalCoins");
        PlayerPrefs.DeleteKey("isFirstTime");
        foreach (CoinsController controller in coinsControllers)
        {
            Debug.Log("Exit: estado de la moneda con clave: " + controller.coinKey + ", valor: " + PlayerPrefs.GetInt(controller.coinKey, 0));
            if (PlayerPrefs.GetInt(controller.coinKey, 0) == 1) // Solo borrar si la moneda fue recogida
            {
                Debug.Log("Exit: Borrando clave de moneda: " + controller.coinKey);
                PlayerPrefs.DeleteKey(controller.coinKey);
            }
        }
        Application.Quit();
    }

    void PauseGame()
    {
        paused = true;
        //Pausamo sonido del juego y aparece texto
        Camera.main.GetComponent<AudioSource>().Stop();
#if UNITY_ANDROID
        txtMessage.text = "Pausa";
#else
        localizedStringEventMessage.StringReference.SetReference("Prueba", "Pausa_PC");
        localizedStringEventMessage.RefreshString();
#endif
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        paused = false;
        //Pausamo sonido del juego y aparece texto
        Camera.main.GetComponent<AudioSource>().Play();
        txtMessage.text = "";
        Time.timeScale = 1;
    }

    public static void SumCoins(int coinValue)
    {
        // Incrementa el contador de monedas según el valor de la moneda recogida
        Instance.coins += coinValue;

        // Actualiza el texto del contador de monedas en la UI
        if (Instance.coins < 10)
        {
            Instance.counterCoins.text = "0" + Instance.coins;
        }
        else
        {
            Instance.counterCoins.text = Instance.coins.ToString();
        }
    }
}
