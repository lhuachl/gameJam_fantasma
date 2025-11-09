using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestor centralizado de fondos con efecto parallax.
/// Soporta resoluciones 1920x1080 y 960x540 (mitad).
/// Gestiona múltiples capas de fondo con diferentes velocidades de parallax.
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        [Tooltip("GameObject del fondo (debe tener SpriteRenderer o Renderer)")]
        public GameObject backgroundObject;
        
        [Tooltip("Factor de parallax (0 = pegado a cámara, 1 = no se mueve)")]
        [Range(0f, 1f)]
        public float parallaxFactor = 0.5f;
        
        [Tooltip("Profundidad Z del fondo (más negativo = más atrás)")]
        public float zDepth = -10f;
        
        [Tooltip("Habilitar tiling infinito horizontal")]
        public bool infiniteTilingX = true;
        
        [Tooltip("Seguir cámara en Y")]
        public bool followCameraY = false;
        
        // Referencia al controlador creado
        [HideInInspector]
        public BackgroundParallaxFill controller;
    }

    [Header("Configuración de Capas")]
    [SerializeField] private List<ParallaxLayer> layers = new List<ParallaxLayer>();
    
    [Header("Configuración de Resolución")]
    [Tooltip("Resolución objetivo: 1920x1080 o 960x540")]
    [SerializeField] private Vector2 targetResolution = new Vector2(1920, 1080);
    
    [Tooltip("Escalar fondos automáticamente según resolución de pantalla")]
    [SerializeField] private bool autoScaleToScreen = true;
    
    [Header("Referencia de Cámara")]
    [SerializeField] private Camera mainCamera;
    
    private Vector2 baseResolution1080p = new Vector2(1920, 1080);
    private Vector2 baseResolution540p = new Vector2(960, 540);

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("BackgroundManager: No se encontró la cámara principal.");
                return;
            }
        }
        
        InitializeLayers();
        
        if (autoScaleToScreen)
        {
            ScaleBackgroundsToScreen();
        }
    }

    /// <summary>
    /// Inicializa todas las capas de parallax
    /// </summary>
    private void InitializeLayers()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];
            
            if (layer.backgroundObject == null)
            {
                Debug.LogWarning($"BackgroundManager: Layer {i} no tiene GameObject asignado.");
                continue;
            }
            
            // Configurar posición Z
            Vector3 pos = layer.backgroundObject.transform.position;
            pos.z = layer.zDepth;
            layer.backgroundObject.transform.position = pos;
            
            // Verificar si ya tiene un controlador de parallax
            layer.controller = layer.backgroundObject.GetComponent<BackgroundParallaxFill>();
            
            if (layer.controller == null)
            {
                // Añadir controlador si no existe
                layer.controller = layer.backgroundObject.AddComponent<BackgroundParallaxFill>();
            }
            
            // Configurar el controlador
            ConfigureParallaxController(layer);
        }
        
        Debug.Log($"BackgroundManager: {layers.Count} capas de parallax inicializadas.");
    }

    /// <summary>
    /// Configura un controlador de parallax según los parámetros de la capa
    /// </summary>
    private void ConfigureParallaxController(ParallaxLayer layer)
    {
        if (layer.controller == null) return;
        
        // Usar reflexión para configurar propiedades públicas
        var controllerType = layer.controller.GetType();
        
        // parallaxFactor
        var parallaxFactorField = controllerType.GetField("parallaxFactor");
        if (parallaxFactorField != null)
        {
            parallaxFactorField.SetValue(layer.controller, layer.parallaxFactor);
        }
        
        // followCameraX (siempre true para parallax horizontal)
        var followCameraXField = controllerType.GetField("followCameraX");
        if (followCameraXField != null)
        {
            followCameraXField.SetValue(layer.controller, true);
        }
        
        // followCameraY
        var followCameraYField = controllerType.GetField("followCameraY");
        if (followCameraYField != null)
        {
            followCameraYField.SetValue(layer.controller, layer.followCameraY);
        }
        
        // infiniteX
        var infiniteXField = controllerType.GetField("infiniteX");
        if (infiniteXField != null)
        {
            infiniteXField.SetValue(layer.controller, layer.infiniteTilingX);
        }
    }

    /// <summary>
    /// Escala los fondos según la resolución actual de la pantalla
    /// </summary>
    private void ScaleBackgroundsToScreen()
    {
        if (mainCamera == null) return;
        
        // Calcular el tamaño visible de la cámara en unidades del mundo
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        // Calcular el factor de escala basado en la resolución objetivo
        float scaleFactorX = cameraWidth / (targetResolution.x / 100f); // Ajuste empírico
        float scaleFactorY = cameraHeight / (targetResolution.y / 100f);
        
        // Determinar si usamos resolución 1080p o 540p
        bool isHalfResolution = Mathf.Approximately(targetResolution.x, baseResolution540p.x) &&
                                Mathf.Approximately(targetResolution.y, baseResolution540p.y);
        
        // Ajustar escala para cada capa
        foreach (var layer in layers)
        {
            if (layer.backgroundObject == null) continue;
            
            var spriteRenderer = layer.backgroundObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                // Calcular escala necesaria para cubrir la pantalla
                Sprite sprite = spriteRenderer.sprite;
                float spriteWidth = sprite.bounds.size.x;
                float spriteHeight = sprite.bounds.size.y;
                
                // Escalar para que cubra al menos el ancho de la cámara
                float scaleX = cameraWidth / spriteWidth;
                float scaleY = cameraHeight / spriteHeight;
                
                // Usar la escala mayor para asegurar cobertura completa
                float finalScale = Mathf.Max(scaleX, scaleY);
                
                // Aplicar factor adicional para resolución mitad si es necesario
                if (isHalfResolution)
                {
                    finalScale *= 0.5f;
                }
                
                Vector3 scale = layer.backgroundObject.transform.localScale;
                scale.x = finalScale;
                scale.y = finalScale;
                layer.backgroundObject.transform.localScale = scale;
                
                Debug.Log($"BackgroundManager: Capa '{layer.backgroundObject.name}' escalada a {finalScale:F2}x para resolución {targetResolution}");
            }
        }
    }

    /// <summary>
    /// Añade una nueva capa de parallax en tiempo de ejecución
    /// </summary>
    public void AddLayer(GameObject backgroundObj, float parallaxFactor, float zDepth, bool infiniteTiling = true)
    {
        if (backgroundObj == null)
        {
            Debug.LogWarning("BackgroundManager.AddLayer: GameObject es null.");
            return;
        }
        
        ParallaxLayer newLayer = new ParallaxLayer
        {
            backgroundObject = backgroundObj,
            parallaxFactor = parallaxFactor,
            zDepth = zDepth,
            infiniteTilingX = infiniteTiling,
            followCameraY = false
        };
        
        layers.Add(newLayer);
        
        // Configurar posición Z
        Vector3 pos = backgroundObj.transform.position;
        pos.z = zDepth;
        backgroundObj.transform.position = pos;
        
        // Añadir y configurar controlador
        newLayer.controller = backgroundObj.GetComponent<BackgroundParallaxFill>();
        if (newLayer.controller == null)
        {
            newLayer.controller = backgroundObj.AddComponent<BackgroundParallaxFill>();
        }
        
        ConfigureParallaxController(newLayer);
        
        Debug.Log($"BackgroundManager: Nueva capa '{backgroundObj.name}' añadida.");
    }

    /// <summary>
    /// Elimina una capa de parallax
    /// </summary>
    public void RemoveLayer(GameObject backgroundObj)
    {
        for (int i = layers.Count - 1; i >= 0; i--)
        {
            if (layers[i].backgroundObject == backgroundObj)
            {
                layers.RemoveAt(i);
                Debug.Log($"BackgroundManager: Capa '{backgroundObj.name}' eliminada.");
                return;
            }
        }
    }

    /// <summary>
    /// Cambia la resolución objetivo y reescala los fondos
    /// </summary>
    public void SetTargetResolution(int width, int height)
    {
        targetResolution = new Vector2(width, height);
        
        if (autoScaleToScreen)
        {
            ScaleBackgroundsToScreen();
        }
        
        Debug.Log($"BackgroundManager: Resolución objetivo cambiada a {width}x{height}");
    }

    /// <summary>
    /// Cambia el factor de parallax de una capa específica
    /// </summary>
    public void SetLayerParallaxFactor(int layerIndex, float factor)
    {
        if (layerIndex < 0 || layerIndex >= layers.Count)
        {
            Debug.LogWarning($"BackgroundManager.SetLayerParallaxFactor: Índice {layerIndex} fuera de rango.");
            return;
        }
        
        layers[layerIndex].parallaxFactor = Mathf.Clamp01(factor);
        ConfigureParallaxController(layers[layerIndex]);
    }

    /// <summary>
    /// Obtiene el número de capas configuradas
    /// </summary>
    public int GetLayerCount()
    {
        return layers.Count;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Validar que la resolución sea una de las soportadas
        if (!Mathf.Approximately(targetResolution.x, baseResolution1080p.x) ||
            !Mathf.Approximately(targetResolution.y, baseResolution1080p.y))
        {
            if (!Mathf.Approximately(targetResolution.x, baseResolution540p.x) ||
                !Mathf.Approximately(targetResolution.y, baseResolution540p.y))
            {
                Debug.LogWarning("BackgroundManager: Se recomienda usar 1920x1080 o 960x540 como resolución objetivo.");
            }
        }
    }
#endif
}
