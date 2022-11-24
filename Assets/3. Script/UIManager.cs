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
    [SerializeField] private ARObject aRObjectPrefab;
    [SerializeField] private Button spawnObjectButton;
    [SerializeField] private Button despawnObjectButton;
    
    void Awake()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Start()
    {
        spawnObjectButton.onClick.AddListener(() =>
        {
            if(ARObjectSpawnerScript.SpawnObject(raycastManager, ref objectInstance, aRObjectPrefab))
            {
                spawnObjectButton.gameObject.SetActive(false);
                despawnObjectButton.gameObject.SetActive(true);
            }
        });

        despawnObjectButton.gameObject.SetActive(false);
        despawnObjectButton.onClick.AddListener(() => { 
            Destroy(objectInstance.gameObject); 
            despawnObjectButton.gameObject.SetActive(false); 
            spawnObjectButton.gameObject.SetActive(true);
        });
    }

}
