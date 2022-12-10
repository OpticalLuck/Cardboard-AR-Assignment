using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;

    PlacementMarkerScript placementMarker;
    [Header("ARObject Spawning")]
    private ARObject objectInstance;
    [SerializeField] private GameObject spawnLayer;
    [SerializeField] private ARObject aRObjectPrefab;
    [SerializeField] private Button spawnObjectButton;

    [Header("Body System Select")]
    [SerializeField] private GameObject selectionLayer;
    [SerializeField] private TMP_Text header;
    [SerializeField] private Button despawnObjectButton;
    [SerializeField] private Button resetOrientationButton;
    [SerializeField] private RadialLayoutGroup selectionGroup;

    
    void Awake()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        placementMarker = FindObjectOfType<PlacementMarkerScript>();
    }

    void Start()
    {
        InitSpawnLayer();
        InitSelectionLayer();
        ToggleUILayer(spawnLayer);

#if DEBUG
        objectInstance = FindObjectOfType<ARObject>();
        if(objectInstance != null)
            LinkButtonToObjectInstance();
#endif
    }

    void InitSpawnLayer()
    {
        spawnObjectButton.onClick.AddListener(() =>
        {
            if (ARObjectSpawnerScript.SpawnObject(raycastManager, ref objectInstance, aRObjectPrefab))
                OnObjectSpawned();

        });
    }

    void InitSelectionLayer()
    {
        despawnObjectButton.onClick.AddListener(OnObjectDestoy);
        resetOrientationButton.onClick.AddListener(() => objectInstance.transform.up = Vector3.up);
    }

    void OnObjectSpawned()
    {
        LinkButtonToObjectInstance();
        ToggleUILayer(selectionLayer);
        placementMarker.enabled = false;

        planeManager.enabled = false;
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(false);

        raycastManager.raycastPrefab.GetComponent<ARPlaneMeshVisualizer>().trackingStateVisibilityThreshold = UnityEngine.XR.ARSubsystems.TrackingState.None;
        foreach(var plane in FindObjectsOfType<ARPlaneMeshVisualizer>())
        {
            plane.trackingStateVisibilityThreshold = UnityEngine.XR.ARSubsystems.TrackingState.None;
        }
    }

    void OnObjectDestoy()
    {
        placementMarker.enabled = true;

        planeManager.enabled = true;
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(true);

        Destroy(objectInstance.gameObject);
        ToggleUILayer(spawnLayer);
        foreach (var btn in selectionGroup.GetComponentsInChildren<Button>())
            btn.onClick.RemoveAllListeners();
    }

    void LinkButtonToObjectInstance()
    {
        int i = 0;
        foreach (var btn in selectionGroup.GetComponentsInChildren<Button>())
        {
            int temp = i;
            btn.onClick.AddListener(() => objectInstance.FocusOnSystem(temp));
            btn.onClick.AddListener(() => { header.text = objectInstance.GetSystemName(temp); });
            i++;
        }

        header.text = objectInstance.GetSystemName(0);
    }

    void ToggleUILayer(GameObject uiLayer)
    {
        spawnLayer.SetActive(uiLayer == spawnLayer);
        selectionLayer.SetActive(uiLayer == selectionLayer);

    }
}
