using UnityEngine;
using Firebase.Extensions;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] InputField emailInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Text feedbackText;
    [SerializeField] LocalizeStringEvent localizedStringEventMessage;
    [SerializeField] LocalizeStringEvent localizedStringEventPassword;
    void Start()
    {
        LoadLanguage();
        HideFeedback();
        // Suscribirse a los eventos OnValueChanged de los InputFields
        emailInput.onValueChanged.AddListener(delegate { HideFeedback(); });
        passwordInput.onValueChanged.AddListener(delegate { HideFeedback(); });
    }
    private void LoadLanguage()
    {
        // Cargar el idioma guardado
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
            localizedStringEventPassword.RefreshString();
        }
    }

    public async Task<bool> AuthenticateUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        try
        {
            // Intentar crear un nuevo usuario
            await FirebaseManager.Instance.Auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log("Usuario registrado y autenticado correctamente.");
            return true;
        }
        catch (FirebaseException ex)
        {
            if (ex.ErrorCode == (int)AuthError.EmailAlreadyInUse)
            {
                try
                {
                    // Intentar iniciar sesión con credenciales existentes
                    await FirebaseManager.Instance.Auth.SignInWithEmailAndPasswordAsync(email, password);
                    Debug.Log("Inicio de sesión exitoso con el correo existente.");
                    return true;
                }
                catch (FirebaseException signInEx)
                {
                    if (signInEx.ErrorCode == (int)AuthError.WrongPassword)
                    {
                        Debug.Log("Error: Contraseña incorrecta.");
                        localizedStringEventMessage.StringReference.SetReference("Prueba", "ContraseñaErronea");
                        localizedStringEventMessage.RefreshString();
                        ShowFeedback();
                        await Task.Delay(100);
                        passwordInput.ActivateInputField();
                    }
                    return false;
                }
            }
            else if (ex.ErrorCode == (int)AuthError.InvalidEmail)
            {
                localizedStringEventMessage.StringReference.SetReference("Prueba", "EmailErroneo");
                localizedStringEventMessage.RefreshString();
                ShowFeedback();
                await Task.Delay(100);
                emailInput.ActivateInputField();
            }
            return false;
        }
    }

    private void ShowFeedback()
    {
        if (!feedbackText.gameObject.activeSelf)
        {
            feedbackText.gameObject.SetActive(true);
        }
    }
    private void HideFeedback()
    {
        if (feedbackText.gameObject.activeSelf)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }
}