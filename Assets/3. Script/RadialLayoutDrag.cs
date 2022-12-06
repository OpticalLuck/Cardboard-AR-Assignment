using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RadialLayoutGroup))]
[RequireComponent(typeof(Image))]
public class RadialLayoutDrag : MonoBehaviour, IDragHandler
{
    RadialLayoutGroup radialLayout;
    Image DragArea;
    [SerializeField] private float sensitivity;

    public float savedAlpha;
    public void OnDrag(PointerEventData eventData)
    {
        radialLayout.currAngle -= eventData.delta.y * sensitivity;
    }

    void Awake()
    {
        radialLayout = GetComponent<RadialLayoutGroup>();
        DragArea = GetComponent<Image>();
        savedAlpha = DragArea.color.a;
    }

    public IEnumerator LerpDragAreaOpacity(float alpha, float duration)
    {
        float timeElapsed = 0;
        float startval = DragArea.color.a;

        var color = DragArea.color;
        while (timeElapsed < duration)
        {
            SetDragAreaOpacity(Mathf.Lerp(startval, alpha, timeElapsed / duration));
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        SetDragAreaOpacity(alpha);
    }

    public void SetDragAreaOpacity(float alpha)
    {
        var color = DragArea.color;
        color.a = alpha;
        DragArea.color = color;
    }
}
