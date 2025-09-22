using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TMP_Text buttonText;
    private Color normalColor = Color.white;
    private Color hoverColor = new Color32(125, 137, 59, 255); // #7D893B
    private Color clickColor = new Color32(125, 137, 59, 255);

    void Awake()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = clickColor;
    }
}
