using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using UnityEditor;


public class InstructionController : MonoBehaviour
{
    [SerializeField] Text txtMessage;
    [SerializeField] float duration;
    [SerializeField] Text txtControls;
    [SerializeField] Text txtTitulControls;
    [SerializeField] LocalizeStringEvent localizedStringEvent;
    [SerializeField] LocalizeStringEvent[] localizedTexts;


    Color startColor;
    Color endColor;

    void Start()
    {
        LoadLanguage();
        startColor = new Color(67 / 255.0f, 34 / 255.0f, 103 / 255.0f);
        endColor = Color.white;
        StartCoroutine("ChangeColor");

#if UNITY_ANDROID
        localizedStringEvent.StringReference.SetReference("Prueba", "Empezar_Android");

        // txtMessage.text = "TAP TO START";
        txtControls.gameObject.SetActive(false);
        txtTitulControls.gameObject.SetActive(false);
#else
        localizedStringEvent.StringReference.SetReference("Prueba", "Empezar_PC");
        txtControls.gameObject.SetActive(true);
        txtTitulControls.gameObject.SetActive(true);
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

    private void UpdateLocalizedTexts()
    {
        if (localizedStringEvent != null && localizedStringEvent.StringReference != null)
        {
            txtMessage.text = localizedStringEvent.StringReference.GetLocalizedString();
        }
        foreach (var textEvent in localizedTexts)
        {
            textEvent.RefreshString();
        }
    }
    IEnumerator ChangeColor()
    {
        while (true)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                txtMessage.color = Color.Lerp(startColor, endColor, t / duration);
                yield return null;
            }

            //intercambiamos los colores
            Color temColor = startColor;
            startColor = endColor;
            endColor = temColor;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Nivel1");
        }
        // Detectar toque en la pantalla del móvil
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                SceneManager.LoadScene("Nivel1");
            }
        }

    }
}