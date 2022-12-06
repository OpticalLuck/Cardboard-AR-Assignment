using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RadialLayoutGroup))]
public class RadialLayoutToggle : MonoBehaviour
{
    private RadialLayoutGroup radialLayout;
    [SerializeField] Button toggleButton;
    [SerializeField] bool startAsClosed;
    [SerializeField] float duration;
    float timeElapsed;
    float savedRadius;

    // Start is called before the first frame update
    void Start()
    {
        radialLayout = GetComponent<RadialLayoutGroup>();
        toggleButton.onClick.AddListener(ToggleLayout);

        savedRadius = radialLayout.radius;
        if (startAsClosed)
        {
            radialLayout.radius = 0;
            GetComponent<RadialLayoutDrag>().SetDragAreaOpacity(0);
            gameObject.SetActive(false);
        }
    }

    private void ToggleLayout()
    {
        var dragComponent = GetComponent<RadialLayoutDrag>();

        if (gameObject.activeSelf)
        {
            foreach (Button btn in radialLayout.GetComponentsInChildren<Button>())
                btn.interactable = false;
            StartCoroutine(routine: ExpandLayout(0, () => gameObject.SetActive(false)));
            StartCoroutine(routine: dragComponent.LerpDragAreaOpacity(0, duration));
        }
        else
        {
            gameObject.SetActive(true);
            StartCoroutine(routine: ExpandLayout(savedRadius, () => 
            {
                foreach (Button btn in radialLayout.GetComponentsInChildren<Button>())
                    btn.interactable = true;
            }));
            StartCoroutine(routine: dragComponent.LerpDragAreaOpacity(dragComponent.savedAlpha, duration));
        }
    }

    public IEnumerator ExpandLayout(float endVal, Action onCompleted)
    {
        timeElapsed = 0;
        float startval = radialLayout.radius;

        while (timeElapsed < duration)
        {
            radialLayout.radius = Mathf.Lerp(startval, endVal, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        radialLayout.currAngle = 0;
        radialLayout.radius = endVal;
        onCompleted?.Invoke();
    }
}
