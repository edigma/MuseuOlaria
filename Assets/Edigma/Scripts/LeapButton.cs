using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LeapButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Color startColor;
    public Color selectColor;
    Image image;

    bool selected = false;
    float progress = 0.0f;

    public UnityEvent clicked;

    void Start()
    {
        if (clicked == null)
            clicked = new UnityEvent();
        image = GetComponent<Image>();
        image.color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.Lerp(startColor, selectColor, progress);
    }

    public void Off()
    {
        selected = false;
        progress = 0.0f;
    }

    public void On()
    {
        selected = true;
    }

    public void DoClick() {
        clicked.Invoke();
    }

    public void Progress(float p)
    {
        progress = p;
    }
}
