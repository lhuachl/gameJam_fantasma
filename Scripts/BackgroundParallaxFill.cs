using UnityEngine;

// Adjunta este script a tus prefabs de fondo.
// - Si el fondo tiene SpriteRenderer y 'infiniteX' está activo, creará 3 tiles (izq, centro, der) y los reutilizará
//   para que el fondo nunca termine en X.
// - También puedes activar parallax con 'parallaxFactor'.
// - Si no hay SpriteRenderer, hará parallax por offset de material (requiere textura con Wrap Mode = Repeat).
public class BackgroundParallaxFill : MonoBehaviour
{
    [Header("Parallax")]
    [Tooltip("0 = pegado a la cámara, 1 = no se mueve")] 
    [Range(0f, 1f)] public float parallaxFactor = 0.2f;
    public bool followCameraX = true;
    public bool followCameraY = false;

    [Header("Tiling infinito")]
    public bool infiniteX = true;
    public float extraWidthFactor = 1.2f; // multiplica el ancho base para cubrir un poco más

    private Transform cam;
    private Vector3 camStartPos;

    // Para SpriteRenderer tiling
    private SpriteRenderer sr;
    private Transform leftTile, centerTile, rightTile;
    private float tileWorldWidth;

    // Para material offset
    private Renderer rend;
    private Material matInstance;
    private int texProp = -1; // _BaseMap(URP) o _MainTex(Built-in)

    void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;
        if (cam == null)
        {
            var camGo = GameObject.FindWithTag("MainCamera");
            cam = camGo ? camGo.transform : null;
        }
        if (cam == null)
        {
            enabled = false;
            return;
        }
        camStartPos = cam.position;

        sr = GetComponent<SpriteRenderer>();
        rend = GetComponent<Renderer>();

        // Preparar material para offset si no usamos SpriteRenderer o queremos sólo offset
        if (rend != null && !(sr != null && infiniteX))
        {
            matInstance = rend.material;
            if (matInstance != null)
            {
                if (matInstance.HasProperty("_BaseMap")) texProp = Shader.PropertyToID("_BaseMap");
                else if (matInstance.HasProperty("_MainTex")) texProp = Shader.PropertyToID("_MainTex");
            }
        }

        if (sr != null && infiniteX)
        {
            // Crear 3 tiles
            centerTile = this.transform;
            tileWorldWidth = sr.bounds.size.x * extraWidthFactor;

            // Instanciar izquierda y derecha como copias hija del mismo padre
            leftTile = new GameObject(name + "_L").transform;
            rightTile = new GameObject(name + "_R").transform;
            leftTile.SetParent(transform.parent, false);
            rightTile.SetParent(transform.parent, false);

            // Añadir SR a clones
            var srL = leftTile.gameObject.AddComponent<SpriteRenderer>();
            var srR = rightTile.gameObject.AddComponent<SpriteRenderer>();
            srL.sprite = sr.sprite; srR.sprite = sr.sprite;
            srL.color = sr.color; srR.color = sr.color;
            srL.sortingLayerID = sr.sortingLayerID; srR.sortingLayerID = sr.sortingLayerID;
            srL.sortingOrder = sr.sortingOrder; srR.sortingOrder = sr.sortingOrder;

            // Posicionar tiles
            Vector3 cpos = centerTile.position;
            leftTile.position = new Vector3(cpos.x - tileWorldWidth, cpos.y, cpos.z);
            rightTile.position = new Vector3(cpos.x + tileWorldWidth, cpos.y, cpos.z);
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Parallax por posición
        Vector3 camDelta = cam.position - camStartPos;
        float targetX = transform.position.x;
        float targetY = transform.position.y;
        if (followCameraX) targetX = cam.position.x;
        if (followCameraY) targetY = cam.position.y;

        float px = camDelta.x * (1f - parallaxFactor);
        float py = camDelta.y * (1f - parallaxFactor);

        // Si tenemos tiles infinitos, sólo movemos el tile central y reciclamos
        if (sr != null && infiniteX && centerTile != null && leftTile != null && rightTile != null)
        {
            centerTile.position = new Vector3(targetX, targetY, centerTile.position.z);
            leftTile.position = new Vector3(centerTile.position.x - tileWorldWidth, targetY, centerTile.position.z);
            rightTile.position = new Vector3(centerTile.position.x + tileWorldWidth, targetY, centerTile.position.z);

            // Reciclaje cuando la cámara cruza el tile derecho o izquierdo
            if (cam.position.x > rightTile.position.x)
            {
                // Mover izquierda -> derecha al final
                var tmp = leftTile; leftTile = centerTile; centerTile = rightTile; rightTile = tmp;
                rightTile.position = new Vector3(centerTile.position.x + tileWorldWidth, targetY, centerTile.position.z);
            }
            else if (cam.position.x < leftTile.position.x)
            {
                // Mover derecha -> izquierda al inicio
                var tmp = rightTile; rightTile = centerTile; centerTile = leftTile; leftTile = tmp;
                leftTile.position = new Vector3(centerTile.position.x - tileWorldWidth, targetY, centerTile.position.z);
            }
        }
        else if (matInstance != null && texProp != -1)
        {
            // Parallax por offset de material (infinito si la textura es repeat)
            Vector2 off = matInstance.GetTextureOffset(texProp);
            off.x = px * 0.01f; // escala suave
            off.y = py * 0.01f;
            matInstance.SetTextureOffset(texProp, off);

            // También opcionalmente seguir cámara en X/Y para que se mantenga centrado
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
        else
        {
            // Fallback: sólo seguir la cámara con un factor de parallax (sin loops)
            transform.position = new Vector3(targetX - px, targetY - py, transform.position.z);
        }
    }
}
