using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NeonTextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("⚡ Neon Base Settings")]
    public TextMeshProUGUI text;

    [Tooltip("Color 1 del parpadeo (Rosado pastel)")]
    public Color colorA = new Color(1f, 0.72f, 0.85f); // #FFB8D9 aprox.
    [Tooltip("Color 2 del parpadeo (Celeste pastel)")]
    public Color colorB = new Color(0.70f, 0.90f, 1f); // #B3E6FF aprox.

    [Range(0.1f, 10f)]
    public float blinkSpeed = 0.8f; // Parpadeo suave y lento
    public float transitionSmoothness = 2.5f; // Transición fluida

    [Header("🎮 Interaction Colors")]
    public Color hoverColor = new Color(0.8f, 1f, 1f);   // Cian claro al pasar el mouse
    public Color pressedColor = new Color(1f, 0.6f, 0.8f); // Rosado más fuerte al presionar

    private Color currentColor;
    private bool isHovered;
    private bool isPressed;

    void Start()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();

        currentColor = colorA;
    }

    void Update()
    {
        Color targetColor;

        // Hover / Press override
        if (isPressed)
        {
            targetColor = pressedColor;
        }
        else if (isHovered)
        {
            targetColor = hoverColor;
        }
        else
        {
            // Pulso suave entre rosado y celeste
            float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;
            targetColor = Color.Lerp(colorA, colorB, t);
        }

        // Transición suave
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * transitionSmoothness);
        text.color = currentColor;
    }

    // Eventos UI
    public void OnPointerEnter(PointerEventData eventData) => isHovered = true;
    public void OnPointerExit(PointerEventData eventData) => isHovered = false;
    public void OnPointerDown(PointerEventData eventData) => isPressed = true;
    public void OnPointerUp(PointerEventData eventData) => isPressed = false;
}
