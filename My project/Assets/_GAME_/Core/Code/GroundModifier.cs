using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GroundModifier : MonoBehaviour
{
    [Header("UI")]
    public Button palaButton; 

    [Header("Tilemap")]
    public Tilemap groundTilemap;  
    public TileBase tierraTile;   
    public TileBase pastoTile;     

    private bool modoPala = false;

    void Start()
    {
        if (palaButton != null)
            palaButton.onClick.AddListener(ActivarModoPala);
        else
            Debug.LogWarning("⚠️ No se asignó el botón de la pala en el Inspector.");
    }

    void Update()
    {
       
        if (modoPala && Input.GetMouseButtonDown(0))
        {
            
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = groundTilemap.WorldToCell(mouseWorldPos);

            
            TileBase tileActual = groundTilemap.GetTile(cellPos);
            if (tileActual != null)
            {
                groundTilemap.SetTile(cellPos, tierraTile);
                Debug.Log("🌾 Tile cambiado a tierra en: " + cellPos);
            }
        }
    }

    void ActivarModoPala()
    {
        modoPala = !modoPala;
        palaButton.image.color = modoPala ? Color.yellow : Color.white; 
        Debug.Log(modoPala ? " Modo pala activado" : " Modo pala desactivado");
    }
}
