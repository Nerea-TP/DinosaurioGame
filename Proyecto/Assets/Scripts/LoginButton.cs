using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    public AuthenticationManager authenticationManager;
    [SerializeField] InputField emailInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Text feedbackText;
    [SerializeField] LocalizeStringEvent localizedStringEventMessage;

    void Start()
    {
        LoadLanguage();
        HideFeedback();
        // Suscribirse a los eventos OnValueChanged de los InputFields
        emailInput.onValueChanged.AddListener(delegate { HideFeedback(); });
        passwordInput.onValueChanged.AddListener(delegate { HideFeedback(); });

        // Enfocar el campo de correo electrónico al iniciar la escena
        emailInput.ActivateInputField();
        // Suscribirse a los eventos OnEndEdit para manejar el enter key
        emailInput.onEndEdit.AddListener(delegate { OnEmailSubmit(); });
        passwordInput.onEndEdit.AddListener(delegate { OnPasswordSubmit(); });
    }
    private void LoadLanguage()
    {
        int languageIndex = PlayerPrefs.GetInt("SelectedLanguage", 1); // 1 es el valor por defecto (español)
        ChangeLanguage(languageIndex); // Cambia el idioma según el índice
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

    private void UpdateLocalizedTexts()
    {
        // Actualiza los textos locales en función del idioma seleccionado
        if (localizedStringEventMessage != null && localizedStringEventMessage.StringReference != null)
        {
            feedbackText.text = localizedStringEventMessage.StringReference.GetLocalizedString(); // Actualiza el texto de feedback
        }
    }

    private void OnEmailSubmit()
    {
        // Mover el enfoque al campo de contraseña al presionar Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            passwordInput.ActivateInputField();
        }
    }
    private void OnPasswordSubmit()
    {
        // Enviar el formulario al presionar Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnButtonClick();
        }
    }
    public void OnButtonClick()
    {
        // Validar campos vacíos
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.Log("Error: Los campos de correo electrónico y contraseña deben estar completos.");
            localizedStringEventMessage.StringReference.SetReference("Prueba", "CamposCompletos");
            localizedStringEventMessage.RefreshString();
            ShowFeedback();
            return; // Detener la ejecución si los campos están vacíos
        }
        authenticationManager.AuthenticateUser()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result) // Verificar si la autenticación fue exitosa
                {
                    Debug.Log("Autenticación exitosa, cargando la escena 'Menu'.");
                    SceneManager.LoadScene("Menu");
                }
            });
    }
    private void ShowFeedback()
    {
        feedbackText.gameObject.SetActive(true); // Asegúrate de que el texto esté visible
    }
    private void HideFeedback()
    {
        feedbackText.gameObject.SetActive(false); // Ocultar el texto
    }

}
