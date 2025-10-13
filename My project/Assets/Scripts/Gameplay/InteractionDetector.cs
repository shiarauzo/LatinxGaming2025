using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; //Closest Interactable
    public GameObject interactionIcon;

    void Start()
    {
        var pi = GetComponentInParent<PlayerInput>();
        if (pi != null)
        {
            Debug.Log("Mapa activo: " + pi.currentActionMap.name);
        }


        interactionIcon.SetActive(false);
    }


    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("La tecla E fue presionada");
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("on interact was called" + context);
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if it has an IInteractable script 
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
