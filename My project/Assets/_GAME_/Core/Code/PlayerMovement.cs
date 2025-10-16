using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    public int maxHealth = 4;
    public int currentHealth;
    public int maxWater = 10;
    public int currentWater;
public WaterBar waterBar;

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
         currentHealth = maxHealth;
          currentWater = 0;
        if (waterBar != null)
        {
            waterBar.SetMaxWater(maxWater);
            waterBar.SetWater(currentWater);
        }
     else
    {
        Debug.LogWarning("WaterBar no asignado en Player");
    }
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
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

  void OnRefill(InputAction.CallbackContext context)
{
    if (context.performed && isNearWater)
    {
        AddWater(1);
    }
}

    void AddWater(int amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);

    if (waterBar != null)
    {
        waterBar.SetWater(currentWater);
        Debug.Log($"Water refilled: {currentWater}/{maxWater}");
    }
    else
    {
        Debug.LogWarning("WaterBar no asignado en PlayerMovement");
    }
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
