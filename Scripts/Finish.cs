using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public int nivelActual = 1;
        public int finalBueno = 0;
        public int finalMalo = 0;
        public int boss1 = 0;
        public int boss2 = 0;
        public int pendingBoss = 0;
    }

    public string folderName = "Guardado";
    public string fileName = "guardado.json";
    public string cinematicasFolder = "Cinematicas"; // Resources/Cinematicas
    public float fadeDuration = 0.5f;

    bool running = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (running) return;
        if (other.CompareTag("Player") || other.name == "Player")
        {
            StartCoroutine(HandleFinish());
        }
    }

    IEnumerator HandleFinish()
    {
        running = true;
        string dir = Path.Combine(Application.persistentDataPath, folderName);
        string path = Path.Combine(dir, fileName);

        SaveData data = new SaveData();
        try
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var loaded = JsonUtility.FromJson<SaveData>(json);
                if (loaded != null) data = loaded;
            }
            else
            {
                // crear default si no existía
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(path, json);
            }
        }
        catch { }

        int currentLevel = Mathf.Max(1, data.nivelActual);
        // Buscar sprites buenoN y maloN
        Sprite sprBueno = Resources.Load<Sprite>($"{cinematicasFolder}/bueno{currentLevel}");
        Sprite sprMalo = Resources.Load<Sprite>($"{cinematicasFolder}/malo{currentLevel}");

        if (sprBueno == null && sprMalo == null)
        {
            // No hay cinematics, solo avanzar nivel con fade simple
            yield return StartCoroutine(FadeAndAdvance(data, path));
            yield break;
        }

        // Construir UI temporal
        var canvasGO = new GameObject("FinishCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Fondo negro para fade
        var blackGO = new GameObject("Black");
        blackGO.transform.SetParent(canvasGO.transform, false);
        var blackImg = blackGO.AddComponent<Image>();
        blackImg.color = new Color(0, 0, 0, 0);
        blackImg.rectTransform.anchorMin = Vector2.zero;
        blackImg.rectTransform.anchorMax = Vector2.one;
        blackImg.rectTransform.offsetMin = Vector2.zero;
        blackImg.rectTransform.offsetMax = Vector2.zero;

        // Imagen izquierda (bueno) y derecha (malo)
        Image imgBueno = null, imgMalo = null;
        if (sprBueno != null)
        {
            var go = new GameObject("Bueno");
            go.transform.SetParent(canvasGO.transform, false);
            imgBueno = go.AddComponent<Image>();
            imgBueno.sprite = sprBueno;
            imgBueno.color = new Color(1, 1, 1, 0);
            var rt = imgBueno.rectTransform;
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
        if (sprMalo != null)
        {
            var go = new GameObject("Malo");
            go.transform.SetParent(canvasGO.transform, false);
            imgMalo = go.AddComponent<Image>();
            imgMalo.sprite = sprMalo;
            imgMalo.color = new Color(1, 1, 1, 0);
            var rt = imgMalo.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        // Fade in negro
        yield return StartCoroutine(FadeImage(blackImg, 0f, 1f, fadeDuration));
        // Mostrar imágenes con fade in
        if (imgBueno != null) StartCoroutine(FadeImage(imgBueno, 0f, 1f, fadeDuration));
        if (imgMalo != null) StartCoroutine(FadeImage(imgMalo, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        // Esperar elección (B o M)
        bool decided = false;
        while (!decided)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                data.finalBueno += 1;
                decided = true;
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                data.finalMalo += 1;
                decided = true;
            }
            yield return null;
        }

        // Guardar progreso (final elegido) y avanzar nivel
        if (currentLevel % 3 == 0)
        {
            // marcar boss pendiente antes del siguiente nivel normal
            data.pendingBoss = 1;
        }
        data.nivelActual = currentLevel + 1;
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }
        catch { }

        // Fade out y recargar escena
        yield return StartCoroutine(FadeImage(blackImg, 1f, 0f, 0f)); // asegurar visible
        yield return StartCoroutine(FadeImage(blackImg, 0f, 1f, fadeDuration));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator FadeAndAdvance(SaveData data, string path)
    {
        // Fade negro simple
        var canvasGO = new GameObject("FinishCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var blackGO = new GameObject("Black");
        blackGO.transform.SetParent(canvasGO.transform, false);
        var blackImg = blackGO.AddComponent<Image>();
        blackImg.color = new Color(0, 0, 0, 0);
        blackImg.rectTransform.anchorMin = Vector2.zero;
        blackImg.rectTransform.anchorMax = Vector2.one;
        blackImg.rectTransform.offsetMin = Vector2.zero;
        blackImg.rectTransform.offsetMax = Vector2.zero;

        yield return StartCoroutine(FadeImage(blackImg, 0f, 1f, fadeDuration));
        int prevLevel = Mathf.Max(1, data.nivelActual);
        if (prevLevel % 3 == 0)
        {
            data.pendingBoss = 1;
        }
        data.nivelActual = prevLevel + 1;
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }
        catch { }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator FadeImage(Graphic img, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            var c = img.color; c.a = to; img.color = c; yield break;
        }
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            var c = img.color; c.a = a; img.color = c;
            yield return null;
        }
        var c2 = img.color; c2.a = to; img.color = c2;
    }
}
