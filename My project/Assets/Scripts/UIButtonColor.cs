using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    private TMP_Text buttonText;
    private Color normalColor = Color.white;
    private Color hoverColor = new Color32(125, 137, 59, 255); // #7D893B
    private Color clickColor = new Color32(125, 137, 59, 255);

    void Awake()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
            buttonText.color = normalColor;
    }

    // Mouse Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }

    // Mouse Click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = clickColor;
    }

    // Teclado / Gamepad selección
    public void OnSelect(BaseEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = hoverColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }
}
