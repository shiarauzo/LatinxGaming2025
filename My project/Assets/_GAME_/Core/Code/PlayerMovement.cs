using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public InstructionsUIController instructionsUI;
    [SerializeField] private float moveSpeed = 5f;
    public int maxHealth = 4;
    public int currentHealth;
    public int maxWater = 10;
    public int currentWater;

    public WaterBar waterBar;

    public float footstepsSpeed = 0.5f;

    [Header("Fire Detection")]
    public float fireDetectionRadius = 0.5f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool isNearWater = false;
    private bool isNearFire = false;
    private bool playingFootSteps = false;
    private PlayerControls controls;
    
    private string currentFootstepType = "FootstepsGrass";

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Refill.performed += OnRefill;
        controls.Player.Refill.canceled += OnRefillCanceled;
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Refill.performed -= OnRefill;
            controls.Player.Refill.canceled -= OnRefillCanceled;
            controls.Disable();
        }
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
        animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0);

        // StartFootSteps
        if (rb.linearVelocity.magnitude > 0.01f && !playingFootSteps)
            StartFootSteps(); //  Debug.Log("IS WALKING");
        else if (rb.linearVelocity.magnitude <= 0.01f && playingFootSteps)
            StopFootSteps(); //  Debug.Log("STOPPED WALKING");

        CheckFireNearby();
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
            SoundEffectManager.PlayLongSFX("CollectWater");
        }
    }

    void AddWater(int amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);

        if (waterBar != null)
        {
            waterBar.SetWater(currentWater);
         //   Debug.Log($"Water refilled: {currentWater}/{maxWater}");
        }
        else
        {
            Debug.LogWarning("WaterBar no asignado en PlayerMovement");
        }
    }

    private void OnRefillCanceled(InputAction.CallbackContext context)
    {
        SoundEffectManager.StopLongSFX("CollectWater");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = true;
            instructionsUI?.ShowInstructions("REFILL_WATER");
         //   Debug.Log("Chocaste con el agua. presiona R para recolectar.");
        }

        if (other.CompareTag("Plant"))
        {
            currentFootstepType = "FootstepsDirt";
            if (playingFootSteps)
            {
                SoundEffectManager.PlayLongSFX(currentFootstepType);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = false;
            instructionsUI?.CloseInstructions();
          //  Debug.Log("Saliste del área de agua.");
        }

        if (other.CompareTag("Plant"))
        {
            currentFootstepType = "FootstepsGrass";
            if (playingFootSteps)
            {
                SoundEffectManager.StopLongSFX(currentFootstepType);
            }
        }
    }


    // Flood: inundar, apagar fuego
    public void OnFlood(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Detectar planta
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        bool extinguishedAny = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Plant"))
            {
                FireController[] fires = hit.GetComponentsInChildren<FireController>();
                foreach (var fire in fires)
                {
                    if (fire.isActiveAndEnabled && fire.IsBurning)
                    {
                        if (currentWater > 0)
                        {
                            fire.Extinguish(false, null);
                            AddWater(-1);
                            extinguishedAny = true;
                            Debug.Log("🔥 Fuego apagado en " + fire.name);
                            SoundEffectManager.Play("WaterPlant");
                        }
                        else
                        {
                            StartCoroutine(ShowTemporaryInstruction("NO_WATER", 3f));
                        }
                    }
                }
            }
        }
        if (!extinguishedAny)
            Debug.Log("No hay plantas quemándose cerca.");
    }

    // Plantar semillas
    public void OnPlantSeeds(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
    }
    void StartFootSteps()
    {
        if (!playingFootSteps)
        {
            playingFootSteps = true;
            SoundEffectManager.PlayLongSFX("FootstepsGrass");
        }        
    }

    void StopFootSteps()
    {
        if (playingFootSteps)
        {
            SoundEffectManager.StopLongSFX(currentFootstepType);
            playingFootSteps = false;
        }
    }

    public void CheckFireNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, fireDetectionRadius);
        bool foundFire = false;

        foreach (var hit in hits)
        {
            FireController[] fires = hit.GetComponentsInChildren<FireController>();
            foreach (var fire in fires)
            {
                if (fire.isActiveAndEnabled && fire.IsBurning)
                {
                    foundFire = true;
                    break;
                }
            }

            if (foundFire) break;
        }

        if (foundFire && !isNearFire && currentWater > 0)
        {
            isNearFire = true;
            instructionsUI?.ShowInstructions("EXTINGUISH_FIRE");
        }
        else if ((!foundFire || currentWater == 0) && isNearFire)
        {
            isNearFire = false;
            instructionsUI?.CloseInstructions();
        }
    }
    private IEnumerator ShowTemporaryInstruction(string key, float duration)
    {
        if (instructionsUI != null)
            instructionsUI.ShowInstructions(key);

        yield return new WaitForSeconds(duration);

        if (instructionsUI != null)
            instructionsUI.CloseInstructions();
    }

}
