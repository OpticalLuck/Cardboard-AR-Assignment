using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectSpawnerScript : MonoBehaviour
{
    public static bool SpawnObject(ARRaycastManager raycastManager, ref ARObject prefabInstance, ARObject prefab)
    {
        if (prefabInstance != null)
            return false;

        var hitList = new List<ARRaycastHit>();
        // Raycast from the center of the screen which should hit only detected surfaces.
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinBounds | TrackableType.PlaneWithinPolygon))
        {
            prefabInstance = Instantiate(prefab);

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
            return true;
        }

        return false;
    }

}
