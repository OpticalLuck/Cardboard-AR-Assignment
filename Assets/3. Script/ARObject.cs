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

[Serializable]
struct ColliderToCanvasLinker
{
    public List<Collider> colliders;
    public GameObject prefab;
}
public class ARObject : MonoBehaviour
{
    private ARRaycastManager raycastManager;

    [SerializeField] private List<Transform> meshTransforms;
    [SerializeField] private float minPinchScale;
    [SerializeField] private float maxPinchScale;
    [SerializeField] private WorldToolTipScript toolTip;
    [SerializeField] private List<ColliderToCanvasLinker> Information;

    public LayerMask skinLayer;
    public LayerMask internalLayer;

    private List<MeshMaterialGroup> materialList = new List<MeshMaterialGroup>();

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        GestureScript.OnPinchZoom.AddListener(ScaleObject);
        GestureScript.OnTwoFingerDrag.AddListener(RotateObject);
        GestureScript.RegisterDragCallbacks(transform, null, MoveObject, null);
        
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

        FocusOnSystem(0);
    }
    private void OnDestroy()
    {
        GestureScript.OnPinchZoom.RemoveListener(ScaleObject);
        GestureScript.UnRegisterDragCallbacks(transform);
        GestureScript.OnTwoFingerDrag.RemoveListener(RotateObject);
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            CheckBodyTap();
        }
    }

    private void CheckBodyTap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        RaycastHit info;
        if (Physics.Raycast(ray: ray,
            hitInfo: out info,
            maxDistance: Mathf.Infinity,
            layerMask: internalLayer))
        {
            Testtooltip(info.point, info.collider);
        }
    }
    private void Testtooltip(Vector3 hit, Collider collider)
    {
        toolTip.gameObject.SetActive(true);
        toolTip.transform.position = hit;
        var content = Information.Find(x => x.colliders.Find(y => y == collider)).prefab;
        if(content != null)
            toolTip.SetCanvasContent(content);
    }

    public void FocusOnSystem(int index)
    {
        if (index >= materialList.Count)
            return;

        toolTip.gameObject.SetActive(false);
        
        if (index == 0) //hides internal
        {
            for (int i = 0; i < materialList.Count; i++)
            {
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

        
        SetTransparencyInMaterialGroup(0, 0.6f, false); //Set skin to translucent
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

    void SetTransparencyInMaterialGroup(int index, float transparency, bool raycast = true)
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
                string param_Opacity = "_Opacity";
                if (material.HasFloat(param_Opacity))
                {
                    var alpha = material.GetFloat(param_Opacity);
                    alpha = transparency;
                    material.SetFloat(param_Opacity, alpha);
                }

                string param_Color = "_Color";
                if (material.HasColor(param_Color))
                {
                    var color = material.GetColor(param_Color);
                    color.a = transparency;
                    material.SetColor(param_Color, color);
                }
            }
        }
    }

    private void MoveObject(Vector2 delta)
    {
        delta = delta * 0.05f;
        Debug.Log(delta);
        Vector3 deltapos = new Vector3(delta.x, 0, delta.y);
        transform.position = Vector3.Lerp(transform.position, transform.position + deltapos, Time.deltaTime);
    }

    private void RotateObject(Vector2 delta)
    {
        delta = delta * 0.5f;
        transform.Rotate(Vector3.down, delta.x, Space.World);
        transform.Rotate(Vector3.right, delta.y, Space.World);
    }

    private void ScaleObject(float diff)
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
