using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

 void Start()
{
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponentInChildren<Animator>();
}


  void FixedUpdate()
{

    rb.linearVelocity = moveInput * moveSpeed;
    
    if(moveInput != Vector2.zero)
    {
       // Debug.Log($"INPUT: {moveInput}  |  VELOCITY: {rb.linearVelocity}  |  POS: {rb.position}");
    }

  // Debug.Log($"INPUT: {moveInput}  |  VELOCITY: {rb.linearVelocity}  |  POS: {rb.position}");
}


    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (animator)
        {
            if (moveInput != Vector2.zero)
            {
              
                animator.SetBool("isWalking", true);
                animator.SetFloat("InputX", moveInput.x);
                animator.SetFloat("InputY", moveInput.y);

              
                animator.SetFloat("LastInputX", moveInput.x);
                animator.SetFloat("LastInputY", moveInput.y);
            }
            else
            {
                // Idle
                animator.SetBool("isWalking", false);
            }
        }
    }
}
