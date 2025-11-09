using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Continue : MonoBehaviour, IPointerClickHandler
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
    public Text messageLabel; // opcional: para mostrar el mensaje en rojo si falta el guardado

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ExecuteContinue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ExecuteContinue();
        }
    }

    private void ExecuteContinue()
    {
        string dir = Path.Combine(Application.persistentDataPath, folderName);
        string path = Path.Combine(dir, fileName);
        if (!File.Exists(path))
        {
            if (messageLabel != null)
            {
                messageLabel.color = Color.red;
                messageLabel.text = "no se encontro el guardado";
            }
            else
            {
                Debug.LogWarning("Continue: no se encontro el guardado");
            }
            return;
        }

        // Validar que el JSON sea legible (opcional)
        try
        {
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data == null)
            {
                throw new System.Exception("JSON inválido");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Continue: Error leyendo guardado: {ex.Message}");
            if (messageLabel != null)
            {
                messageLabel.color = Color.red;
                messageLabel.text = "no se encontro el guardado";
            }
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
