using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    public int maxHealth = 4;
    public int currentHealth; 
    public int maxWater = 10;
    public int currentWater; 

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    private bool isNearWater = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        if (isNearWater && Keyboard.current.rKey.wasPressedThisFrame)
        {
            AddWater(1);
        }

        Debug.Log($"INPUT: {moveInput}  |  VELOCITY: {rb.linearVelocity}  |  POS: {rb.position}");
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

  
    void AddWater(int amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);
        Debug.Log($"Agua actual: {currentWater}/{maxWater}");
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = true;
            Debug.Log("Cerca del agua: presiona R para recolectar.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = false;
            Debug.Log("fuera del agua");
        }
    }
}
