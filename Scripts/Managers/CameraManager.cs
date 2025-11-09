using UnityEngine;

/// <summary>
/// Camera que sigue al jugador con offset configurable
/// </summary>
public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    [SerializeField] private float smoothSpeed = 0.1f;
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    
    private Vector3 velocity = Vector3.zero;
    private float initialZ;

    private void Start()
    {
        initialZ = transform.position.z;
        
        // Buscar jugador si no está asignado
        if (target == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
                target = player.transform;
        }

        if (target == null)
            Debug.LogWarning("[CameraManager] No se encontró target");
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        if (lockX)
            desiredPosition.x = transform.position.x;
        if (lockY)
            desiredPosition.y = transform.position.y;

        desiredPosition.z = initialZ;

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref velocity, 
            smoothSpeed
        );
    }
}
