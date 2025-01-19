using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteEffects : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Suponiendo que el color original es el "encendido"
        spriteRenderer.color = Color.black; // Color inicial "apagado"
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        spriteRenderer.color = originalColor; // Cambiar al color original cuando el cursor está sobre el sprite
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spriteRenderer.color = Color.black; // Volver al color "apagado" cuando el cursor sale del sprite
    }
}