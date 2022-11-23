using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceARObjectButton : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private Button setButton;
    private ARObject prefabInstance;
    [SerializeField] private ARObject spawnPrefab;
    [SerializeField] private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        setButton = GetComponent<Button>();

        prefabInstance = Instantiate(spawnPrefab);
        prefabInstance.gameObject.SetActive(false);
        setButton.onClick.AddListener(SetPrefabLocation);
    }



    private void SetPrefabLocation()
    {
        var hitList = new List<ARRaycastHit>();

        // Raycast from the center of the screen which should hit only detected surfaces.
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinBounds | TrackableType.PlaneWithinPolygon))
        {
            // In the instance is inactive, enable it.
            if (!prefabInstance.gameObject.activeInHierarchy)
            {
                prefabInstance.gameObject.SetActive(true);
            }
            // Sort hit list base on distance to the camera.
            hitList = hitList.OrderBy(h => h.distance).ToList();
            var hitPoint = hitList[0];
            // Position instance in the closest hit point.
            prefabInstance.transform.position = hitPoint.pose.position;
            prefabInstance.transform.up = hitPoint.pose.up;
        }
        else
        {
            // In the instance is active, disable it.
            if (prefabInstance.gameObject.activeInHierarchy)
            {
                prefabInstance.gameObject.SetActive(false);
            }
        }
    }

}
