using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradordeBichos : MonoBehaviour
{
    public GameObject EnemyPrefab;


    void GenerarBicho()
    {
        if (EnemyPrefab != null)
        {
            var instancia = Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
            var renderers = instancia.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in renderers)
            {
                r.sortingOrder = 1;
            }
        }
        else
        {
            GameObject enemigo = CrearEnemigoBasico();
            enemigo.transform.position = transform.position;
        }

    }
    void Start()
    {
        if (EnemyPrefab == null)
        {
            var loaded = Resources.Load<GameObject>("Prefabs/EnemyPrefab");
            if (loaded != null)
            {
                EnemyPrefab = loaded;
                Debug.Log("GeneradordeBichos: EnemyPrefab cargado desde Resources.");
            }
            else
            {
                Debug.LogWarning("GeneradordeBichos: EnemyPrefab no asignado. Se usará un enemigo básico generado por código.");
            }
        }
        InvokeRepeating("GenerarBicho", 2.0f, 3.0f);

    
    }

  
    void Update()
    {
      

    }

    GameObject CrearEnemigoBasico()
    {
        // tuve que crear porque JAMAS me cargo el prefab
        GameObject go = new GameObject("Enemy");
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = false;
        var enemyController = go.AddComponent<EnemyController>();
        enemyController.enabled = true; 
        var sr = go.AddComponent<SpriteRenderer>();
        var square = Resources.Load<Sprite>("Images/op");
        if (square != null)
        {
            sr.sprite = square;
        }
        sr.sortingOrder = 1;
        return go;
    }
}
