using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToolTipScript : MonoBehaviour
{
    [SerializeField] private bool visibleOnStart;
    
    void Start()
    {
        gameObject.SetActive(visibleOnStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.current != null)
            transform.LookAt(Camera.current.transform, Vector3.up);
    }
}
