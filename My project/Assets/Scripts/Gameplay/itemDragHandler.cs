using UnityEngine;
using UnityEngine.EventSystems;

public class itemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Called when a drag begins. Add initialization logic here if needed.
        originalParent = transform.parent;
        transform.SetParent(transform.root); //Above other canvas
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Called while dragging. Add drag handling logic here if needed.
        transform.position = eventData.position; //Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Called when the drag ends. Add cleanup or drop logic here if needed.
        canvasGroup.blocksRaycasts = true; //Enable raycasts
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); // Slot where item dropped
        if (dropSlot == null)
        {
            GameObject foundItem = eventData.pointerEnter;
            if (foundItem != null)
            {
                dropSlot = foundItem.GetComponentInParent<Slot>();
            }
        }
        
        Slot originalSlot = originalParent.GetComponent<Slot>();
        if (dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                //Slot has an item - swap
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // Move item into srop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            //No slot under drop point
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
