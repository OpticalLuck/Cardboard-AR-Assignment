using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    private ARRaycastManager raycastManager;

    [Header("ARObject Spawning")]
    private ARObject objectInstance;
    [SerializeField] private GameObject spawnLayer;
    [SerializeField] private ARObject aRObjectPrefab;
    [SerializeField] private Button spawnObjectButton;

    [Header("Body System Select")]
    [SerializeField] private GameObject selectionLayer;
    [SerializeField] private Button despawnObjectButton;
    [SerializeField] private Button resetOrientationButton;
    [SerializeField] private GameObject selectionGroup;
    private List<Button> selectionButtons = new List<Button>();

    
    void Awake()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Start()
    {
        spawnObjectButton.onClick.AddListener(() => 
        {
            if(ARObjectSpawnerScript.SpawnObject(raycastManager, ref objectInstance, aRObjectPrefab))
                OnObjectSpawned();
        });
        despawnObjectButton.onClick.AddListener(OnObjectDestoy);
        resetOrientationButton.onClick.AddListener(() => objectInstance.transform.up = Vector3.up);

        foreach (var btn in selectionGroup.GetComponentsInChildren<Button>(true))
            selectionButtons.Add(btn);

        ToggleUILayer(spawnLayer);
#if DEBUG
        objectInstance = FindObjectOfType<ARObject>();
        if(objectInstance != null)
            LinkButtonToObjectInstance();
#endif
    }

    void OnObjectSpawned()
    {
        LinkButtonToObjectInstance();
        ToggleUILayer(selectionLayer);
    }

    void OnObjectDestoy()
    {
        Destroy(objectInstance.gameObject);
        ToggleUILayer(spawnLayer);

        foreach (var btn in selectionButtons)
            btn.onClick.RemoveAllListeners();
    }

    void LinkButtonToObjectInstance()
    {
        int i = 1;
        foreach (var btn in selectionButtons)
        {
            int temp = i;
            btn.onClick.AddListener(() => objectInstance.FocusOnSystem(temp));
            i++;
        }
    }

    void ToggleUILayer(GameObject uiLayer)
    {
        spawnLayer.SetActive(uiLayer == spawnLayer);
        selectionLayer.SetActive(uiLayer == selectionLayer);

    }
}
