using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestureScript : MonoBehaviour
{
    private static GestureScript script;
    public enum GestureEvent { None, PinchZoom, Drag }
    GestureEvent currGestureEvent = GestureEvent.None;
    GestureEvent prevGestureEvent = GestureEvent.None;

    public static UnityEvent<float> OnPinchZoom => script.onPinchZoom;

    private UnityEvent<float> onPinchZoom;
    void Awake()
    {
        if (script == null)
            script = this;
        else
            Destroy(script);

        DontDestroyOnLoad(this);

        onPinchZoom = new UnityEvent<float>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
            OneFingerGestureCheck();
        else if (Input.touchCount == 2)
            TwoFingerGestureCheck();
        else
            SetGestureEvent(GestureEvent.None);
    }

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

        if (Mathf.Abs(diff) >= 12f)
        {
            if (IsCurrGestureEvent(GestureEvent.Drag) || IsCurrGestureEvent(GestureEvent.None))
            {
                SetGestureEvent(GestureEvent.Drag);
                onPinchZoom.Invoke(diff);
            }
        }
    }

    struct DragData
    {
        public Transform objTransform;
        public float setTimer;

        public float currTimer;
        public bool touchBeginHit;
        public bool canDrag;

        public UnityAction beginDrag;
        public UnityAction<Vector2> duringDrag;
        public UnityAction endDrag;
    }

    public static void RegisterDragCallbacks(Transform target, UnityAction beginDrag, UnityAction<Vector2> dragging, UnityAction endDrag, float beginDragTimer = 1)
    {
        if (script.recordedobj == null)
            script.recordedobj = new List<DragData>();
        DragData data = new DragData();
        data.objTransform = target;
        data.setTimer = beginDragTimer;
        data.beginDrag = beginDrag;
        data.duringDrag = dragging;
        data.endDrag = endDrag;

        data.currTimer = 0;
        data.canDrag = false;
        data.touchBeginHit = false;
        
        script.recordedobj.Add(data);
    }
    List<DragData> recordedobj;
    void OneFingerGestureCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        RaycastHit info;
        if (Physics.Raycast(ray, out info))
        {
            foreach (var obj in recordedobj)
            {
                DragCheck(obj, info);
            }
        }
    }

    void DragCheck(DragData target, RaycastHit info)
    {
        Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
        if (touch.phase == TouchPhase.Began)
        {
            if (info.collider.transform.IsChildOf(target.objTransform) || info.collider.transform == target.objTransform)
            {
                target.touchBeginHit = true;
                target.beginDrag?.Invoke();
            }
        }
        
        else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            target.currTimer = 0;
            target.touchBeginHit = false;
            target.canDrag = false;

            target.endDrag?.Invoke();
        }
        else
        {
            if (target.canDrag)
            {
                target.duringDrag?.Invoke(touch.deltaPosition);
            }
            else
            {
                if (target.touchBeginHit)
                {
                    if (target.currTimer >= target.setTimer)
                    {
                        target.canDrag = true;
                    }
                    else
                    {
                        target.currTimer += Time.deltaTime;
                    }
                }
            }
        }
    }
    Vector2 GetPrevTouchPosition(Touch touch) 
    {
        return touch.position - touch.deltaPosition;
    }
    void SetGestureEvent(GestureEvent g_Event)
    {
        if (currGestureEvent != g_Event)
        {
            prevGestureEvent = currGestureEvent;
            currGestureEvent = g_Event;
        }
    }
    bool IsCurrGestureEvent(GestureEvent g_Event)
    {
        return currGestureEvent == g_Event;
    }
    #endregion
}
