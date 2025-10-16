using System.Collections;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private float duration = 20f;
    private Animator animator;
    private bool isBurning = false;
    public bool IsBurning => isBurning; // getter

    [Header("References")]
    public GameObject baseSprite;
    public GameObject burnedSprite;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (baseSprite == null)
            baseSprite = transform.parent.Find("BaseSprite")?.gameObject;
        if (burnedSprite == null)
            burnedSprite = transform.parent.Find("BurnedSprite")?.gameObject;
    }

    public void Ignite()
    {
        if (isBurning) return;
        isBurning = true;
        gameObject.SetActive(true);

        // Ocultar sprite quemado
        if (burnedSprite != null) burnedSprite.SetActive(false);

        animator.Play("Fire_Start");
    }

    public void Extinguish(bool burned, System.Action onFinished = null)
    {
        if (!isBurning) return;
        isBurning = false;

        animator.Play("Fire_End");

        float animLength = GetAnimationLength("Fire_End");
        // Ejecutar el cambio visual luego de la animación
        StartCoroutine(WaitAndHandlePostFire(burned, animLength, onFinished));
    }
    
    private float GetAnimationLength(string animName)
    {
        if (animator.runtimeAnimatorController == null) return 1f;

        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        foreach (var clip in rac.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 0f;
    }

    private IEnumerator WaitAndHandlePostFire(bool burned, float delay, System.Action onFinished)
    {
        yield return new WaitForSeconds(delay);
        HandlePostFire(burned);
        onFinished?.Invoke();
    }

    private void HandlePostFire(bool burned)
    {
        if (burned)
        {
            // Mostrar sprite quemado
            if (baseSprite != null) baseSprite.SetActive(false);
            if (burnedSprite != null) burnedSprite.SetActive(true);
            Debug.Log($"🔥 {name} completó su animación y quedó quemada.");
        }
        else
        {
            // Restaurar sprite base
            if (baseSprite != null) baseSprite.SetActive(true);
            if (burnedSprite != null) burnedSprite.SetActive(false);
        }
        
        // Solo oculta visualmente el fuego, mantiene el collider activo
        if (TryGetComponent(out SpriteRenderer fireVFX))
            fireVFX.enabled = false;

        // Mantener el collider padre del fuego activo para evitar re-ignición inmediata
        if (TryGetComponent(out Collider2D col2D))
            col2D.enabled = true;
        else if (TryGetComponent(out Collider col))
            col.enabled = true;    
    }
}
