using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARObject : MonoBehaviour
{
    private ARRaycastManager raycastManager;

    float holdSetTimer = 1;
    float currholdTimer;
    bool touchBeganHit;
    bool canDrag;
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        GestureScript.OnPinchZoom.AddListener(OnTouchPinch);
        GestureScript.RegisterDragCallbacks(transform, () => { FindObjectOfType<TMPro.TMP_Text>().text = "HI" + Time.deltaTime.ToString(); }, 
            (delta) =>
            {
                FindObjectOfType<TMPro.TMP_Text>().text = delta.ToString();
                Vector3 deltapos = new Vector3(delta.x, 0, delta.y);
                transform.position = Vector3.Lerp(transform.position, transform.position + deltapos, Time.deltaTime);
            }, 
            () => {
                FindObjectOfType<TMPro.TMP_Text>().text = "bye" + Time.deltaTime.ToString();
            });
    }

    void Update()
    {
        /*
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                RaycastHit info;
                if(Physics.Raycast(ray, out info))
                {
                    if(info.collider.transform.IsChildOf(transform))
                    {
                        touchBeganHit = true;
                    }
                }
            }
            else if(touch.phase == TouchPhase.Stationary)
            {
                if(touchBeganHit)
                {
                    if(currholdTimer >= holdSetTimer)
                    {
                        canDrag = true;
                    }
                    else
                    {
                        currholdTimer += Time.deltaTime;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if(canDrag)
                {
                    Vector3 deltapos = new Vector3(touch.deltaPosition.x, 0, touch.deltaPosition.y);
                    transform.position = Vector3.Lerp(transform.position, transform.position + deltapos, Time.deltaTime);
                }
            }
            else if(touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                currholdTimer = 0;
                touchBeganHit = false;
                canDrag = false;
            }
        }
        */
    }

    private void OnTouchPinch(float diff)
    {
        if (gameObject.activeSelf)
        {
            float multipler = 0.005f;
            float scale = multipler * diff;
            scale = Mathf.Clamp(transform.localScale.x + scale, 0.01f, 1f);

            var newScale = new Vector3(scale, scale, scale);
            transform.localScale = newScale;
        }
    }

}
