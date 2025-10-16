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

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Refill.performed += OnRefill;
    }

    void OnDisable()
    {
        controls.Player.Refill.performed -= OnRefill;
        controls.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
     //   Debug.Log($"INPUT: {moveInput}  |  VELOCITY: {rb.linearVelocity}  |  POS: {rb.position}");
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    private void OnRefill(InputAction.CallbackContext context)
    {
        if (isNearWater)
        {
            AddWater(1);
            Debug.Log($"Agua añadida. Ahora: {currentWater}/{maxWater}");
        }
        else
        {
            Debug.Log("No estás cerca del agua.");
        }
    }

    void AddWater(int amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = true;
            Debug.Log("Chocaste con el agua. presiona R para recolectar.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = false;
            Debug.Log("Saliste del área de agua.");
        }
    }
}
