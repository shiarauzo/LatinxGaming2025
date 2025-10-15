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

 void Start()
{
    rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
}

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
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
    currentHealth -=damage;
    }
}
