using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

struct MeshMaterialGroup
{
    public Transform transform;
    public List<Material> materials;
}
public class ARObject : MonoBehaviour
{
    private ARRaycastManager raycastManager;

    [SerializeField] private List<Transform> meshTransforms;
    [SerializeField] private float maxPinchScale;
    [SerializeField] private float minPinchScale;
    private List<MeshMaterialGroup> materialList = new List<MeshMaterialGroup>();
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        GestureScript.OnPinchZoom.AddListener(OnTouchPinch);
        GestureScript.RegisterDragCallbacks(transform, null, OnTouchDrag, null);

        foreach (var meshTransform in meshTransforms)
        {
            MeshMaterialGroup grp = new MeshMaterialGroup();
            grp.materials = new List<Material>();
            grp.transform = meshTransform;
            foreach (var component in meshTransform.GetComponentsInChildren<MeshRenderer>())
            {
                grp.materials.Add(component.material);

                Debug.LogFormat("MaterialGrp {0}: size = {1}", grp.transform.gameObject.name, grp.materials.Count);
            }
            materialList.Add(grp);
        }
    }

    private void OnDestroy()
    {
        GestureScript.OnPinchZoom.RemoveListener(OnTouchPinch);
        GestureScript.UnRegisterDragCallbacks(transform);
    }

    public void FocusOnSystem(int index, float transparencylevel)
    {
        if (index >= materialList.Count)
            return;

        if (index == 0) //hides internal
        {
            for (int i = 1; i < materialList.Count; i++)
            {
                var material = materialList[i];
                if (i == index)
                {
                    SetTransparencyInMaterialGroup(i, 1);
                }
                else
                {
                    SetTransparencyInMaterialGroup(i, -1);
                }
            }
            return;
        }

        SetTransparencyInMaterialGroup(0, 0.6f); //Set skin to translucent
        for (int i = 1; i < materialList.Count; i++)
        {
            if(i == index)
            {
                SetTransparencyInMaterialGroup(i, 1);
            }
            else
            {
                SetTransparencyInMaterialGroup(i, -1);
            }
        }
    }

    void SetTransparencyInMaterialGroup(int index, float transparency)
    {
        if(transparency <= 0.00001f)
        {
            materialList[index].transform.gameObject.SetActive(false);
        }
        else
        {
            materialList[index].transform.gameObject.SetActive(true);
            foreach(var material in materialList[index].materials)
            {
                var color = material.GetColor("_Color");
                color.a = transparency;
                material.SetColor("_Color", color);
            }
        }
    }

    private void OnTouchDrag(Vector2 delta)
    {
        delta = delta * 0.05f;
        Debug.Log(delta);
        Vector3 deltapos = new Vector3(delta.x, 0, delta.y);
        transform.position = Vector3.Lerp(transform.position, transform.position + deltapos, Time.deltaTime);
    }
    private void OnTouchPinch(float diff)
    {
        if (gameObject.activeSelf)
        {
            float multipler = 0.005f;
            float scale = multipler * diff;
            scale = Mathf.Clamp(transform.localScale.x + scale, minPinchScale, maxPinchScale);

            var newScale = new Vector3(scale, scale, scale);
            transform.localScale = newScale;
        }
    }

}
