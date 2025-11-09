using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public class SaveData
    {
        public int nivelActual = 1;
        public int finalBueno = 0;
        public int finalMalo = 0;
    }

    public string folderName = "Guardado";
    public string fileName = "guardado.json";
    public string sceneToLoad = "SampleScene";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        ExecuteNewGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ExecuteNewGame();
        }
    }

    private void ExecuteNewGame()
    {
        string dir = Path.Combine(Application.persistentDataPath, folderName);
        string path = Path.Combine(dir, fileName);
        try
        {
            Debug.Log($"NewGame: creando guardado en {path}");
            Directory.CreateDirectory(dir);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("NewGame: guardado anterior eliminado");
            }

            var data = new SaveData { nivelActual = 1, finalBueno = 0, finalMalo = 0 };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log("NewGame: guardado creado correctamente");

            Time.timeScale = 1f;
            try
            {
                Debug.Log($"NewGame: cargando escena '{sceneToLoad}'");
                SceneManager.LoadScene(sceneToLoad);
            }
            catch (System.Exception ex2)
            {
                Debug.LogError($"NewGame: Error cargando escena '{sceneToLoad}'. Asegúrate de agregarla en Build Settings. Detalle: {ex2.Message}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"NewGame: Error creando guardado: {ex.Message}");
        }
    }
}
