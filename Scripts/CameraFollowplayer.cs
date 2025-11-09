using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowplayer : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;
    float initialZ;
  
    void Start()
    {
        initialZ = transform.position.z;
        if (target == null)
        {
            var byTag = GameObject.FindWithTag("Player");
            if (byTag != null) target = byTag.transform;
        }
        if (target == null)
        {
            var byName = GameObject.Find("Player");
            if (byName != null) target = byName.transform;
        }
        if (target == null)
        {
            var controller = FindObjectOfType<ControlesPersonaje>();
            if (controller != null) target = controller.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            var byTag = GameObject.FindWithTag("Player");
            if (byTag != null) target = byTag.transform;
            if (target == null)
            {
                var byName = GameObject.Find("Player");
                if (byName != null) target = byName.transform;
            }
            if (target == null)
            {
                var controller = FindObjectOfType<ControlesPersonaje>();
                if (controller != null) target = controller.transform;
            }
        }
        if (target == null) return;
        Vector3 pos = transform.position;
        float x = followX ? target.position.x + offset.x : pos.x;
        float y = followY ? target.position.y + offset.y : pos.y;
        transform.position = new Vector3(x, y, initialZ);
    }
}
