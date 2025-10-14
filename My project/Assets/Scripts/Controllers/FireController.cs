using System.Collections;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private float duration = 10f;
    private Animator animator;
    private bool isBurning = false;

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

        // Ocultar sprite base
        if (baseSprite != null) baseSprite.SetActive(false);
        // Ocultar sprite quemado
        if (burnedSprite != null) burnedSprite.SetActive(true);

        animator.Play("Fire_Start");
    }

    public void Extinguish(bool burned)
    {
        if (!isBurning) return;
        isBurning = false;

        animator.Play("Fire_End");

        // Ejecutar el cambio visual luego de la animación
        StartCoroutine(WaitAndHandlePostFire(burned, duration));
    }

    private IEnumerator WaitAndHandlePostFire(bool burned, float delay)
    {
        yield return new WaitForSeconds(delay);
        HandlePostFire(burned);
    }
    
    private void HandlePostFire(bool burned)
    {
        gameObject.SetActive(false);
        
        if (burned)
        {
            // Mostrar sprite quemado
            if (burnedSprite != null) burnedSprite.SetActive(true);
        }
        else
        {
            // Restaurar sprite base
            if (baseSprite != null) baseSprite.SetActive(true);
            if (burnedSprite != null) burnedSprite.SetActive(false);
        }
    }
}
