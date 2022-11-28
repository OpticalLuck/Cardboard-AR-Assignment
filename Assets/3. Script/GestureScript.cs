using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
public class GestureScript : MonoBehaviour
{
    private static GestureScript script;
    public enum GestureEvent { None, PinchZoom, Drag1, Drag2 }
    GestureEvent currGestureEvent = GestureEvent.None;
    GestureEvent prevGestureEvent = GestureEvent.None;

    public static UnityEvent<float> OnPinchZoom => script.onPinchZoom;
    public static UnityEvent<Vector2> OnTwoFingerDrag => script.onTwoFingerDrag;
    public static UnityEvent<Vector3> OnDeviceTap => script.onTouchTap;

    private UnityEvent<Vector3> onTouchTap = new UnityEvent<Vector3>();
    private UnityEvent<float> onPinchZoom = new UnityEvent<float>();
    private UnityEvent<Vector2> onTwoFingerDrag = new UnityEvent<Vector2>();

    void Awake()
    {
        if (script == null)
            script = this;
        else
            Destroy(script);

        DontDestroyOnLoad(this);
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
            if (CanChangeGestureTo(GestureEvent.PinchZoom))
            {
                SetGestureEvent(GestureEvent.PinchZoom);
                onPinchZoom.Invoke(diff);
            }
        }
        else
        {
            var t0_Dir = touchZero.deltaPosition;
            var t1_Dir = touchZOne.deltaPosition;

            float minMovement = 5f;
            bool oneFingerNotMoving = touchZero.deltaPosition.magnitude < minMovement || touchZero.deltaPosition.magnitude < minMovement;
            if (Vector2.Angle(t0_Dir, t1_Dir) < 25 && !oneFingerNotMoving && CanChangeGestureTo(GestureEvent.Drag2))
            {
                SetGestureEvent(GestureEvent.Drag2);
                onTwoFingerDrag.Invoke(new Vector2((t0_Dir.x + t1_Dir.x) / 2, (t0_Dir.y + t1_Dir.y) / 2));
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

        //public UnityAction beginDrag;
        public UnityAction<Vector2> duringDrag;
        //public UnityAction endDrag;
    }
    List<DragData> recordedobj;

    float tapDurationThreshold = 0.5f;
    float longtapDuration = 1.0f;
    public static void RegisterDragCallbacks(Transform target, UnityAction beginDrag, UnityAction<Vector2> dragging, UnityAction endDrag, float beginDragTimer = 1)
    {
        if (script.recordedobj == null)
            script.recordedobj = new List<DragData>();
        DragData data = new DragData();
        data.objTransform = target;
        data.setTimer = beginDragTimer;
        //data.beginDrag = beginDrag;
        data.duringDrag = dragging;
        //data.endDrag = endDrag;

        data.currTimer = 0;
        data.canDrag = false;
        data.touchBeginHit = false;

        script.recordedobj.Add(data);
    }
    public static void UnRegisterDragCallbacks(Transform target)
    {
        script.recordedobj.RemoveAll(x => x.objTransform == target);
    }
    void OneFingerGestureCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        RaycastHit info;
        if (Physics.Raycast(ray, out info, LayerMask.GetMask("ARObject")))
        {
            for (int i = 0; i < recordedobj.Count; i++)
            {
                if (CanChangeGestureTo(GestureEvent.Drag1))
                {
                    var tmp = recordedobj[i];
                    SetGestureEvent(GestureEvent.Drag1);
                    DragCheck(ref tmp, info);
                    recordedobj[i] = tmp;
                }
            }
        }
    }

    #endregion
    void DragCheck(ref DragData target, RaycastHit info)
    {
        Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
        if (touch.phase == TouchPhase.Began)
        {
            if (info.collider.transform.IsChildOf(target.objTransform) || info.collider.transform == target.objTransform)
            {
                target.touchBeginHit = true;
                //target.beginDrag?.Invoke();
            }
        }
        else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            if (target.currTimer <= tapDurationThreshold)
                onTouchTap.Invoke(info.point);

            target.currTimer = 0;
            target.touchBeginHit = false;
            target.canDrag = false;

            //target.endDrag?.Invoke();
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

    bool CanChangeGestureTo(GestureEvent g_Event)
    {
        return IsCurrGestureEvent(GestureEvent.None) || IsCurrGestureEvent(g_Event);
    }
}