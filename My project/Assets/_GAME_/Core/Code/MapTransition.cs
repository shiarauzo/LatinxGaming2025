using Unity.Cinemachine;
using System.Collections;
using UnityEngine;
using System;

public class MapTransition : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] PolygonCollider2D mapBoundry;
    [SerializeField] Direction direction;
    [SerializeField] float additivePos = 2f;
    
    [Header("Debug")]
    [SerializeField] bool showDebug = true;
    [SerializeField] bool enableCameraDebug = true;
    
    //CinemachineCamera virtualCamera;//>
    CinemachineConfiner2D confiner;
    //Transform player;//.
    
    //static bool isTransitioning = false;
    
    enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // if (isTransitioning) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundry;
            if (showDebug) Debug.Log("*** [" + gameObject.name + "] Transición iniciada");
           // StartCoroutine(TransitionMap(collision.gameObject));
           UpdatePlayerPosition(collision.gameObject);
        }
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