using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToolTipScript : MonoBehaviour
{
    [SerializeField] private bool visibleOnStart;
    [SerializeField] private Vector3 startingScale;
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        gameObject.SetActive(visibleOnStart);
        canvas.worldCamera = Camera.main;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(startingScale.x / transform.lossyScale.x, startingScale.y / transform.lossyScale.y, startingScale.z / transform.lossyScale.z);
    }
#endif
    // Update is called once per frame
    void Update()
    {
        if (Camera.current != null)
            transform.LookAt(Camera.current.transform, Vector3.up);

        SetGlobalScale(transform.parent, startingScale);
    }

    public void SetCanvasContent(GameObject prefab)
    {
        foreach(RectTransform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(prefab, canvas.transform);
    }

    public void SetGlobalScale(Transform parent, Vector3 scale)
    {
        transform.parent = null;
        transform.localScale = scale;
        transform.parent = parent;
    }
}
