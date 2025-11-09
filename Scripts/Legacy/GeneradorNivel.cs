using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class GeneradorNivel : MonoBehaviour
{
    // --- VARIABLES DE CONFIGURACIÓN ---
    [Header("Configuración de Prefabs")]
    public GameObject prefabPared;    // Para los '1'
    public GameObject prefabInicio;   // Para la 'S'
    public GameObject prefabFinal;    // Para la 'E'
    public GameObject prefabEnemigo;  // Para la 'X' (compatibilidad retroactiva)
    public GameObject prefabPincho;   // Para la 'P'

    [Header("Tipos de Enemigos (categorización)")]
    public GameObject prefabEnemigoBasico;    // Tipo 'X' - enemigo normal
    public GameObject prefabEnemigoRapido;    // Tipo 'Y' - enemigo rápido
    public GameObject prefabEnemigoFuerte;    // Tipo 'Z' - enemigo fuerte
    public GameObject prefabEnemigoVolador;   // Tipo 'V' - enemigo volador

    [Header("Boss Prefabs")]
    public GameObject prefabBoss1;
    public GameObject prefabBoss2;

    [Header("Fondos Aleatorios (1-5)")]
    public GameObject fondo1;
    public GameObject fondo2;
    public GameObject fondo3;
    public GameObject fondo4;
    public GameObject fondo5;
    [Header("Fondos (lista opcional)")]
    public List<GameObject> fondosPrefabs = new List<GameObject>();
    [Tooltip("Margen adicional en unidades para que el fondo sobresalga por los bordes del nivel")]
    public float fondoMargen = 2f;
    [Tooltip("Si no asignas fondos arriba, intentará cargarlos automáticamente desde esta ruta en Resources")] 
    public string fondosResourcesPath = "Prefabs";

    [Header("Configuración de Niveles")]
    [Tooltip("El índice del nivel que se cargará al iniciar (0 es el primer nivel de la carpeta Maps)")]
    public int nivelACargarAlInicio = 0;

    [Header("Dimensiones del Grid")]
    public float tamanoCelda = 1.0f;

    [Header("Jugador")]
    public Transform jugadorTransform; // Arrastra el objeto del jugador aquí

    // --- VARIABLES PRIVADAS ---
    private List<TextAsset> archivosDeNivel;
    private Transform inicioTransform;
    [Header("Guardado")]
    public string folderName = "Guardado";
    public string fileName = "guardado.json";

    [System.Serializable]
    private class SaveData
    {
        public int nivelActual = 1;
        public int finalBueno = 0;
        public int finalMalo = 0;
        public int boss1 = 0; // 0: no derrotado, 1: derrotado
        public int boss2 = 0; // siguiente boss
        public int pendingBoss = 0; // 1 si debe cargar boss antes del siguiente nivel normal
    }

    private void EnsureUnlitSprite(GameObject go)
    {
        if (go == null) return;
        var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs == null || srs.Length == 0) return;
        
        foreach (var sr in srs)
        {
            if (sr == null) continue;
            
            // 1. Resetear color a blanco
            sr.color = Color.white;
            
            // 2. Usar material unlit por defecto
            sr.material = new Material(Shader.Find("Sprites/Default"));
            
            // 3. Asegurar sorting layer correcto
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 0;
            
            // 4. Validar que el sprite exista
            if (sr.sprite == null)
            {
                Debug.LogWarning($"EnsureUnlitSprite: {go.name} tiene SpriteRenderer sin sprite asignado");
            }
        }
    }

    private void EnsureUnlitForWalls(GameObject go)
    {
        if (go == null) return;
        var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs == null || srs.Length == 0) return;
        
        foreach (var sr in srs)
        {
            if (sr == null) continue;
            
            // 1. Resetear color a blanco
            sr.color = Color.white;
            
            // 2. Usar material unlit
            sr.material = new Material(Shader.Find("Sprites/Default"));
            
            // 3. Sorting layer visible (Default, no fondo)
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 10; // Sobre el fondo pero bajo otros elementos
            
            // 4. Validar sprite
            if (sr.sprite == null)
            {
                Debug.LogWarning($"EnsureUnlitForWalls: {go.name} no tiene sprite");
            }
        }
    }

    private void SpawnRandomBackgroundToFit()
    {
        // Reunir los prefabs válidos
        var fondos = new List<GameObject>();
        if (fondosPrefabs != null && fondosPrefabs.Count > 0)
        {
            foreach (var f in fondosPrefabs) if (f != null) fondos.Add(f);
        }
        if (fondo1 != null) fondos.Add(fondo1);
        if (fondo2 != null) fondos.Add(fondo2);
        if (fondo3 != null) fondos.Add(fondo3);
        if (fondo4 != null) fondos.Add(fondo4);
        if (fondo5 != null) fondos.Add(fondo5);
        if (fondos.Count == 0)
        {
            // Intentar descubrir desde Resources
            try
            {
                var loaded = Resources.LoadAll<GameObject>(fondosResourcesPath);
                if (loaded != null && loaded.Length > 0)
                {
                    foreach (var go0 in loaded)
                    {
                        if (go0 == null) continue;
                        string n = go0.name.ToLower();
                        if (n.Contains("escenario") || n.Contains("fondo") || n.Contains("background"))
                        {
                            fondos.Add(go0);
                        }
                    }
                }
            }
            catch { }
            if (fondos.Count == 0)
            {
                Debug.LogWarning($"GeneradorNivel: No se encontraron fondos asignados ni en Resources/{fondosResourcesPath}. Asigna prefabs de escenario en el inspector o mueve algunos a esa carpeta.");
                return;
            }
        }

        int idx = Random.Range(0, fondos.Count);
        var prefab = fondos[idx];
        if (prefab == null) return;

        // Calcular tamaño del nivel en unidades
        float width = Mathf.Max(1, anchoUltimoMapa) * tamanoCelda + fondoMargen * 2f;
        float height = Mathf.Max(1, altoUltimoMapa) * tamanoCelda + fondoMargen * 2f;
        // Centro del nivel
        float cx = (Mathf.Max(1, anchoUltimoMapa) * tamanoCelda) * 0.5f;
        float cy = (Mathf.Max(1, altoUltimoMapa) * tamanoCelda) * 0.5f;

        var go = Instantiate(prefab, transform);
        go.name = $"Fondo_{idx+1}";
        go.transform.position = new Vector3(cx, cy, 0f);
        
        // IMPORTANTE: Resetear scale local antes de aplicar escala
        go.transform.localScale = Vector3.one;
        
        // Asegurar que sea unlit
        EnsureUnlitSprite(go);

        // Procesar todos los SpriteRenderers
        var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs != null && srs.Length > 0)
        {
            foreach (var sr in srs)
            {
                if (sr == null) continue;
                
                // Ordenar al fondo
                sr.sortingLayerName = "Default";
                sr.sortingOrder = -100;
                
                // Intentar escalar el sprite padre
                if (sr == srs[0]) // Solo en el primer SpriteRenderer
                {
                    var size = sr.bounds.size;
                    
                    // Si bounds está vacío, obtener tamaño del sprite
                    if (size.x <= 0.0001f || size.y <= 0.0001f)
                    {
                        if (sr.sprite != null)
                        {
                            size = sr.sprite.bounds.size;
                        }
                    }
                    
                    // Escalar para cubrir el nivel
                    if (size.x > 0.0001f && size.y > 0.0001f)
                    {
                        float sx = width / size.x;
                        float sy = height / size.y;
                        float s = Mathf.Max(sx, sy);
                        
                        // Escalar el objeto raíz del fondo
                        go.transform.localScale = new Vector3(s, s, 1f);
                        
                        Debug.Log($"GeneradorNivel: Fondo escalado a ({s:F2}, {s:F2}) para cubrir nivel de {width:F1}x{height:F1}");
                    }
                    else
                    {
                        Debug.LogWarning($"GeneradorNivel: No se pudo calcular tamaño del sprite fondo ({size.x:F2}x{size.y:F2})");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"GeneradorNivel: Prefab de fondo '{prefab.name}' no tiene SpriteRenderer. Usa tiling material si es necesario.");
        }
    }

    private IEnumerator ShowIntroIfAvailable()
    {
        int level1Based = (saveCache != null) ? Mathf.Max(1, saveCache.nivelActual) : (nivelACargarAlInicio + 1);
        Sprite intro = Resources.Load<Sprite>($"Cinematicas/introduccion{level1Based}");
        if (intro == null) yield break;

        // Build overlay UI
        var canvasGO = new GameObject("IntroCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var blackGO = new GameObject("Black");
        blackGO.transform.SetParent(canvasGO.transform, false);
        var black = blackGO.AddComponent<Image>();
        black.color = new Color(0,0,0,0);
        black.rectTransform.anchorMin = Vector2.zero;
        black.rectTransform.anchorMax = Vector2.one;
        black.rectTransform.offsetMin = Vector2.zero;
        black.rectTransform.offsetMax = Vector2.zero;

        var imgGO = new GameObject("Intro");
        imgGO.transform.SetParent(canvasGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.sprite = intro;
        img.color = new Color(1,1,1,0);
        img.rectTransform.anchorMin = Vector2.zero;
        img.rectTransform.anchorMax = Vector2.one;
        img.rectTransform.offsetMin = Vector2.zero;
        img.rectTransform.offsetMax = Vector2.zero;

        float fade = 0.5f;
        yield return StartCoroutine(FadeImage(black, 0f, 1f, fade));
        yield return StartCoroutine(FadeImage(img, 0f, 1f, fade));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeImage(img, 1f, 0f, fade));
        yield return StartCoroutine(FadeImage(black, 1f, 0f, fade));
        GameObject.Destroy(canvasGO);
    }

    private IEnumerator FadeImage(Graphic g, float from, float to, float duration)
    {
        if (g == null) yield break;
        if (duration <= 0f)
        {
            var c = g.color; c.a = to; g.color = c; yield break;
        }
        float t = 0f;
        // Use unscaled time to ignore timescale changes
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            var c = g.color; c.a = a; g.color = c;
            yield return null;
        }
        var c2 = g.color; c2.a = to; g.color = c2;
    }
    private SaveData saveCache;
    private string savePath;
    private bool esNivelBoss = false;
    private int anchoUltimoMapa = 0;
    private int altoUltimoMapa = 0;

    // --- MÉTODOS DE UNITY ---
    void Start()
    {
        CargarArchivosDeNivel();
        TryLoadSaveAndApplyLevelIndex();
        StartCoroutine(RunLevelFlow());
    }

    // --- MÉTODOS PÚBLICOS ---

    private IEnumerator RunLevelFlow()
    {
        yield return StartCoroutine(ShowIntroIfAvailable());
        CargarNivel(nivelACargarAlInicio);
    }

    public void CargarNivel(int indiceNivel)
    {
        if (archivosDeNivel == null || archivosDeNivel.Count == 0)
        {
            Debug.LogError("No se encontraron archivos de nivel en la carpeta 'Resources/Maps'. Asegúrate de que existan y estén configurados como TextAsset.");
            return;
        }

        if (indiceNivel < 0 || indiceNivel >= archivosDeNivel.Count)
        {
            Debug.LogError($"Índice de nivel ({indiceNivel}) fuera de rango. Hay {archivosDeNivel.Count} niveles.");
            return;
        }

        LimpiarNivelActual();
        esNivelBoss = DebeCargarBossDesdeSave();
        if (esNivelBoss)
        {
            var bossCsv = ObtenerCsvBossParaEstado();
            if (bossCsv != null)
            {
                GenerarNivelDesdeAsset(bossCsv);
                SpawnRandomBackgroundToFit();
                InstanciarBossYJugador();
            }
            else
            {
                Debug.LogWarning("GeneradorNivel: No se encontró CSV de boss. Cargando nivel normal.");
                esNivelBoss = false;
                GenerarNivelDesdeAsset(archivosDeNivel[indiceNivel]);
                SpawnRandomBackgroundToFit();
            }
        }
        else
        {
            GenerarNivelDesdeAsset(archivosDeNivel[indiceNivel]);
            SpawnRandomBackgroundToFit();
        }
    }

    // --- MÉTODOS PRIVADOS ---

    private void CargarArchivosDeNivel()
    {
        var all = Resources.LoadAll<TextAsset>("Maps");
        archivosDeNivel = new List<TextAsset>();
        foreach (var t in all)
        {
            if (t == null) continue;
            string n = t.name.ToLower();
            if (n.Contains("boss")) continue; // excluir mapas de jefe de la lista normal
            archivosDeNivel.Add(t);
        }
        archivosDeNivel = archivosDeNivel.OrderBy(nivel => nivel.name).ToList();
    }

    private void TryLoadSaveAndApplyLevelIndex()
    {
        try
        {
            string dir = Path.Combine(Application.persistentDataPath, folderName);
            savePath = Path.Combine(dir, fileName);
            if (!File.Exists(savePath)) return;
            string json = File.ReadAllText(savePath);
            saveCache = JsonUtility.FromJson<SaveData>(json);
            if (saveCache == null) return;
            int desiredLevel = Mathf.Max(1, saveCache.nivelActual); // 1-based
            if (archivosDeNivel != null && archivosDeNivel.Count > 0)
            {
                // Convert to zero-based index clamped
                nivelACargarAlInicio = Mathf.Clamp(desiredLevel - 1, 0, archivosDeNivel.Count - 1);
            }
            else
            {
                nivelACargarAlInicio = Mathf.Max(0, desiredLevel - 1);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"GeneradorNivel: Error leyendo guardado: {ex.Message}");
        }
    }

    private bool DebeCargarBossDesdeSave()
    {
        // Cargar boss sólo si está pendiente en el guardado
        if (saveCache != null && saveCache.pendingBoss == 1) return true;
        return false;
    }

    private TextAsset ObtenerCsvBossParaEstado()
    {
        // Siempre usa el mismo mapa CSV para bosses
        var bossCsv = Resources.Load<TextAsset>("Maps/nivel_boss");
        return bossCsv;
    }

    private void LimpiarNivelActual()
    {
        foreach (Transform hijo in transform)
        {
            Destroy(hijo.gameObject);
        }
    }

    private void GenerarNivelDesdeAsset(TextAsset archivoCSV)
    {
        if (archivoCSV == null) return;

        string[] lineas = archivoCSV.text.Split('\n');
        int alto = lineas.Length;
        anchoUltimoMapa = 0;
        altoUltimoMapa = alto;
        Vector3? posicionInicio = null; // Usamos un Vector3 nulable
        inicioTransform = null;

        for (int y = 0; y < alto; y++)
        {
            int yInvertido = alto - 1 - y;
            string linea = lineas[yInvertido].Trim();
            if (string.IsNullOrEmpty(linea)) continue;

            string[] celdas = linea.Split(',');
            int ancho = celdas.Length;
            anchoUltimoMapa = Mathf.Max(anchoUltimoMapa, ancho);

            for (int x = 0; x < ancho; x++)
            {
                string celda = celdas[x].Trim();
                Vector3 posicion = new Vector3(x * tamanoCelda, y * tamanoCelda, 0);
                GameObject prefabACrear = null;

                switch (celda)
                {
                    case "1": 
                        prefabACrear = prefabPared; 
                        break;
                    case "S":
                        prefabACrear = prefabInicio;
                        posicionInicio = posicion;
                        break;
                    case "E": 
                        prefabACrear = prefabFinal; 
                        break;
                    case "X": 
                        // Enemigo normal o fallback a prefabEnemigo para compatibilidad
                        prefabACrear = prefabEnemigoBasico != null ? prefabEnemigoBasico : prefabEnemigo;
                        break;
                    case "Y": 
                        // Enemigo rápido
                        prefabACrear = prefabEnemigoRapido;
                        break;
                    case "Z": 
                        // Enemigo fuerte
                        prefabACrear = prefabEnemigoFuerte;
                        break;
                    case "V": 
                        // Enemigo volador
                        prefabACrear = prefabEnemigoVolador;
                        break;
                    case "P": 
                        prefabACrear = prefabPincho; 
                        break;
                }

                if (prefabACrear != null)
                {
                    var inst = Instantiate(prefabACrear, posicion, Quaternion.identity, transform);
                    inst.name = $"{prefabACrear.name}_{x}_{y}";
                    
                    // Solo forzar material/color en PAREDES, no en enemigos/pinchos/etc.
                    if (prefabACrear == prefabPared)
                    {
                        EnsureUnlitForWalls(inst);
                    }
                    
                    if (celda == "S")
                    {
                        inicioTransform = inst.transform;
                    }
                }
                else if (!string.IsNullOrEmpty(celda) && celda != "0")
                {
                    // Log para debugging si hay símbolos no reconocidos
                    Debug.LogWarning($"GeneradorNivel: Símbolo desconocido '{celda}' en posición ({x}, {y})");
                }
            }
        }

        // Después de crear el nivel, posicionar al jugador en el inicio
        if (jugadorTransform == null)
        {
            var goByTag = GameObject.FindWithTag("Player");
            if (goByTag != null) jugadorTransform = goByTag.transform;
            if (jugadorTransform == null)
            {
                var goByName = GameObject.Find("Player");
                if (goByName != null) jugadorTransform = goByName.transform;
            }
        }

        if (!esNivelBoss && jugadorTransform != null && posicionInicio.HasValue)
        {
            jugadorTransform.position = posicionInicio.Value;
            var rb = jugadorTransform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            var ctrl = jugadorTransform.GetComponent<ControlesPersonaje>();
            if (ctrl != null && inicioTransform != null)
            {
                ctrl.inicio = inicioTransform;
            }
        }
        else
        {
            if (jugadorTransform == null)
            {
                Debug.LogError("GeneradorNivel: No se encontró el jugador en la escena (ni asignado, ni por tag/nombre).");
            }
            if (!esNivelBoss && !posicionInicio.HasValue)
            {
                Debug.LogWarning("GeneradorNivel: No se encontró una posición de inicio 'S' en el mapa del nivel.");
            }
        }
    }

    private void InstanciarBossYJugador()
    {
        if (jugadorTransform == null)
        {
            var goByTag = GameObject.FindWithTag("Player");
            if (goByTag != null) jugadorTransform = goByTag.transform;
            if (jugadorTransform == null)
            {
                var goByName = GameObject.Find("Player");
                if (goByName != null) jugadorTransform = goByName.transform;
            }
        }

        // Esquina inferior derecha: (ancho-1, 0)
        float bx = Mathf.Max(0, anchoUltimoMapa - 1) * tamanoCelda;
        float by = 0f;
        Vector3 bossPos = new Vector3(bx, by, 0f);

        // Determinar qué boss instanciar según progreso
        GameObject bossPrefab = null;
        int nivelActual1Based = (saveCache != null) ? Mathf.Max(1, saveCache.nivelActual) : (nivelACargarAlInicio + 1);
        int justCompleted = nivelActual1Based - 1;
        if (justCompleted == 3)
        {
            bossPrefab = prefabBoss1;
        }
        else if (justCompleted == 6)
        {
            bossPrefab = (saveCache != null && saveCache.boss1 == 1 && prefabBoss2 != null) ? prefabBoss2 : prefabBoss1;
        }
        else
        {
            // Otros múltiplos de 3: si boss1 no derrotado aún, usar boss1; si ya, boss2
            bossPrefab = (saveCache != null && saveCache.boss1 == 1 && prefabBoss2 != null) ? prefabBoss2 : prefabBoss1;
        }

        if (bossPrefab != null)
        {
            var boss = Instantiate(bossPrefab, bossPos, Quaternion.identity);
            // Si el boss necesita notificar la victoria, puede llamar directamente:
            // FindObjectOfType<GeneradorNivel>()?.OnBossDefeated("Boss1" or "Boss2");
        }
        else
        {
            Debug.LogWarning("GeneradorNivel: Boss prefab no asignado.");
        }

        if (jugadorTransform != null)
        {
            // A la izquierda del boss, dos celdas
            Vector3 playerPos = new Vector3(bx - 2 * tamanoCelda, by, 0f);
            jugadorTransform.position = playerPos;
            var rb = jugadorTransform.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
            var ctrl = jugadorTransform.GetComponent<ControlesPersonaje>();
            if (ctrl != null)
            {
                ctrl.inicio = jugadorTransform; // respawn aquí en caso de muerte
            }
        }
    }

    // Llamar cuando un boss muere
    public void OnBossDefeated(string bossName)
    {
        // Marcar en save
        if (!string.IsNullOrEmpty(savePath))
        {
            if (saveCache == null) saveCache = new SaveData();
            if (!string.IsNullOrEmpty(bossName))
            {
                if (bossName.ToLower().Contains("boss1")) saveCache.boss1 = 1;
                if (bossName.ToLower().Contains("boss2")) saveCache.boss2 = 1;
            }
            saveCache.pendingBoss = 0; // limpiar bandera de boss pendiente
            GuardarSave();
        }

        // Tras derrotar al boss, cargar el nivel normal correspondiente al nivelActual actual (sin incrementarlo aquí)
        int desiredLevel1Based = (saveCache != null) ? Mathf.Max(1, saveCache.nivelActual) : (nivelACargarAlInicio + 1);
        nivelACargarAlInicio = Mathf.Clamp(desiredLevel1Based - 1, 0, (archivosDeNivel?.Count ?? 1) - 1);
        esNivelBoss = false;
        CargarNivel(nivelACargarAlInicio);
    }

    private void GuardarSave()
    {
        try
        {
            string dir = Path.Combine(Application.persistentDataPath, folderName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, fileName);
            string json = JsonUtility.ToJson(saveCache, true);
            File.WriteAllText(path, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"GeneradorNivel: Error guardando progreso: {ex.Message}");
        }
    }
}
