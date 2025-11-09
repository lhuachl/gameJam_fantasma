using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    private float lengthX, startPosX, startPosY;
    public GameObject cam;
    [Range(0f, 1f)] public float parallaxX = 0.5f;
    [Range(0f, 1f)] public float parallaxY = 0f;
    public bool useHorizontal = true;
    public bool useVertical = false;
    public bool repeatX = true; // para scrolling infinito en X si el sprite es tileable
    private SpriteRenderer sr;

    void Start()
    {
        if (cam == null)
        {
            if (Camera.main != null)
            {
                cam = Camera.main.gameObject;
            }
            else
            {
                Debug.LogError("ParalaxController: La cámara (cam) no está asignada y no se encontró Camera.main.");
                enabled = false;
                return;
            }
        }
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        if (repeatX)
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                // Si no hay SpriteRenderer, desactivar repetición horizontal y continuar sin error
                repeatX = false;
            }
            else
            {
                lengthX = sr.bounds.size.x;
            }
        }
    }

    void LateUpdate()
    {
        if (cam == null) return; // No hacer nada si la cámara no está asignada

        Vector3 camPos = cam.transform.position;
        float newX = transform.position.x;
        float newY = transform.position.y;

        if (useHorizontal)
        {
            float tempX = camPos.x * (1 - parallaxX);
            float distX = camPos.x * parallaxX;
            newX = startPosX + distX;

            if (repeatX && sr != null)
            {
                if (tempX > startPosX + lengthX) startPosX += lengthX;
                else if (tempX < startPosX - lengthX) startPosX -= lengthX;
            }
        }

        if (useVertical)
        {
            float distY = camPos.y * parallaxY;
            newY = startPosY + distY;
        }

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}