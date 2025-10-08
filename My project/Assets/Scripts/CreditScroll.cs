using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    public RectTransform content;
    public float speed = 20f;

    // Update is called once per frame
    void Update()
    {
        content.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (content.anchoredPosition.y >= content.rect.height)
        {
            content.anchoredPosition = new Vector2(0, -content.rect.height);
        }

    }
    
    public void ResetScroll()
    {
        // AnchoredPosition con pivot arriba
        content.anchoredPosition = new Vector2(0, 0);
    }
}