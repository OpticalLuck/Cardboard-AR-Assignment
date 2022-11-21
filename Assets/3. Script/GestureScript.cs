using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestureScript : MonoBehaviour
{
    private static GestureScript script;

    private UnityEvent<float> OnPinchZoom;
    private UnityEvent<Vector2> OnDrag;
    void Awake()
    {
        if (script == null)
            script = this;
        else
            Destroy(script);

        DontDestroyOnLoad(this);

        OnPinchZoom = new UnityEvent<float>();
        OnDrag = new UnityEvent<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
            TwoFingerGestureCheck();
    }

    #region Events
    public static void SubscribeToPinch(UnityAction<float> action, bool subscribe = true)
    {
        if (action != null)
        {
            if(subscribe)
                script.OnPinchZoom.AddListener(action);
            else
                script.OnPinchZoom.RemoveListener(action);
        }
        else
            Debug.LogError("SubscribeToPinch Subscribe Action is null!");
    }
    #endregion

    #region Gestures
    void TwoFingerGestureCheck()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchZOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = GetPrevTouchPosition(touchZero);
        Vector2 touchOnePrevPos = GetPrevTouchPosition(touchZOne);

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currMagnitude = (touchZero.position - touchZOne.position).magnitude;

        float diff = currMagnitude - prevMagnitude;

        if (Mathf.Abs(diff) >= 30f)
            OnPinchZoom.Invoke(diff);
        else
        {
            Vector2 midpt = new Vector3( (touchZero.position.x + touchZOne.position.x)/2, (touchZero.position.y + touchZOne.position.y) / 2);
            Vector2 prevmidpt = new Vector3( (touchZeroPrevPos.x + touchOnePrevPos.x)/2, (touchZeroPrevPos.y + touchOnePrevPos.y) / 2);
            OnDrag.Invoke(midpt - prevmidpt);
        }
    }

    Vector2 GetPrevTouchPosition(Touch touch) 
    {
        return touch.position - touch.deltaPosition;
    }
    #endregion
}
