using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class TilemapSetup : MonoBehaviour
{
    void Awake()
    {
        // Si no existe la capa "Ground", avisa
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
            Debug.LogWarning("⚠️ No existe la capa 'Ground'. Créala en Project Settings > Tags and Layers.");
        }
        else
        {
            // Asigna la capa Ground al Tilemap
            gameObject.layer = groundLayer;
            Debug.Log("✅ Tilemap asignado a la capa 'Ground'.");
        }

        // Configurar Rigidbody2D
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;
        rb.interpolation = RigidbodyInterpolation2D.None;

        // Configurar TilemapCollider2D
        var tilemapCollider = GetComponent<TilemapCollider2D>();
        tilemapCollider.usedByComposite = true;
        tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;

        // Configurar CompositeCollider2D
        var composite = GetComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
        composite.generationType = CompositeCollider2D.GenerationType.Synchronous;
        composite.vertexDistance = 0.0005f;
        composite.edgeRadius = 0f;

        Debug.Log("✅ Tilemap configurado correctamente con Composite Collider y capa Ground.");
    }
}
