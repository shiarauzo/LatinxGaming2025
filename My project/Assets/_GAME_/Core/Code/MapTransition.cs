using Unity.Cinemachine;
using System.Collections;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] PolygonCollider2D mapBoundry;
    [SerializeField] Direction direction;
    [SerializeField] float additivePos = 2f;
    
    [Header("Debug")]
    [SerializeField] bool showDebug = true;
    [SerializeField] bool enableCameraDebug = true;
    
    CinemachineCamera virtualCamera;
    CinemachineConfiner2D confiner;
    Transform player;
    
    static bool isTransitioning = false;
    
    enum Direction { Up, Down, Left, Right }

    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineCamera>();
        
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        }
        
        if (mapBoundry == null)
        {
            Debug.LogError("✗ [" + gameObject.name + "] MapBoundry NO está asignado");
        }
        
    
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
    
        if (enableCameraDebug && confiner != null && confiner.BoundingShape2D != null && player != null)
        {
            Debug.Log($"Límite: {confiner.BoundingShape2D.name} | Player: {player.position} | Cam: {virtualCamera.transform.position}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (showDebug) Debug.Log("*** [" + gameObject.name + "] Transición iniciada");
            StartCoroutine(TransitionMap(collision.gameObject));
        }
    }

    private IEnumerator TransitionMap(GameObject player)
    {
        isTransitioning = true;
        
       
        if (confiner != null && mapBoundry != null)
        {
            confiner.BoundingShape2D = mapBoundry;
            confiner.InvalidateBoundingShapeCache();
            if (showDebug) Debug.Log("Límites cambiados a: " + mapBoundry.name);
        }
        
     
        yield return null;
        
     
        Vector3 oldPos = player.transform.position;
        UpdatePlayerPosition(player);
        if (showDebug) Debug.Log($"Jugador movido de {oldPos} a {player.transform.position}");
        
      
        yield return new WaitForSeconds(1f);
        
        isTransitioning = false;
        if (showDebug) Debug.Log("✓ Transición completada");
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;
        
        switch (direction)
        {
            case Direction.Up:
                newPos.y += additivePos;
                break;
            case Direction.Down:
                newPos.y -= additivePos;
                break;
            case Direction.Left:
                newPos.x -= additivePos;
                break;
            case Direction.Right:
                newPos.x += additivePos;
                break;
        }
        
        player.transform.position = newPos;
    }
}