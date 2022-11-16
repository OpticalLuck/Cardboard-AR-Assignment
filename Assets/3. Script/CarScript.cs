using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CarScript : MonoBehaviour
{
    public GameObject objectToSpawn;
    private PlacementMarkerScript placement;

    // Start is called before the first frame update
    void Start()
    {
        placement = FindObjectOfType<PlacementMarkerScript>();
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            GameObject obj = Instantiate(objectToSpawn, placement.transform.position, placement.transform.rotation);
        }
    }
}
