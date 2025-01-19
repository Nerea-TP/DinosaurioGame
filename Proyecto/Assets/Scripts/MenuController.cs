using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("OPCIONES GENERALES")]
    [SerializeField] float timeChange;
    [SerializeField] int volumeMusic;
    [SerializeField] int volumeSound;
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject optionsScreen;

    [Header("ELEMENTOS DEL OPCIONES")]
    [SerializeField] SpriteRenderer music;
    [SerializeField] SpriteRenderer sound;
    [SerializeField] SpriteRenderer back;

    [Header("ELEMENTOS DEL MENU")]
    [SerializeField] SpriteRenderer start;
    [SerializeField] SpriteRenderer ranking;
    [SerializeField] SpriteRenderer options;
    [SerializeField] SpriteRenderer exit;

    [Header("SPRITES DEL MENU")]
    [SerializeField] Sprite start_off;
    [SerializeField] Sprite start_on;
    [SerializeField] Sprite ranking_on;
    [SerializeField] Sprite ranking_off;
    [SerializeField] Sprite options_off;
    [SerializeField] Sprite options_on;
    [SerializeField] Sprite exit_off;
    [SerializeField] Sprite exit_on;
    [Header("SPRITES DE OPCIONES")]
    [SerializeField] Sprite music_off;
    [SerializeField] Sprite music_on;
    [SerializeField] Sprite sound_off;
    [SerializeField] Sprite sound_on;
    [SerializeField] Sprite back_off;
    [SerializeField] Sprite back_on;
    [SerializeField] Sprite vol_on;
    [SerializeField] Sprite vol_off;
    [SerializeField] SpriteRenderer[] music_spr;
    [SerializeField] SpriteRenderer[] sound_spr;
    [Header("ELEMENTOS DE LAS BANDERAS")]
    [SerializeField] SpriteRenderer spanish;
    [SerializeField] SpriteRenderer galician;
    [SerializeField] SpriteRenderer english;
    [Header("SPRITES DE LAS BANDERAS")]
    [SerializeField] Sprite spanish_off;
    [SerializeField] Sprite spanish_on;
    [SerializeField] Sprite english_off;
    [SerializeField] Sprite english_on;
    [SerializeField] Sprite galician_off;
    [SerializeField] Sprite galician_on;

    [Header("SONIDOS DEL MENU")]
    [SerializeField] AudioSource sfxMenu;
    [SerializeField] AudioSource sfxOptions;
    [SerializeField] AudioSource sfxSelection;
    [SerializeField] Locale[] flagLocales;

    int screen;
    int optionMenu, optionMenuAnt;
    int optionOptions, optionOptionsAnt;
    int optionFlags, optionFlagsAnt;
    bool inFlags;

    bool submit;
    bool isAdjustingMusic = true; // True para ajustar música, false para ajustar sonido

    float x, y;
    float timeX, timeY;

    void Awake()
    {
        screen = 0;
        timeX = timeY = 0;
        optionMenu = optionMenuAnt = 1;
        optionFlags = optionFlagsAnt = 1;
        inFlags = false;
        AdjustOptions();
        ReadPreferences();
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
    }

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += UpdateLocalizedSprites;
    }


    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= UpdateLocalizedSprites;
    }

    void UpdateLocalizedSprites(Locale newLocale)
    {
        // Mantiene el idioma seleccionado y los sprites actualizados
        if (LocalizationSettings.SelectedLocale != newLocale)
        {
            LocalizationSettings.SelectedLocale = newLocale;
        }
        var startOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "start_on");
        var startOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "start_off");
        var optionsOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "options_on");
        var optionsOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "options_off");
        var exitOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "exit_on");
        var exitOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "exit_off");


        var musicOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "music_on");
        var musicOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "music_off");
        var soundOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "sound_on");
        var soundOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "sound_off");
        var backOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "back_on");
        var backOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "back_off");

        if (inFlags)
        {
            start.sprite = startOffSprite;
            options.sprite = optionsOffSprite;
            ranking.sprite = ranking_off;
            exit.sprite = exitOffSprite;
            return;
        }
        if (startOnSprite != null && startOffSprite != null)
        {
            start.sprite = (optionMenu == 1) ? startOnSprite : startOffSprite;
        }
        if (optionsOnSprite != null && optionsOffSprite != null)
        {
            options.sprite = (optionMenu == 2) ? optionsOnSprite : optionsOffSprite;
        }
        if (ranking_on != null && ranking_off != null)
        {
            ranking.sprite = (optionMenu == 3) ? ranking_on : ranking_off;
        }


        if (exitOnSprite != null && exitOffSprite != null)
        {
            exit.sprite = (optionMenu == 4) ? exitOnSprite : exitOffSprite;
        }

        if (musicOnSprite != null && musicOffSprite != null)
        {
            music.sprite = (optionOptions == 1) ? musicOnSprite : musicOffSprite;
        }

        if (soundOnSprite != null && soundOffSprite != null)
        {
            sound.sprite = (optionOptions == 2) ? soundOnSprite : soundOffSprite;
        }

        if (backOnSprite != null && backOffSprite != null)
        {
            back.sprite = (optionOptions == 3) ? backOnSprite : backOffSprite;
        }
    }

    private void AdjustOptions()
    {
        //nos muestra cuando se entra en opciones como está ajustado la musica y sonido
        AdjustMusic();
        AdjustSound();
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
    }

    void Update()
    {
        y = Input.GetAxisRaw("Vertical");
        x = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonUp("Submit")) submit = false;
        if (y == 0) timeY = 0;
        if (screen == 0) Menu();
        if (screen == 1) OptionsMenu();
    }


    private void Menu()//nos movemos por el menu principal
    {
        if (y != 0)
        {
            if (timeY == 0 || timeY > timeChange)
            {
                if (inFlags)
                {
                    if (y == 1 && optionFlags > 1) SelectionFlags(optionFlags - 1);
                    if (y == -1 && optionFlags < 3) SelectionFlags(optionFlags + 1);
                }
                else
                {
                    if (y == 1 && optionMenu > 1) SelectionMenu(optionMenu - 1);
                    if (y == -1 && optionMenu < 4) SelectionMenu(optionMenu + 1);
                }
                if (timeY > timeChange) timeY = 0;
            }
            timeY += Time.deltaTime;
        }
        if (x != 0)
        {
            if (x == 1 && !inFlags)
            {
                inFlags = true;
                SelectionFlags(1); // Selecciona la primera bandera al movernos hacia las banderas
                ResetMenuSprites();
            }
            if (x == -1 && inFlags)
            {
                inFlags = false;
                ResetFlags(); // Resetea las banderas al salir
                SelectionMenu(optionMenu); // Vuelve a poner el sprite_on del menú seleccionado
            }
        }
        if (Input.GetButtonDown("Submit") && !submit)
        {
            sfxSelection.Play();
            if (inFlags)
            {
                ChangeLanguage(optionFlags); // Cambia el idioma según la bandera seleccionada
            }
            else
            {
                sfxSelection.Play();
                if (optionMenu == 1) SceneManager.LoadScene("Instrucciones");
                if (optionMenu == 2) LoadScreenOptions();
                if (optionMenu == 3) SceneManager.LoadScene("Ranking");
                if (optionMenu == 4) Application.Quit();
            }
        }
    }
    private void ResetMenuSprites()
    {
        start.sprite = start_off;
        options.sprite = options_off;
        exit.sprite = exit_off;
    }

    public void ChangeLanguage(int flagIndex)
    {
        if (flagIndex >= 1 && flagIndex <= flagLocales.Length)
        {
            LocalizationSettings.SelectedLocale = flagLocales[flagIndex - 1];
            PlayerPrefs.SetInt("SelectedLanguage", flagIndex);
            PlayerPrefs.Save();
            UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
        }
    }

    private void ResetFlags()
    {
        spanish.sprite = spanish_off;
        galician.sprite = galician_off;
        english.sprite = english_off;
    }

    private void SelectionFlags(int op)
    {
        sfxOptions.Play();
        optionFlags = op;
        english.sprite = (op == 1) ? english_on : english_off;
        galician.sprite = (op == 2) ? galician_on : galician_off;
        spanish.sprite = (op == 3) ? spanish_on : spanish_off;
        optionFlagsAnt = op;
    }


    private void OptionsMenu()//nos movemos por el menu opciones
    {
        if (y != 0)
        {
            if (timeY == 0 || timeY > timeChange)
            {
                if (y == 1 && optionOptions > 1) SelectionOptions(optionOptions - 1);
                if (y == -1 && optionOptions < 3) SelectionOptions(optionOptions + 1);
                if (timeY > timeChange) timeY = 0;
            }
            timeY += Time.deltaTime;
        }
        if (x == 0) timeX = 0;
        else
        {//para aumentar o disminuir el volumen y el sonido
            if ((timeX == 0 || timeX > timeChange) && (optionOptions == 1 || optionOptions == 2))
            {
                if (optionOptions == 1 && ((x < 0 && volumeMusic > 0) || (x > 0 && volumeMusic < 10)))
                {
                    volumeMusic += (int)x;
                    AdjustMusic();
                }
                if (optionOptions == 2 && ((x < 0 && volumeSound > 0) || (x > 0 && volumeSound < 10)))
                {
                    volumeSound += (int)x;
                    AdjustSound();
                    sfxOptions.Play();
                }
                if (timeX > timeChange) timeX = 0;
            }
            timeX += Time.deltaTime;
        }


        if (Input.GetButtonDown("Submit") && optionOptions == 3 && !submit)
        {
            LoadPreferences();
            LoadScreenMenu();
        }
    }

    private void LoadPreferences()
    {
        PlayerPrefs.SetInt("MusicVolum", volumeMusic);
        PlayerPrefs.SetInt("SoundVolum", volumeSound);
        PlayerPrefs.Save();
    }
    void ReadPreferences()
    {
        volumeMusic = PlayerPrefs.GetInt("MusicVolum", 5);
        volumeSound = PlayerPrefs.GetInt("SoundVolum", 4);
        int selectedLanguage = PlayerPrefs.GetInt("SelectedLanguage", 1); // Carga el idioma
        ChangeLanguage(selectedLanguage);
    }
    public void HandleTouch(GameObject touchedObject)
    {
        switch (touchedObject.name)
        {
            case "Start":
                SceneManager.LoadScene("Instrucciones");
                break;
            case "Ranking":
                SceneManager.LoadScene("Ranking");
                break;
            case "Options":
                LoadScreenOptions();
                break;
            case "Exit":
                Application.Quit();
                break;
            case "MusicOption":
                isAdjustingMusic = true;
                SetOptionSprites(true);
                //UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
                break;
            case "SoundOption":
                isAdjustingMusic = false;
                SetOptionSprites(false);
                //UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
                break;
            case "Back":
                LoadPreferences();
                LoadScreenMenu();
                break;
            case "English":
                optionFlags = 1;
                english.sprite = english_on;
                spanish.sprite = spanish_off;
                galician.sprite = galician_off;
                ChangeLanguage(optionFlags);
                break;
            case "Spanish":
                optionFlags = 3;
                spanish.sprite = spanish_on;
                english.sprite = english_off;
                galician.sprite = galician_off;
                ChangeLanguage(optionFlags);
                break;
            case "Galician":
                optionFlags = 2;
                galician.sprite = galician_on;
                english.sprite = english_off;
                spanish.sprite = spanish_off;
                ChangeLanguage(optionFlags);
                break;
            default:
                // Asume que son toques en las barras de volumen
                if (touchedObject.name.StartsWith("Vol"))
                {
                    AdjustVolumeBasedOnTouch(touchedObject);
                }
                break;
        }
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
    }

    private void SetOptionSprites(bool isMusic)
    {
        Debug.Log("SetOptionSprites called. isMusic = " + isMusic);

        // Obtener los sprites localizados de la base de datos
        var soundOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "sound_on");
        var soundOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "sound_off");
        var musicOnSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "music_on");
        var musicOffSprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>("SpritesMenu", "music_off");

        Debug.Log("Sprites cargados: " + "Sound On: " + (soundOnSprite != null ? "Sí" : "No") + ", " + "Sound Off: " + (soundOffSprite != null ? "Sí" : "No") + ", " + "Music On: " + (musicOnSprite != null ? "Sí" : "No") + ", " + "Music Off: " + (musicOffSprite != null ? "Sí" : "No"));
        if (isMusic)
        {
            Debug.Log("Music option selected. Music sprite set to: " + music.sprite.name + ", Sound sprite set to: " + sound.sprite.name);
            music.sprite = musicOnSprite;  // Usa el sprite localizado para la música
            sound.sprite = soundOffSprite;  // Usa el sprite localizado para el sonido
        }
        else
        {
            Debug.Log("Sound option selected. Music sprite set to: " + music.sprite.name + ", Sound sprite set to: " + sound.sprite.name);
            music.sprite = musicOffSprite;  // Usa el sprite localizado para la música
            sound.sprite = soundOnSprite;  // Usa el sprite localizado para el sonido
        }        
        // Actualiza los sprites localizados después de cambiar la configuración
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);

    }


    private void AdjustVolumeBasedOnTouch(GameObject touchedObject)
    {
        string name = touchedObject.name;
        if (name.StartsWith("Vol"))
        {
            int level = int.Parse(name.Substring(3)); // Asumiendo nombres como vol0, vol1, ..., vol10
            if (screen == 1) // Asegúrate de que estás en la pantalla correcta
            {
                if (isAdjustingMusic)
                {
                    volumeMusic = level;
                    AdjustMusic();
                }
                else
                {
                    volumeSound = level;
                    AdjustSound();
                }
            }
        }
    }

    private void AdjustMusic()
    {
        if (volumeMusic == 0) music_spr[0].enabled = true;
        else music_spr[0].enabled = false;
        for (int i = 1; i <= 10; i++)
        {
            if (i <= volumeMusic) music_spr[i].sprite = vol_on;
            else music_spr[i].sprite = vol_off;
        }
        sfxMenu.volume = volumeMusic / 10F;

    }

    private void AdjustSound()
    {
        if (volumeSound == 0) sound_spr[0].enabled = true;
        else sound_spr[0].enabled = false;
        for (int i = 1; i <= 10; i++)
        {
            if (i <= volumeSound) sound_spr[i].sprite = vol_on;
            else sound_spr[i].sprite = vol_off;
        }
        GameObject[] sounds = GameObject.FindGameObjectsWithTag("Sounds");
        foreach (GameObject sound in sounds)
        {
            sound.GetComponent<AudioSource>().volume = volumeSound / 10F;
        }
    }

    private void LoadScreenOptions()
    {
        spanish.enabled = false;
        galician.enabled = false;
        english.enabled = false;
        submit = true;
        sfxSelection.Play();
        menuScreen.SetActive(false);
        screen = 1;
        optionOptions = optionOptionsAnt = 1;
        music.sprite = music_on;
        sound.sprite = sound_off;
        back.sprite = back_off;
        optionsScreen.SetActive(true);
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
    }
    private void LoadScreenMenu()
    {
        spanish.enabled = true;
        galician.enabled = true;
        english.enabled = true;
        submit = true;
        sfxSelection.Play();
        optionsScreen.SetActive(false);
        screen = 0;
        menuScreen.SetActive(true);
    }

    private void SelectionOptions(int op)
    {
        sfxOptions.Play();
        optionOptions = op;
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
        optionOptionsAnt = op;
    }

    private void SelectionMenu(int op)
    {
        sfxOptions.Play();
        optionMenu = op;
        UpdateLocalizedSprites(LocalizationSettings.SelectedLocale);
        optionMenuAnt = op;
    }
}

