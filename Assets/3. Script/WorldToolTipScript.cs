using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToolTipScript : MonoBehaviour
{
    [SerializeField] private bool visibleOnStart;
    private Canvas canvas;
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        gameObject.SetActive(visibleOnStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.current != null)
            transform.LookAt(Camera.current.transform, Vector3.up);
    }

    public void SetCanvasContent(GameObject prefab)
    {
        foreach(RectTransform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(prefab, canvas.transform);
    }
}
