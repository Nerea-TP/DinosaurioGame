using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    [SerializeField] LocalizeStringEvent localizedStringEvent;
    [SerializeField] LocalizeStringEvent localizedPlatformMessageEvent;
    [SerializeField] Text txtMessage;
    [SerializeField] Text txtMessageCopy;
    [SerializeField] Text txtPlatformMessage;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button menuButton;
    RectTransform rectTransform;
    RectTransform rectTransformCopy;
    float fullWidth;
    float copyWidth;
    float screenWidth;

    void Start()
    {
        rectTransform = txtMessage.GetComponent<RectTransform>();
        rectTransformCopy = txtMessageCopy.GetComponent<RectTransform>();
        LoadLanguage();
        // Inicializar el ancho total del texto y la pantalla
        fullWidth = txtMessage.preferredWidth;
        copyWidth = txtMessageCopy.preferredWidth;
        screenWidth = Screen.width;
    }

    void Update()
    {
        // Mover ambos textos de derecha a izquierda
        rectTransform.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);
        rectTransformCopy.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);

        // Si el primer texto ha salido completamente de la pantalla, reiniciarlo
        if (rectTransform.anchoredPosition.x <= -fullWidth)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransformCopy.anchoredPosition.x + copyWidth + 2000f, rectTransform.anchoredPosition.y);
        }

        // Si la copia ha salido completamente de la pantalla, reiniciarla
        if (rectTransformCopy.anchoredPosition.x <= -fullWidth)
        {
            rectTransformCopy.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + fullWidth + 2000f, rectTransformCopy.anchoredPosition.y);
        }
#if UNITY_ANDROID
           exitButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
            //jumpButton.gameObject.SetActive(false);
            //jumpButton.gameObject.SetActive(false);
            //downButton.gameObject.SetActive(false);
            //leftButton.gameObject.SetActive(false);
            //rightButton.gameObject.SetActive(false);
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("Menu");
        }
#endif  
    }
    private void LoadLanguage()
    {
        // Cargar el idioma guardado
        int languageIndex = PlayerPrefs.GetInt("SelectedLanguage", 1); // 1 es el valor por defecto (español)
        ChangeLanguage(languageIndex);
    }

    private void ChangeLanguage(int flagIndex)
    {
        if (flagIndex >= 1 && flagIndex <= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            Locale locale = LocalizationSettings.AvailableLocales.Locales[flagIndex - 1];
            LocalizationSettings.SelectedLocale = locale;
            UpdateLocalizedTexts();
        }
    }
     public void OnExitButtonPressed()
    {
        Application.Quit();
    }

    public void OnMenuButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }


    private void UpdateLocalizedTexts()
    {
        if (localizedStringEvent != null && localizedStringEvent.StringReference != null)
        {
            txtMessage.text = localizedStringEvent.StringReference.GetLocalizedString();
            txtMessageCopy.text = localizedStringEvent.StringReference.GetLocalizedString();
        }
        if (localizedPlatformMessageEvent != null && localizedPlatformMessageEvent.StringReference != null)
        {
            // Configura el mensaje para la plataforma específica
#if UNITY_ANDROID
                localizedPlatformMessageEvent.StringReference.SetReference("Prueba", "Ranking_Android"); // Referencia para Android
#else
            localizedPlatformMessageEvent.StringReference.SetReference("Prueba", "Ranking_PC"); // Referencia para PC
#endif

            // Obtener y asignar el mensaje adicional localizado
            txtPlatformMessage.text = localizedPlatformMessageEvent.StringReference.GetLocalizedString();
        }

    }
}
