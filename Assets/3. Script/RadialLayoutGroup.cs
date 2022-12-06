using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialLayoutGroup : LayoutGroup
{
    public enum Orientation { Clockwise, AntiClockWise }
    public Orientation orientation;
    public float fDistance;
    public bool equalSpacing;
    public bool mirror;
    [Range(0f, 360f)]
    public float CurrAngle;

    [Range(0f, 360f)]
    [SerializeField] private float StartAngle, MaxAngle;

    protected override void Start()
    {
        base.Start();
        CurrAngle = StartAngle;
    }

    protected override void OnEnable() { base.OnEnable(); CalculateRadial(); }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (!Application.isPlaying)
        {
            //CurrAngle = StartAngle;
            CalculateRadial();
        }
    }
#endif
    #region Layout Group Functions
    public override void SetLayoutHorizontal()
    {
    }
    public override void SetLayoutVertical()
    {
    }
    public override void CalculateLayoutInputVertical()
    {
        CalculateRadial();
    }
    public override void CalculateLayoutInputHorizontal()
    {
        CalculateRadial();
    }
    #endregion
    void CalculateRadial()
    {
        m_Tracker.Clear();
        if (transform.childCount == 0)
            return;

        //Calculate max angle needed for equal spacing
        if (equalSpacing)
            MaxAngle = 360 - (360 / transform.childCount);

       
        //Calc offset angle
        float fOffsetAngle = (MaxAngle) / (transform.childCount - 1);
        if (orientation == Orientation.Clockwise) fOffsetAngle *= -1;

        //Clamp start angle between 0 and 360
        if (CurrAngle > 360) CurrAngle -= 360;
        else if (CurrAngle < 0) CurrAngle += 360;
        float fAngle = StartAngle + CurrAngle;

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);
            if (child != null)
            {
                if (mirror)
                {

                    while (fAngle > MaxAngle + StartAngle + (fOffsetAngle / 2))
                    {
                        fAngle -= MaxAngle + (fOffsetAngle);
                    }
                }

                //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                m_Tracker.Add(this, child,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Pivot);
                Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                child.localPosition = vPos * fDistance;
                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                fAngle += fOffsetAngle;
            }
        }

    }
    private void Update()
    {
#if DEBUG
        if(Input.GetKey(KeyCode.DownArrow))
        {
            CurrAngle += 100 * Time.deltaTime;
            CalculateRadial();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            CurrAngle -= 100 * Time.deltaTime;
            CalculateRadial();
        }
#endif  
    }
}
